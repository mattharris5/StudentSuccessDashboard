using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SSD.IO;
using System;

namespace SSD.DependencyInjection
{
    public class BlobClientInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            container.Register(Component.For<IBlobClient>()
                .ImplementedBy<AzureBlobClient>()
                .LifestyleSingleton()
                .UsingFactoryMethod(AzureBlobClientFactory.Create));
        }
    }
}