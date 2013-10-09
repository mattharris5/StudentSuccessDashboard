using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EFCachingProvider.Caching;
using SSD.Data;
using System;

namespace SSD.DependencyInjection
{
    public class CacheInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            container.Register(Component.For<ICache>()
                .ImplementedBy<AppFabricCache>()
                .UsingFactoryMethod(Create)
                .LifestylePerWebRequest());
        }

        private ICache Create(IKernel kernel, CreationContext context)
        {
            return new AppFabricCache("default");
        }
    }
}
