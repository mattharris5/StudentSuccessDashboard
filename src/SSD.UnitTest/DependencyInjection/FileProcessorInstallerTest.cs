using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.ModelBuilder;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Releasers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.Controllers;
using SSD.IO;
using SSD.Repository;
using SSD.Security.Permissions;
using System;
using System.Linq;

namespace SSD.DependencyInjection
{
    [TestClass]
    public class FileProcessorInstallerTest
    {
        private FileProcessorInstaller Target { get; set; }
        private IWindsorContainer Container { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new FileProcessorInstaller();
            Container = new WindsorContainer().Install(Target);
        }

        [TestMethod]
        public void GivenNullContainer_WhenInstall_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Install(null, new DefaultConfigurationStore()));
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenAllHandlersHaveExpectedLifestyle()
        {
            var invalidHandlers = Container.GetHandlersFor(typeof(IFileProcessor))
                .Where(handler => handler.ComponentModel.LifestyleType != LifestyleType.PerWebRequest)
                .ToArray();
            Assert.AreEqual(0, invalidHandlers.Length);
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_AndRequestingComponentIsServiceAttendanceController_WhenResolve_ThenReturnServiceAttendanceFileProcessor()
        {
            Container = new WindsorContainer();
            Container.Kernel.ComponentModelBuilder.AddContributor(new TransientEqualizer());
            Container.Install(Target);
            Container.Register(Component.For<IBlobClient>().Instance(MockRepository.GenerateMock<IBlobClient>()));
            Container.Register(Component.For<IRepositoryContainer>().Instance(MockRepository.GenerateMock<IRepositoryContainer>()));
            Container.Register(Component.For<IPermissionFactory>().Instance(MockRepository.GenerateMock<IPermissionFactory>()));
            var handler = Container.GetHandlersFor(typeof(IFileProcessor)).Single();
            var context = new CreationContext(handler, new LifecycledComponentsReleasePolicy(Container.Kernel), typeof(IFileProcessor), null, null, null);
            handler.ComponentModel.Name = typeof(ServiceAttendanceController).FullName;

            var actual = handler.Resolve(context);

            Assert.IsInstanceOfType(actual, typeof(ServiceAttendanceFileProcessor));
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_AndRequestingComponentIsServiceOfferingController_WhenResolve_ThenReturnServiceOfferingFileProcessor()
        {
            Container = new WindsorContainer();
            Container.Kernel.ComponentModelBuilder.AddContributor(new TransientEqualizer());
            Container.Install(Target);
            Container.Register(Component.For<IBlobClient>().Instance(MockRepository.GenerateMock<IBlobClient>()));
            Container.Register(Component.For<IRepositoryContainer>().Instance(MockRepository.GenerateMock<IRepositoryContainer>()));
            Container.Register(Component.For<IPermissionFactory>().Instance(MockRepository.GenerateMock<IPermissionFactory>()));
            var handler = Container.GetHandlersFor(typeof(IFileProcessor)).Single();
            var context = new CreationContext(handler, new LifecycledComponentsReleasePolicy(Container.Kernel), typeof(IFileProcessor), null, null, null);
            handler.ComponentModel.Name = typeof(ServiceOfferingController).FullName;

            var actual = handler.Resolve(context);

            Assert.IsInstanceOfType(actual, typeof(ServiceOfferingFileProcessor));
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_AndRequestingComponentIsUnknown_WhenResolve_ThenReturnNullYieldsException()
        {
            Container = new WindsorContainer();
            Container.Kernel.ComponentModelBuilder.AddContributor(new TransientEqualizer());
            Container.Install(Target);
            Container.Register(Component.For<IBlobClient>().Instance(MockRepository.GenerateMock<IBlobClient>()));
            Container.Register(Component.For<IRepositoryContainer>().Instance(MockRepository.GenerateMock<IRepositoryContainer>()));
            Container.Register(Component.For<IPermissionFactory>().Instance(MockRepository.GenerateMock<IPermissionFactory>()));
            var handler = Container.GetHandlersFor(typeof(IFileProcessor)).Single();
            var context = new CreationContext(handler, new LifecycledComponentsReleasePolicy(Container.Kernel), typeof(IFileProcessor), null, null, null);

            Target.ExpectException<ComponentActivatorException>(() => handler.Resolve(context));
        }

        public class TransientEqualizer : IContributeComponentModelConstruction
        {
            public void ProcessModel(IKernel kernel, ComponentModel model)
            {
                if (model.LifestyleType == LifestyleType.PerWebRequest)
                {
                    model.LifestyleType = LifestyleType.Singleton;
                }
            }
        }
    }
}
