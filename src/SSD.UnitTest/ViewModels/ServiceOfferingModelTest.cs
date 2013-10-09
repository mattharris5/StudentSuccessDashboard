using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.ViewModels
{
    [TestClass]
    public class ServiceOfferingModelTest
    {
        private ServiceOfferingModel Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new ServiceOfferingModel();
        }

        [TestMethod]
        public void GivenNullModel_WhenCopyTo_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.CopyTo(null));
        }

        [TestMethod]
        public void GivenNullModel_WhenCopyFrom_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.CopyFrom(null));
        }
    }
}
