using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SSD.Data;
using System;

namespace SSD.DependencyInjection
{
    public class DataContextInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            container.Register(Component.For<IEducationContext>()
                .ImplementedBy<EducationDataContext>()
                .UsingFactoryMethod(EducationDataContextFactory.Create)
                .LifestylePerWebRequest());
        }
    }
}