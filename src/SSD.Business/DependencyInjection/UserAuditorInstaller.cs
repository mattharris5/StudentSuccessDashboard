using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SSD.Security;
using System;

namespace SSD.DependencyInjection
{
    public class UserAuditorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            container.Register(Component.For<IUserAuditor>()
                .ImplementedBy<UserAuditor>()
                .LifestyleSingleton());
        }
    }
}