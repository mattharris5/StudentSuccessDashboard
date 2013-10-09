using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class SubjectRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<Subject> MockDbSet { get; set; }
        private SubjectRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<Subject>>();
            MockContext.Expect(m => m.Subjects).Return(MockDbSet);
            Target = new SubjectRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new SubjectRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsSubjects_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenASubject_WhenAdd_ThenAddToContext()
        {
            var expected = new Subject { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Add(expected));
        }

        [TestMethod]
        public void GivenASubject_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new Subject { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Update(expected));
        }

        [TestMethod]
        public void GivenASubject_WhenRemove_ThenRemoveFromContext()
        {
            var item = new Subject { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Remove(item));
        }
    }
}
