using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class FulfillmentStatusRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<FulfillmentStatus> MockDbSet { get; set; }
        private FulfillmentStatusRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<FulfillmentStatus>>();
            MockContext.Expect(m => m.FulfillmentStatuses).Return(MockDbSet);
            Target = new FulfillmentStatusRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new FulfillmentStatusRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsFulfillmentStatus_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenAFulfillmentStatus_WhenAdd_ThenThrowException()
        {
            var expected = new FulfillmentStatus { Id = 1, Name = "New" };

            Target.ExpectException<NotSupportedException>(() => Target.Add(expected));
        }

        [TestMethod]
        public void GivenAFulfillmentStatus_ThenUpdate_ThenThrowException()
        {
            var expected = new FulfillmentStatus { Id = 1, Name = "New" };

            Target.ExpectException<NotSupportedException>(() => Target.Update(expected));
        }

        [TestMethod]
        public void GivenAFulfillmentStatus_WhenRemove_ThenThrowException()
        {
            var expected = new FulfillmentStatus { Id = 1, Name = "New" };

            Target.ExpectException<NotSupportedException>(() => Target.Remove(expected));
        }
    }
}
