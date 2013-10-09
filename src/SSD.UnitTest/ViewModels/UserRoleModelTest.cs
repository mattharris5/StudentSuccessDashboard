using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.ViewModels
{
    [TestClass]
    public class UserRoleModelTest
    {
        private UserRoleModel Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new UserRoleModel();
        }

        [TestMethod]
        public void GivenNullModel_WhenCopyFrom_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.CopyFrom(null));
        }
    }
}
