using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class PrivateHealthDataViewEventRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<PrivateHealthDataViewEvent> MockDbSet { get; set; }
        private IPrivateHealthDataViewEventRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<PrivateHealthDataViewEvent>>();
            MockContext.Expect(m => m.PrivateHealthDataViewEvents).Return(MockDbSet);
            Target = new PrivateHealthDataViewEventRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new PrivateHealthDataViewEventRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsItems_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenPrivateHealthDataViewEvent_WhenAdd_ThenAddToContext()
        {
            var expected = new PrivateHealthDataViewEvent { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenPrivateHealthDataViewEvent_WhenUpdate_ThenThrowException()
        {
            var item = new PrivateHealthDataViewEvent { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Update(item));
        }

        [TestMethod]
        public void GivenPrivateHealthDataViewEvent_WhenRemove_ThenThrowException()
        {
            var item = new PrivateHealthDataViewEvent { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Remove(item));
        }
    }
}
