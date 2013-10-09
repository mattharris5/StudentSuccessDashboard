using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.ViewModels
{
    [TestClass]
    public class ServiceTypeModelTest
    {
        private ServiceTypeModel Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new ServiceTypeModel();
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
