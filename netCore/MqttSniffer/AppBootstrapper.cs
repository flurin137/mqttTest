using Caliburn.Micro;
using MqttSniffer.Service;
using MqttSniffer.ViewModels;
using Ninject;
using System;
using System.Collections.Generic;
using System.Windows;

namespace MqttSniffer
{
    public class AppBootstrapper : BootstrapperBase
    {
        private IKernel kernel;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            kernel = new StandardKernel();
            
            kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();

            var publisher = new MqttInteraction("127.0.0.1");
            publisher.Initialize();

            kernel.Bind<IPublisher>().ToConstant(publisher);

            kernel.Bind<IMainViewModel>().To<MainViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return kernel.Get(service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return kernel.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            kernel.Inject(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            kernel.Dispose();
            base.OnExit(sender, e);
        }
    }
}
