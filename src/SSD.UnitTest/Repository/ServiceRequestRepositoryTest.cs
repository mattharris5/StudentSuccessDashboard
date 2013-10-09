using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class ServiceRequestRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<ServiceRequest> MockDbSet { get; set; }
        private ServiceRequestRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<ServiceRequest>>();
            MockContext.Expect(m => m.ServiceRequests).Return(MockDbSet);
            Target = new ServiceRequestRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceRequestRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsServiceRequests_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenServiceRequest_WhenAdd_ThenAddToContext()
        {
            var expected = new ServiceRequest { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenAServiceRequests_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new ServiceRequest { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenAServiceRequest_WhenRemove_ThenRemoveFromContext()
        {
            var item = new ServiceRequest { Id = 1 };

            Target.Remove(item);

            MockDbSet.AssertWasCalled(m => m.Remove(item));
        }
    }
}
