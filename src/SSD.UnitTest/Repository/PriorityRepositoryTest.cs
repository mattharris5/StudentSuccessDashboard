using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class PriorityRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<Priority> MockDbSet { get; set; }
        private PriorityRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<Priority>>();
            MockContext.Expect(m => m.Priorities).Return(MockDbSet);
            Target = new PriorityRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new PriorityRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsPriorities_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenAPriority_WhenAdd_ThenAddToContext()
        {
            var expected = new Priority { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Add(expected));
        }

        [TestMethod]
        public void GivenAPriority_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new Priority { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Update(expected));
        }

        [TestMethod]
        public void GivenAPriority_WhenRemove_ThenRemoveFromContext()
        {
            var item = new Priority { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Remove(item));
        }
    }
}
