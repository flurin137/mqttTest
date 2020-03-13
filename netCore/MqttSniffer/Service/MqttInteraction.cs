using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MqttSniffer.Service
{
    internal interface IPublisher
    {
        Task Initialize();
        Task SendMessage(string topic, string message);

        void SetMessageCallback(Action<MqttMessage> action);
    }

    internal class MqttInteraction : IPublisher
    {
        private readonly string url;
        private IMqttClient mqttClient;

        public MqttInteraction(string url)
        {
            this.url = url;
        }

        public async Task Initialize()
        {
            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(url, 1883)
                .Build();

            mqttClient.UseConnectedHandler(async e =>
            {
                var topicFilters = new TopicFilterBuilder()
                    .WithTopic("FX/FromNode")
                    .WithTopic("FX/FromPython")
                    .Build();

                await mqttClient.SubscribeAsync(topicFilters);
            });

            await mqttClient.ConnectAsync(options, CancellationToken.None);
        }

        public async Task SendMessage(string topic, string message)
        {
            await mqttClient.PublishAsync(topic, message);
        }

        public void SetMessageCallback(Action<MqttMessage> action)
        {
            mqttClient.UseApplicationMessageReceivedHandler(
                e => action(new MqttMessage(e.ApplicationMessage.Topic, Encoding.UTF8.GetString(e.ApplicationMessage.Payload))));
        }
    }
}