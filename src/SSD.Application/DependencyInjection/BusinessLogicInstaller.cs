using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SSD.Business;
using SSD.Controllers;
using System;

namespace SSD.DependencyInjection
{
    public class BusinessLogicInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            container.Register(Classes.FromAssemblyContaining<IProviderManager>()
                .InSameNamespaceAs<IProviderManager>(true)
                .WithServiceDefaultInterfaces()
                .LifestylePerWebRequest());
            container.Register(Component.For<ICustomFieldManager>()
                .LifestylePerWebRequest()
                .UsingFactoryMethod(Create));
        }

        private ICustomFieldManager Create(IKernel kernel, CreationContext context)
        {
            if (context.Handler.ComponentModel.Name == typeof(PublicController).FullName
                || context.Handler.ComponentModel.Name == typeof(DataFileController).FullName
                || context.Handler.ComponentModel.Name == typeof(CustomFieldController).FullName)
            {
                return kernel.Resolve<PublicFieldManager>();
            }
            else if (context.Handler.ComponentModel.Name == typeof(PrivateHealthController).FullName)
            {
                return kernel.Resolve<PrivateHealthFieldManager>();
            }
            throw new InvalidOperationException(string.Format("Cannot resolve {0} using given {1}.", typeof(ICustomFieldManager).Name, context.GetType().Name));
        }
    }
}