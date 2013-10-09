using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSD.Repository
{
    [TestClass]
    public class StudentRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<Student> MockDbSet { get; set; }
        private StudentRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<Student>>();
            MockContext.Expect(m => m.Students).Return(MockDbSet);
            Target = new StudentRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new StudentRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsStudents_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenNullUser_WhenGetAllowedList_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.GetAllowedList(null));
        }

        [TestMethod]
        public void GivenNullSchoolIdList_WhenResetApprovals_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.ResetApprovals(null));
        }

        [TestMethod]
        public void GivenAStudent_WhenAdd_ThenThrowNotImplementedException()
        {
            var expected = new Student { Id = 1 };

            Target.ExpectException<NotImplementedException>(() => Target.Add(expected));
        }

        [TestMethod]
        public void GivenAStudent_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new Student { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenAStudent_WhenRemove_ThenThrowNotImplementedException()
        {
            Target.ExpectException<NotImplementedException>(() => Target.Remove(new Student { Id = 1 }));
        }

        [TestMethod]
        public void GivenAnUnassociatedStudentAndProvider_WhenAddLink_ThenTheyAreAssociated()
        {
            Student student = new Student();
            Provider provider = new Provider();

            Target.AddLink(student, provider);

            CollectionAssert.Contains(student.ApprovedProviders.ToList(), provider);
            CollectionAssert.Contains(provider.ApprovingStudents.ToList(), student);
        }

        [TestMethod]
        public void GivenNullStudent_WhenAddLink_ThenThrowException()
        {
            Provider provider = new Provider();

            Target.ExpectException<ArgumentNullException>(() => Target.AddLink(null, provider));
        }

        [TestMethod]
        public void GivenNullProvider_WhenAddLink_ThenThrowException()
        {
            Student student = new Student();

            Target.ExpectException<ArgumentNullException>(() => Target.AddLink(student, null));
        }

        [TestMethod]
        public void GivenAnAssociatedStudentAndProvider_WhenDeleteLink_ThenTheyAreNoLongerAssociated()
        {
            Student student = new Student();
            Provider provider = new Provider { ApprovingStudents = new List<Student> { student } };
            student.ApprovedProviders.Add(provider);

            Target.DeleteLink(student, provider);

            CollectionAssert.DoesNotContain(student.ApprovedProviders.ToList(), provider);
            CollectionAssert.DoesNotContain(provider.ApprovingStudents.ToList(), student);
        }

        [TestMethod]
        public void GivenNullStudent_WhenDeleteLink_ThenThrowException()
        {
            Provider provider = new Provider();

            Target.ExpectException<ArgumentNullException>(() => Target.DeleteLink(null, provider));
        }

        [TestMethod]
        public void GivenNullProvider_WhenDeleteLink_ThenThrowException()
        {
            Student student = new Student();

            Target.ExpectException<ArgumentNullException>(() => Target.DeleteLink(student, null));
        }
    }
}
