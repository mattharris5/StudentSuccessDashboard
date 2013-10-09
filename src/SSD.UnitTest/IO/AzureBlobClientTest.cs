using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.IO
{
    [TestClass]
    public class AzureBlobClientTest
    {
        [TestMethod]
        public void GivenNullStorageAccount_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new AzureBlobClient(null));
        }
    }
}
