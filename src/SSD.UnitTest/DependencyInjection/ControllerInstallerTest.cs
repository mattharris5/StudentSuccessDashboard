using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SSD.DependencyInjection
{
    [TestClass]
    public class ControllerInstallerTest
    {
        private ControllerInstaller Target { get; set; }
        private IWindsorContainer Container { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new ControllerInstaller();
            Container = new WindsorContainer().Install(Target);
        }

        [TestMethod]
        public void GivenNoContainer_WhenIInstall_ThenThrowArgumentNullException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Install(null, new DefaultConfigurationStore()));
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenRegistrationsExist_AndAllRegisteredImplementIController()
        {
            var allHandlers = Container.GetAllHandlers();
            var controllerHandlers = Container.GetHandlersFor(typeof(IController));
            Assert.AreNotEqual(0, allHandlers.Length);
            CollectionAssert.AreEqual(allHandlers, controllerHandlers);
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenAllControllersAreRegistered()
        {
            var allControllers = typeof(HomeController).Assembly.GetPublicClasses(c => c.Is<IController>());
            var registeredControllers = Container.GetImplementationTypesFor(typeof(IController));
            CollectionAssert.AreEquivalent(allControllers, registeredControllers);
        }

        [TestMethod]
        public void GivenRegistrationIsnstalled_ThenAllControllersHaveSuffix_AndOnlyControllersHaveSuffix()
        {
            var allControllers = typeof(HomeController).Assembly.GetPublicClasses(c => c.Name.EndsWith("Controller"));
            var registeredControllers = Container.GetImplementationTypesFor(typeof(IController));
            CollectionAssert.AreEquivalent(allControllers, registeredControllers);
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenAllControllersLiveInControllersNamespace_AndOnlyControllersLiveInControllersNamespace()
        {
            var allControllers = typeof(HomeController).Assembly.GetPublicClasses(c => c.Namespace.Contains("Controllers"));
            var registeredControllers = Container.GetImplementationTypesFor(typeof(IController));
            CollectionAssert.AreEquivalent(allControllers, registeredControllers);
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenAllControllersHaveExpectedLifestyle()
        {
            var invalidControllers = Container.GetHandlersFor(typeof(IController))
                .Where(controller => controller.ComponentModel.LifestyleType != LifestyleType.PerWebRequest)
                .ToArray();
            Assert.AreEqual(0, invalidControllers.Length);
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenAllControllersExposeThemselvesAsAService()
        {
            var invalidControllers = Container.GetHandlersFor(typeof(IController))
                .Where(controller => controller.ComponentModel.Services.First() != controller.ComponentModel.Implementation)
                .ToArray();
            Assert.AreEqual(0, invalidControllers.Length);
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenAllRegistrationsAreNamedAfterTheirType()
        {
            var invalidControllers = Container.GetHandlersFor(typeof(IController))
                .Where(controller => controller.ComponentModel.Name != controller.ComponentModel.Implementation.FullName)
                .ToArray();
            Assert.AreEqual(0, invalidControllers.Length);
        }
    }
}
