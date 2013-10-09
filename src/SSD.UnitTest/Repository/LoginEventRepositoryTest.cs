using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class LoginEventRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<LoginEvent> MockDbSet { get; set; }
        private LoginEventRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<LoginEvent>>();
            MockContext.Expect(m => m.LoginEvents).Return(MockDbSet);
            Target = new LoginEventRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new LoginEventRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsItems_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenItem_WhenAdd_ThenAddToContext()
        {
            var expected = new LoginEvent { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenItem_WhenUpdate_ThenThrowException()
        {
            var item = new LoginEvent { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Update(item));
        }

        [TestMethod]
        public void GivenItem_WhenRemove_ThenThrowException()
        {
            var item = new LoginEvent { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Remove(item));
        }
    }
}
