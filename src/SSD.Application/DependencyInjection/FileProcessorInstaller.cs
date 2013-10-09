using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SSD.Controllers;
using SSD.IO;
using System;

namespace SSD.DependencyInjection
{
    public class FileProcessorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            container.Register(Component.For<IFileProcessor>()
                .LifestylePerWebRequest()
                .UsingFactoryMethod(Create));
        }

        private IFileProcessor Create(IKernel kernel, CreationContext context)
        {
            string typeKey = null;
            if (context.Handler.ComponentModel.Name == typeof(ServiceOfferingController).FullName)
            {
                typeKey = "ServiceOffering";
            }
            else if (context.Handler.ComponentModel.Name == typeof(ServiceAttendanceController).FullName)
            {
                typeKey = "ServiceAttendance";
            }
            return FileProcessorFactory.Create(kernel, typeKey);
        }
    }
}
