using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.ViewModels.DataTables;
using System;
using System.Linq;

namespace SSD.DependencyInjection
{
    [TestClass]
    public class DataTableBinderInstallerTest
    {
        private DataTableBinderInstaller Target { get; set; }
        private IWindsorContainer Container { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new DataTableBinderInstaller();
            Container = new WindsorContainer().Install(Target);
        }

        [TestMethod]
        public void GivenNullContainer_WhenInstall_ThenArgumentNullExceptionThrown()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Install(null, new DefaultConfigurationStore()));
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_WhenGetHandlers_ThenRegistrationsExist_AndAllHandlersRegistered()
        {
            var allHandlers = Container.GetAllHandlers();
            var actualHandlers = Container.GetHandlersFor(typeof(IDataTableBinder));
            Assert.AreNotEqual(0, allHandlers.Length);
            CollectionAssert.AreEqual(allHandlers, actualHandlers);
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenAllHandlersHaveExpectedLifestyle()
        {
            var invalidHandlers = Container.GetHandlersFor(typeof(IDataTableBinder))
                .Where(handler => handler.ComponentModel.LifestyleType != LifestyleType.Singleton)
                .ToArray();
            Assert.AreEqual(0, invalidHandlers.Length);
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenImplementationsRegistered()
        {
            var allTypes = typeof(DataTableBinder).Assembly.GetPublicClasses(c => c.Is<IDataTableBinder>());
            var actualTypes = Container.GetImplementationTypesFor(typeof(IDataTableBinder));
            CollectionAssert.AreEqual(allTypes, actualTypes);
        }

    }
}
