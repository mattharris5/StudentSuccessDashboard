using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class EulaAcceptanceRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<EulaAcceptance> MockDbSet { get; set; }
        private EulaAcceptanceRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<EulaAcceptance>>();
            MockContext.Expect(m => m.EulaAcceptances).Return(MockDbSet);
            Target = new EulaAcceptanceRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new EulaAcceptanceRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsEulaAcceptances_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenAnEulaAcceptance_WhenAdd_ThenAddToContext()
        {
            var expected = new EulaAcceptance { Id = 1, EulaAgreement = new EulaAgreement() };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenAnEulaAcceptance_WhenUpdate_ThenThrowException()
        {
            var acceptance = new EulaAcceptance { Id = 1, EulaAgreement = new EulaAgreement() };

            Target.ExpectException<NotSupportedException>(() => Target.Update(acceptance));
        }

        [TestMethod]
        public void GivenAnEulaAcceptance_WhenRemove_ThenThrowException()
        {
            var acceptance = new EulaAcceptance { Id = 1, EulaAgreement = new EulaAgreement() };

            Target.ExpectException<NotSupportedException>(() => Target.Remove(acceptance));
        }
    }
}
