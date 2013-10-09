using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class UserRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<User> MockDbSet { get; set; }
        private UserRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<User>>();
            MockContext.Expect(m => m.Users).Return(MockDbSet);
            Target = new UserRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new UserRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsUsers_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenAUser_WhenAdd_ThenAddToContext()
        {   
            var expected = new User { Id = 1 };
            
            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenAUser_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new User { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenAUser_WhenRemove_ThenThrowNotImplementedException()
        {
            var item = new User { Id = 1 };

            Target.ExpectException<NotImplementedException>(() => Target.Remove(item));
        }
    }
}
