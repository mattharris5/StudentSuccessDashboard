using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Hosting;
using System.Web.Optimization;

namespace SSD
{
    [TestClass]
    public class BundleConfigTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void WhenRegisterBundles_ThenAllBundlePathsEndWithBundle()
        {
            BundleTable.VirtualPathProvider = new TestVirtualPathProvider();
            BundleCollection bundles = new BundleCollection();
            BundleConfig.RegisterBundles(bundles);
            foreach (Bundle bundle in bundles)
            {
                Assert.IsTrue(bundle.Path.EndsWith("bundle", StringComparison.OrdinalIgnoreCase));
            }
        }

        private class TestVirtualPathProvider : VirtualPathProvider
        {
            public override string CombineVirtualPaths(string basePath, string relativePath)
            {
                return relativePath;
            }
        }
    }
}
