using MQTTnet;
using MQTTnet.Client;
using System.Text;

namespace MqttSniffer.Service;

internal interface IPublisher
{
    Task Initialize();
    Task SendMessage(string topic, string message);

    void SetMessageCallback(Action<MqttMessage> action);
}

internal class MqttInteraction(string url) : IPublisher
{
    private readonly string url = url;
    private IMqttClient mqttClient = default!;

    public async Task Initialize()
    {
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(url, 1883)
            .Build();

        mqttClient.ConnectedAsync += async e =>
        {
            var options = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter("FX/FromNode")
                .WithTopicFilter("FX/FromPython")
                .Build();

            await mqttClient.SubscribeAsync(options);
        };

        await mqttClient.ConnectAsync(options, CancellationToken.None);
    }

    public async Task SendMessage(string topic, string messageString)
    {
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(messageString)
            .Build();

        await mqttClient.PublishAsync(message);
    }

    public void SetMessageCallback(Action<MqttMessage> action)
    {
        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var message = new MqttMessage(
                e.ApplicationMessage.Topic,
                Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));
            action(message);

            return Task.CompletedTask;
        };
    }
}