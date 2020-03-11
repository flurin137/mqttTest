using Caliburn.Micro;
using MqttSniffer.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttSniffer.ViewModels
{
    internal interface IMainViewModel
    {
        string Output { get; set; }
        string Message { get; set; }
        string Topic { get; set; } 
        Task SendMessage();

    }

    internal class MainViewModel : Screen, IMainViewModel
    {
        private string console;
        private readonly IPublisher publisher;

        public string Output
        {
            get => console; set
            {
                console = value;
                NotifyOfPropertyChange();
            }
        }

        public string Message { get; set; } = "message";
        public string Topic { get; set; } = "FX";

        public MainViewModel(IPublisher publisher)
        {
            this.publisher = publisher;
        }

        public async Task SendMessage()
        {
            await publisher.SendMessage(Topic, Message);
        }
    }
}
