var jscad = require('@jscad/openjscad');
var azure = require('azure-storage');
var fs = require('fs');

var AzureFunction = function (context, myQueueItem) {

    try {
        var messageData = JSON.parse(JSON.stringify(myQueueItem));
        var fileName = "product-" + messageData.OrderId + "-file-" + new Date().getMilliseconds() + ".stl";
        context.log(fileName);
        
        //RENDER JSCAD MODEL//////////////////////////////////////////////////////////
        var script = 'function main() { return [ torus() ] }';
        var params = {}

        context.log("compiling with JSCad");
        jscad.compile(script, params).then(function (compiled) {
            context.log("OK compiling with JSCad");
            
            context.log("generating output with JSCad");
            var outputData = jscad.generateOutput('stlb', compiled);
            context.log("OK generating output with JSCad");

            //TEMPORAL COMPILED FILE//////////////////////////////////////////////////////
            
            context.log("saving temp file lo local fs");
            fs.writeFileSync(getUserHome() + fileName, outputData.asBuffer());
            context.log("OK saving temp file lo local fs");

            context.log("read file from local fs");
            var data = fs.readFileSync(getUserHome() + fileName);
            context.log("OK read file from local fs");

            //UPLOAD TO BLOB STORAGE//////////////////////////////////////////////////////

            context.log("uploading file to Azure");
            var bs = azure.createBlobService();
            bs.createBlockBlobFromText("orderproductgenerated", fileName, data, function (e) { return e; });
            context.log("OK uploading file to Azure");

            //SET BLOB URL TO ORDER///////////////////////////////////////////////////////
            var startDate = new Date();
            var expiryDate = new Date(startDate);
            expiryDate.setYear(startDate.getFullYear() + 100);
            startDate.setDate(startDate.getDate() - 1);
            var sharedAccessPolicy = {
                AccessPolicy: {
                    Permissions: azure.BlobUtilities.SharedAccessPermissions.READ,
                    Start: startDate,
                    Expiry: expiryDate
                }
            };
            var sasToken = bs.generateSharedAccessSignature("orderproductgenerated", fileName, sharedAccessPolicy);
            var sasUrl = bs.getUrl("orderproductgenerated", fileName, sasToken);
            messageData.FileUrl = sasUrl;
            
            //NOTIFY THE NEW COMPILED FILE////////////////////////////////////////////////

            context.log("sending message to queue");
            var retryOperations = new azure.ExponentialRetryPolicyFilter();
            var queueSvc = azure.createQueueService().withFilter(retryOperations);
            queueSvc.createMessage('orderproductgeneratorresult', new Buffer(JSON.stringify(messageData)).toString('base64'), function (e) { return e; });
            context.log("OK sending message to queue");

            //DELETE TEMP FILE////////////////////////////////////////////////////////////

            context.log("deleting from fs");
            fs.unlinkSync(getUserHome() + fileName);
            context.log('OK deleting from fs');

            //////////////////////////////////////////////////////////////////////////////

            context.log("OK bye");
            context.done();
        });
    } catch (e) {
        context.log("ERROR");
        context.log(e);
        context.done(e);
    } 
};

function getUserHome() {
    return process.env.TMP + "\\";
}

module.exports = AzureFunction;