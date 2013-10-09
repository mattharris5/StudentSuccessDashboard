using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;

namespace SSD.ViewModels
{
    [TestClass]
    public class EulaModelTest
    {
        private EulaModel Target { get; set; }
        private TestData TestData { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new EulaModel();
            TestData = new TestData();
        }

        [TestMethod]
        public void GivenNullModel_WhenCopyTo_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.CopyTo(null));
        }

        [TestMethod]
        public void GivenValidModel_WhenCopyTo_ThenAllFieldsAreCopied()
        {
            var actual = new EulaAgreement();
            Target.Id = TestData.Eulas[0].Id;
            Target.EulaText = TestData.Eulas[0].EulaText;

            Target.CopyTo(actual);

            Assert.AreEqual(Target.EulaText, actual.EulaText);
        }

        [TestMethod]
        public void GivenNullModel_WhenCopyFrom_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.CopyFrom(null));
        }

        [TestMethod]
        public void GivenValidModel_WhenCopyFrom_ThenAllFieldsCopied()
        {
            var expected = TestData.Eulas[0];

            Target.CopyFrom(expected);

            Assert.AreEqual(expected.Id, Target.Id);
            Assert.AreEqual(expected.EulaText, Target.EulaText);
        }

        [TestMethod]
        public void GivenModelHasAuditData_WhenCopyFrom_ThenModelStateSet()
        {
            var expected = TestData.Eulas[0];

            Target.CopyFrom(expected);

            AuditModel actualState = Target.Audit;
            Assert.AreEqual(expected.CreateTime, actualState.CreateTime);
            Assert.AreEqual(expected.CreatingUser.DisplayName, actualState.CreatedBy);
        }
    }
}
