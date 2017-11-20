require('dotenv').config({ path: 'config.env' });

var AzureFunction = require('./index');

// Local development queue message
var myQueueItem = '{"ProductSizes":{"Id":0,"A":0.2,"B":0.3,"C":0.4,"D":0},"OrderId":94,"Pieces":{"AtomicLabCover":true,"FingerMechanismHolder":true,"Fingers":true,"FingerStopper":true,"FingersX1":true,"FingersX2P":true,"Palm":true,"ThumbConnector":true,"Thumb":true,"ThumbClip":true,"ThumbScrew":true,"UpperArm_FingerConnector":true,"UpperArm_PalmConnector":true,"UpperArm_ThumbShortConnector":true,"UpperArm_FingerSlider":true,"UpperArm":true}}';

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