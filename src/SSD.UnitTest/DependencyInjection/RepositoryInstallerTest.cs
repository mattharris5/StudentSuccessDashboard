using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Data;
using SSD.Repository;
using System;
using System.Linq;

namespace SSD.DependencyInjection
{
    [TestClass]
    public class RepositoryInstallerTest
    {
        private RepositoryInstaller Target { get; set; }
        private IWindsorContainer Container { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new RepositoryInstaller();
            Container = new WindsorContainer().Install(Target);
        }

        [TestMethod]
        public void GivenNoContainer_WhenIInstall_ThenThrowArgumentNullException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Install(null, new DefaultConfigurationStore()));
        }

        //[TestMethod]
        //public void GivenRegistrationsInstalled_ThenRegistrationsExist_AndAllRegisteredImplementIRepository()
        //{
        //    var allHandlers = Container.GetAllHandlers();
        //    var repositoryHandlers = Container.GetHandlersFor(typeof(IRepository<object>));
        //    Assert.AreNotEqual(0, allHandlers.Length);
        //    CollectionAssert.AreEqual(allHandlers, repositoryHandlers);
        //}

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenAllRepositoriesAreRegistered()
        {
            var allRepositories = typeof(IRepository<object>).Assembly.GetPublicClasses(c => c.Is<IRepository<object>>());
            var registeredRepositories = Container.GetImplementationTypesFor(typeof(IRepository<object>));
            CollectionAssert.AreEqual(allRepositories, registeredRepositories);
        }

        //[TestMethod]
        //public void GivenRegistrationsInstalled_ThenAllRepositoriesHaveSuffix_AndOnlyRepositoriesHaveSuffix()
        //{
        //    var allRepositories = typeof(IRepository<object>).Assembly.GetPublicClasses(c => c.Name.EndsWith("Repository"));
        //    var registeredRepositories = Container.GetImplementationTypesFor(typeof(IRepository<object>));
        //    CollectionAssert.AreEqual(allRepositories, registeredRepositories);
        //}

        //[TestMethod]
        //public void GivenRegistrationsInstalled_ThenAllRepositoriesLiveInRepositoryNamespace_AndOnlyRepositoriesLiveInRepositoryNamespace()
        //{
        //    var allRepositories = typeof(IRepository<object>).Assembly.GetPublicClasses(c => c.Namespace.Contains("Repository"));
        //    var registeredRepositories = Container.GetImplementationTypesFor(typeof(IRepository<object>));
        //    CollectionAssert.AreEqual(allRepositories, registeredRepositories);
        //}

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenAllRepositoriesHaveExpectedLifestyle()
        {
            var invalidRepositories = Container.GetHandlersFor(typeof(IRepository<object>))
                .Where(handler => handler.ComponentModel.LifestyleType != LifestyleType.PerWebRequest)
                .ToArray();
            Assert.AreEqual(0, invalidRepositories.Count());
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenAllRepositoriesExposeThemselvesAsAService()
        {
            var invalidRepositories = Container.GetHandlersFor(typeof(IRepository<object>))
                .Where(handler => handler.ComponentModel.Services.First() != handler.ComponentModel.Implementation)
                .ToArray();
            Assert.AreEqual(0, invalidRepositories.Length);
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenAllRegistrationsAreNamedAfterTheirType()
        {
            var invalidRepositories = Container.GetHandlersFor(typeof(IRepository<object>))
                .Where(handler => handler.ComponentModel.Name != handler.ComponentModel.Implementation.FullName)
                .ToArray();
            Assert.AreEqual(0, invalidRepositories.Length);
        }
    }
}
