using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class EulaAgreementRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<EulaAgreement> MockDbSet { get; set; }
        private EulaAgreementRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<EulaAgreement>>();
            MockContext.Expect(m => m.EulaAgreements).Return(MockDbSet);
            Target = new EulaAgreementRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new EulaAgreementRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsEulaAgreements_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenAnEulaAgreement_WhenAdd_ThenAddToContext()
        {
            var expected = new EulaAgreement { Id = 1, EulaText = "blah" };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenAnEulaAgreement_WhenUpdate_ThenThrowException()
        {
            var expected = new EulaAgreement { Id = 1, EulaText = "blah" };

            Target.ExpectException<NotSupportedException>(() => Target.Update(expected));
        }

        [TestMethod]
        public void GivenAnEulaAgreement_WhenRemove_ThenThrowException()
        {
            var expected = new EulaAgreement { Id = 1, EulaText = "blah" };

            Target.ExpectException<NotSupportedException>(() => Target.Remove(expected));
        }
    }
}
