using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SSD.IO
{
    [TestClass]
    public class AzureBlobClientFactoryTest
    {
        [TestMethod]
        public void WhenCreate_ThenInstanceReturned()
        {
            AzureBlobClient actual = AzureBlobClientFactory.Create();

            Assert.IsNotNull(actual);
        }
    }
}
