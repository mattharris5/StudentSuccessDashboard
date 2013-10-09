using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SSD.Repository;

namespace SSD.DependencyInjection
{
    public class RepositoryInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            container.Register(Classes.FromAssemblyContaining<IRepository<object>>()
                .InSameNamespaceAs<IRepository<object>>(true)
                .WithServiceDefaultInterfaces()
                .If(t => t.GetInterface("IRepository`1") != null)
                .LifestylePerWebRequest());
        }
    }
}