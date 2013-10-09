using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class ServiceRequestFulfillmentRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<ServiceRequestFulfillment> MockDbSet { get; set; }
        private ServiceRequestFulfillmentRepository Target { get; set; }
        private TestData TestData { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<ServiceRequestFulfillment>>();
            MockContext.Expect(m => m.ServiceRequestFulfillments).Return(MockDbSet);
            Target = new ServiceRequestFulfillmentRepository(MockContext);
            TestData = new TestData();
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceRequestFulfillmentRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsServiceRequestFulfillment_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenAServiceRequestFulfillment_WhenAdd_ThenAddToContext()
        {
            var expected = TestData.ServiceRequestFulfillments[0];

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenAServiceRequestFulfillment_WhenUpdate_ThenContextSetsModified()
        {
            var expected = TestData.ServiceRequestFulfillments[0];

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenAServiceRequestFulfillment_WhenRemove_ThenContextCallsRemove()
        {
            var expected = TestData.ServiceRequestFulfillments[0];

            Target.Remove(expected);

            MockDbSet.AssertWasCalled(m => m.Remove(expected));
        }
    }
}
