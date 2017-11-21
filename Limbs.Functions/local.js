require('dotenv').config({ path: 'config.env' });

var AzureFunction = require('./index');

// Local development queue message
var myQueueItem = '{"ProductSizes":{"Id":0,"A":4.32,"B":6.19,"C":5.0,"D":0},"OrderId":13,"Pieces":{"AtomicLabCover":false,"FingerMechanismHolder":false,"Fingers":false,"FingerStopper":false,"FingersX1":false,"FingersX2P":false,"Palm":false,"ThumbConnector":false,"Thumb":false,"ThumbClip":false,"ThumbScrew":false,"UpperArm_FingerConnector":false,"UpperArm_PalmConnector":false,"UpperArm_ThumbShortConnector":false,"UpperArm_FingerSlider":false,"UpperArm":false}}';

// Local development context
var debugContext = {
    invocationId: 'ID',
    bindings: {
        myQueueItem
    },
    log: function () {
        var util = require('util');
        var val = util.format.apply(null, arguments);
        console.log(val);
    },
    done: function () {
        // When done is called, it will log the response to the console
        console.log('Response: OK');
    },
    res: null
};

// Call the AzureFunction locally with your testing params
AzureFunction(debugContext, myQueueItem);