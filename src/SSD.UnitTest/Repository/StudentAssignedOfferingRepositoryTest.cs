using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class StudentAssignedOfferingRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<StudentAssignedOffering> MockDbSet { get; set; }
        private StudentAssignedOfferingRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<StudentAssignedOffering>>();
            MockContext.Expect(m => m.StudentAssignedOfferings).Return(MockDbSet);
            Target = new StudentAssignedOfferingRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new StudentAssignedOfferingRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsStudentAssignedOfferings_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenAStudentAssignedOffering_WhenAdd_ThenAddToContext()
        {
            var expected = new StudentAssignedOffering { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenAStudentAssignedOffering_WhenUpdated_ThenContextSetsModified()
        {
            var expected = new StudentAssignedOffering { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenNullAssignedOffering_WhenRemove_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Remove(null));
        }

        [TestMethod]
        public void GivenAnAssignedOffering_WhenRemove_ThenThrowNotSupportedException()
        {
            var item = new StudentAssignedOffering { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Remove(item));
        }
    }
}
