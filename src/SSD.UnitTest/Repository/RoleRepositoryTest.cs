using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class RoleRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<Role> MockDbSet { get; set; }
        private RoleRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<Role>>();
            MockContext.Expect(m => m.Roles).Return(MockDbSet);
            Target = new RoleRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new RoleRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsRoles_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenARole_WhenAdd_ThenAddToContext()
        {
            var expected = new Role { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Add(expected));
        }

        [TestMethod]
        public void GivenARole_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new Role { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Update(expected));
        }

        [TestMethod]
        public void GivenARole_WhenDelete_ThenRemoveFromContext()
        {
            var item = new Role { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Remove(item));
        }
    }
}
