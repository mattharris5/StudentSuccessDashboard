using Castle.MicroKernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SSD.Data
{
    [TestClass]
    public class EducationDataContextFactoryTest
    {
        private IDataContextConfigurator MockConfigurator { get; set; }
        private IKernel MockKernel { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockConfigurator = MockRepository.GenerateMock<IDataContextConfigurator>();
            MockKernel = MockRepository.GenerateMock<IKernel>();
            MockKernel.Expect(m => m.Resolve<IDataContextConfigurator>()).Return(MockConfigurator);
        }

        [TestMethod]
        public void WhenCreate_ThenGetInstance()
        {
            using (EducationDataContext actual = EducationDataContextFactory.Create(MockKernel))
            {
                Assert.IsNotNull(actual);
            }
        }

        [TestMethod]
        public void GivenInjectedCache_WhenCreate_ThenContextCacheIsSet()
        {
            using (EducationDataContext actual = EducationDataContextFactory.Create(MockKernel))
            {
                MockConfigurator.AssertWasCalled(m => m.Configure(MockKernel, actual));
            }
        }

        [TestMethod]
        public void WhenCreate_ThenContextConnectionStringComesFromConfiguration()
        {
            string expected = ConfigurationManager.AppSettings["DatabaseConnectionString"];

            using (EducationDataContext actual = EducationDataContextFactory.Create(MockKernel))
            {
                Assert.IsTrue(actual.Database.Connection.ConnectionString.Contains(expected)); // NOTE: Won't be equal because the connection is wrapped for caching/tracing
            }
        }
    }
}
