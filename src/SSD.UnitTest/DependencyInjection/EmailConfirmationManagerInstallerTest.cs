using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Security;
using System;
using System.Linq;

namespace SSD.DependencyInjection
{
    [TestClass]
    public class EmailConfirmationManagerInstallerTest
    {
        private EmailConfirmationManagerInstaller Target { get; set; }
        private IWindsorContainer Container { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new EmailConfirmationManagerInstaller();
            Container = new WindsorContainer().Install(Target);
        }

        [TestMethod]
        public void GivenNoContainer_WhenIInstall_ThenThrowArgumentNullException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Install(null, new DefaultConfigurationStore()));
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenRegistrationsExist_AndAllHandlersRegistered()
        {
            var allHandlers = Container.GetAllHandlers();
            var actualHandlers = Container.GetHandlersFor(typeof(IEmailConfirmationManager));
            Assert.AreNotEqual(0, allHandlers.Length);
            CollectionAssert.AreEqual(allHandlers, actualHandlers);
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenAllHandlersHaveExpectedLifestyle()
        {
            var invalidHandlers = Container.GetHandlersFor(typeof(IEmailConfirmationManager))
                .Where(handler => handler.ComponentModel.LifestyleType != LifestyleType.Singleton)
                .ToArray();
            Assert.AreEqual(0, invalidHandlers.Length);
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenImplementationsRegistered()
        {
            var allTypes = typeof(IEmailConfirmationManager).Assembly.GetPublicClasses(c => c.Is<IEmailConfirmationManager>());
            var actualTypes = Container.GetImplementationTypesFor(typeof(IEmailConfirmationManager));
            CollectionAssert.AreEqual(allTypes, actualTypes);
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenAllRegistrationsAreNamedAfterTheirType()
        {
            var invalidHandlers = Container.GetHandlersFor(typeof(IEmailConfirmationManager))
                .Where(handler => handler.ComponentModel.Name != handler.ComponentModel.Implementation.FullName)
                .ToArray();
            Assert.AreEqual(0, invalidHandlers.Length);
        }
    }
}
