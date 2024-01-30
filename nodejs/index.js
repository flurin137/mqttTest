var mqtt = require('mqtt')
var client = mqtt.connect('mqtt://localhost:1883')

client.on('connect', function () {
    console.log("Starting Listening")

    client.subscribe("FX/FromCSharp")
    client.subscribe("FX/FromPython")
})

client.on('message', function (topic, message) {
    console.log(topic.toString());
    console.log(message.toString());
})