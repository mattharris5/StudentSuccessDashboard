using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SSD.Repository;

namespace SSD.DependencyInjection
{
    public class RepositoryContainerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            container.Register(Classes.FromAssemblyContaining<IRepositoryContainer>()
                .InSameNamespaceAs<IRepositoryContainer>(true)
                .WithServiceDefaultInterfaces()
                .If(t => t.GetInterface("IRepositoryContainer") != null)
                .LifestylePerWebRequest());
        }
    }
}