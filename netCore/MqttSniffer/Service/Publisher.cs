using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MqttSniffer.Service
{
    interface IPublisher
    {
        Task Initialize();
        Task SendMessage(string topic, string message);
    }

    class Publisher : IPublisher
    {
        private readonly string url;
        private IMqttClient mqttClient;

        public Publisher(string url)
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
            
            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine($"+ Message = {e.ApplicationMessage.Topic} => {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            });

            mqttClient.UseConnectedHandler(async e =>
            {
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("FX/#").Build());
            });

            await mqttClient.ConnectAsync(options, CancellationToken.None);
        }

        public async Task SendMessage(string topic, string message)
        {
            await mqttClient.PublishAsync(topic, message);
        }
    }
}