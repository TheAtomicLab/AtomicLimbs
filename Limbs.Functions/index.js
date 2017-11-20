var jscad = require('@jscad/openjscad');
var azure = require('azure-storage');
var fs = require('fs');

var AzureFunction = function (context, myQueueItem) {
    
    var messageData = JSON.parse(myQueueItem);
    
    var script = 'function main() { return [ torus() ] }';
    var params = {}

    context.log("compiling with JSCad");
    jscad.compile(script, params).then(function (compiled) {
        context.log("OK compiling with JSCad");

        var outputData = jscad.generateOutput('stlb', compiled);
        var fileName = "product-file-" + new Date().getMilliseconds() + ".stl";
        
        fs.writeFileSync(fileName, outputData.asBuffer());
        fs.readFile(fileName, function (err, data) {
            var bs = azure.createBlobService();
            context.log("uploading file to Azure");
            bs.createBlockBlobFromText("orderproductgenerated", fileName, data, function (error, result, response) {
                if (error) {
                    context.log("ERROR uploading file to Azure " + fileName);
                    context.log(error);
                    return;
                }
                context.log("OK uploading file to Azure");

                messageData.FileUrl = fileName;

                var retryOperations = new azure.ExponentialRetryPolicyFilter();
                var queueSvc = azure.createQueueService().withFilter(retryOperations);
                queueSvc.createMessage('orderproductgeneratorresult', new Buffer(JSON.stringify(messageData)).toString('base64'), function (err) {
                    if (!err) {
                        context.log("OK sending message to queue");
                    } else {
                        context.log("ERROR sending message to queue");
                    }
                });
                context.log("deleting from fs");
                fs.unlink(fileName, (er) => {
                    if (er) {
                        console.log('ERROR deleting from fs' + fileName);
                        return;
                    }
                    console.log('OK deleting from fs');
                });
            });
        });
    });
    context.done();
};

module.exports = AzureFunction;