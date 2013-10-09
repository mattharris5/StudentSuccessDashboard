using Castle.MicroKernel;
using EFCachingProvider.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace SSD.Data
{
    [TestClass]
    public class DataContextConfiguratorTest
    {
        private const string TestDatabaseName = @"SSD.Data.EducationDataContext";

        private IKernel MockKernel { get; set; }
        private DataContextConfigurator Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockKernel = MockRepository.GenerateMock<IKernel>();
            Target = new DataContextConfigurator();
        }

        [TestMethod]
        public void WhenConfigure_ThenDatabaseInitialized()
        {
            using (EducationDataContext actual = new EducationDataContext())
            {
                Target.Configure(MockKernel, actual);

                Assert.IsTrue(actual.Database.Exists());
            }
        }

        [TestMethod]
        public void GivenEnableCachingTrue_WhenConfigure_ThenContextCachingConfigured()
        {
            ICache expected = MockRepository.GenerateMock<ICache>();
            MockKernel.Expect(m => m.Resolve<ICache>()).Return(expected);
            Target.EnableCaching = true;

            using (EducationDataContext actual = new EducationDataContext())
            {
                Target.Configure(MockKernel, actual);

                Assert.AreEqual(expected, actual.Cache);
                Assert.AreEqual(CachingPolicy.CacheAll, actual.CachingPolicy);
            }
        }

        [TestMethod]
        public void GivenEnableCachingFalse_WhenConfigure_ThenContextCachingNotConfigured()
        {
            ICache expected = MockRepository.GenerateMock<ICache>();
            MockKernel.Expect(m => m.Resolve<ICache>()).Return(expected);
            Target.EnableCaching = false;

            using (EducationDataContext actual = new EducationDataContext())
            {
                Target.Configure(MockKernel, actual);

                Assert.IsNull(actual.Cache);
                Assert.AreEqual(CachingPolicy.NoCaching, actual.CachingPolicy);
            }
        }

        [TestMethod]
        public void GivenEnableTracingTrue_WhenConfigure_ThenContextTracingConfigured()
        {
            Target.EnableTracing = true;

            using (EducationDataContext actual = new EducationDataContext())
            {
                Target.Configure(MockKernel, actual);

                Assert.IsNotNull(actual.Log);
            }
        }

        [TestMethod]
        public void GivenEnableTracingFalse_WhenConfigure_ThenContextTracingNotConfigured()
        {
            Target.EnableTracing = false;

            using (EducationDataContext actual = new EducationDataContext())
            {
                Target.Configure(MockKernel, actual);

                Assert.IsNull(actual.Log);
            }
        }
    }
}
