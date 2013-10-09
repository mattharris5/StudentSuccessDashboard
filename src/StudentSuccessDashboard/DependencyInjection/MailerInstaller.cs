using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SSD.Security.Net;
using System;

namespace SSD.DependencyInjection
{
    public class MailerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            container.Register(Component.For<IMailer>().ImplementedBy<Mailer>().LifestyleSingleton());
        }
    }
}