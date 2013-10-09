using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.DependencyInjection;
using System.IO;
using System.Reflection;
using System.Web.Mvc;

namespace SSD
{
    [TestClass]
    public class DependencyInjectionConfigTest
    {
        [TestInitialize]
        public void InitializeTest()
        {
            DependencyInjectionConfig.AssemblySearchPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        [TestMethod]
        public void GivenDependencyInjectionNotRegistered_WhenIRegisterDependencyInjection_ThenControllerFactoryChanged()
        {
            IControllerFactory notExpected = ControllerBuilder.Current.GetControllerFactory();

            DependencyInjectionConfig.RegisterDependencyInjection();
            IControllerFactory actual = ControllerBuilder.Current.GetControllerFactory();

            Assert.AreNotEqual(notExpected, actual);
        }

        [TestMethod]
        public void GivenDependencyInjectionNotRegistered_WhenIRegisterDependencyInjection_ThenControllerFactoryUsesWindsor()
        {
            DependencyInjectionConfig.RegisterDependencyInjection();
            IControllerFactory actual = ControllerBuilder.Current.GetControllerFactory();

            Assert.IsInstanceOfType(actual, typeof(WindsorControllerFactory));
        }

        [TestMethod]
        public void GivenDependencyInjectionNotRegistered_WhenIRegisterDependencyInjection_ThenDependencyResolverChanged()
        {
            IDependencyResolver notExpected = DependencyResolver.Current;

            DependencyInjectionConfig.RegisterDependencyInjection();
            IDependencyResolver actual = DependencyResolver.Current;

            Assert.AreNotEqual(notExpected, actual);
        }

        [TestMethod]
        public void GivenDependencyInjectionNotRegistered_WhenIRegisterDependencyInjection_ThenDependencyResolverUsesWindsor()
        {
            DependencyInjectionConfig.RegisterDependencyInjection();
            IDependencyResolver actual = DependencyResolver.Current;

            Assert.IsInstanceOfType(actual, typeof(WindsorDependencyResolver));
        }

        [TestMethod]
        public void GivenDependencyInjectionRegistered_WhenIReleaseDependencyInjection_ThenDependencyResolverChanged()
        {
            IControllerFactory expectedControllerFactory = ControllerBuilder.Current.GetControllerFactory();
            IDependencyResolver expectedDependencyResolver = DependencyResolver.Current;
            DependencyInjectionConfig.RegisterDependencyInjection();

            DependencyInjectionConfig.ReleaseDependencyInjection();
            IControllerFactory actualControllerFactory = ControllerBuilder.Current.GetControllerFactory();
            IDependencyResolver actualDependencyResolver = DependencyResolver.Current;

            Assert.AreEqual(expectedControllerFactory, actualControllerFactory);
            Assert.AreEqual(expectedDependencyResolver, actualDependencyResolver);
        }
    }
}
