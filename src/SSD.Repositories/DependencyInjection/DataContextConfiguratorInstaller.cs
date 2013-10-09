using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.WindowsAzure;
using SSD.Data;
using System;

namespace SSD.DependencyInjection
{
    public class DataContextConfiguratorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            container.Register(Component.For<IDataContextConfigurator>()
                .ImplementedBy<DataContextConfigurator>()
                .UsingFactoryMethod(Create)
                .LifestyleSingleton());
        }

        private static IDataContextConfigurator Create()
        {
            var instance = new DataContextConfigurator();
            instance.EnableCaching = bool.Parse(CloudConfigurationManager.GetSetting("EnableDataLayerCaching"));
            instance.EnableTracing = bool.Parse(CloudConfigurationManager.GetSetting("EnableDataLayerTracing"));
            return instance;
        }
    }
}