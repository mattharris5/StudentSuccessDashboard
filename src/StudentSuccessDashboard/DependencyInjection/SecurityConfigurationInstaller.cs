using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SSD.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSD.DependencyInjection
{
    public class SecurityConfigurationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            container.Register(Component.For<ISecurityConfiguration>().ImplementedBy<DefaultSecurityConfiguration>().LifestyleSingleton());
        }
    }
}