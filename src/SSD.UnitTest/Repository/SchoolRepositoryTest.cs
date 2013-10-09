using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class SchoolRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<School> MockDbSet { get; set; }
        private SchoolRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<School>>();
            MockContext.Expect(m => m.Schools).Return(MockDbSet);
            Target = new SchoolRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new SchoolRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsSchools_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenASchool_WhenAdd_ThenAddToContext()
        {
            var expected = new School { Id = 1 };
            
            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenASchool_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new School { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenASchool_WhenDelete_ThenRemoveFromContext()
        {
            var item = new School { Id = 1 };

            Target.Remove(item);

            MockDbSet.AssertWasCalled(m => m.Remove(item));
        }
    }
}
