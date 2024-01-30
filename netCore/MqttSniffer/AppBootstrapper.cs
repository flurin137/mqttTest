using MqttSniffer.Service;
using MqttSniffer.ViewModels;
using Stylet;
using StyletIoC;

namespace MqttSniffer;

internal class AppBootstrapper : Bootstrapper<MainViewModel>
{
    protected override void ConfigureIoC(IStyletIoCBuilder builder)
    {
        builder.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
        builder.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();

        var publisher = new MqttInteraction("127.0.0.1");
        publisher.Initialize().Wait();

        builder.Bind<IPublisher>().ToInstance(publisher);

        builder.Bind<IMainViewModel>().To<MainViewModel>();
    }
}
