namespace MqttSniffer.Service;

internal class MqttMessage
{
    public string Topic { get; }
    public string Message { get; }

    public MqttMessage(string topic, string message)
    {
        Topic = topic;
        Message = message;
    }
}
