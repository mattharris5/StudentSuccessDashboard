using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class UserAccessChangeEventRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<UserAccessChangeEvent> MockDbSet { get; set; }
        private UserAccessChangeEventRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<UserAccessChangeEvent>>();
            MockContext.Expect(m => m.UserAccessChangeEvents).Return(MockDbSet);
            Target = new UserAccessChangeEventRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new UserRoleRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsUserRoles_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenAUserAccessChangeEvent_WhenAdd_ThenAddToContext()
        {
            var expected = new UserAccessChangeEvent { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenAUserAccessChangeEvent_WhenUpdate_ThenThrowException()
        {
            var item = new UserAccessChangeEvent { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Update(item));
        }

        [TestMethod]
        public void GivenAUserAccessChangeEvent_WhenRemove_ThenThrowException()
        {
            var item = new UserAccessChangeEvent { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Remove(item));
        }
    }
}
