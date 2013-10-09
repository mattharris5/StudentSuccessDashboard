using System;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SSD.Controllers;

namespace SSD.DependencyInjection
{
    public class ControllerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            container.Register(Classes.FromAssemblyContaining<HomeController>()
                .BasedOn<IController>()
                .LifestylePerWebRequest());
        }
    }
}