using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Data;
using SSD.Domain;
using SSD.Security;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSD.Repository
{
    [TestClass]
    public class StudentRepositoryTest
    {
        private EducationDataContext Context { get; set; }
        private StudentRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Context = new EducationDataContext();
            Target = new StudentRepository(Context);
        }

        [TestCleanup]
        public void CleanupTest()
        {
            if (Context != null)
            {
                Context.Dispose();
            }
        }

        [TestMethod]
        public void GivenAStudentQueryable_WhenIncludeClassesAndTeachers_ThenGetStudentsWithClassesWithATeacher()
        {
            List<Student> items = Target.Items.ToList();
            Context = new EducationDataContext();
            Target = new StudentRepository(Context);

            List<Student> expanded = Target.Items.Include("Classes.Teacher").ToList();

            Assert.IsTrue(items.Any(s => s.Classes.Count == 0));
            Assert.IsTrue(expanded.Any(s => s.Classes.Count > 0));
            Assert.IsTrue(expanded.Any(s => s.Classes.All(c => c.Teacher != null)));
        }

        [TestMethod]
        public void GivenUserIsAdministrator_WhenGetAllowedList_ThenListIsReturned()
        {
            User currentUser = Context.Users.Where(u => u.Id == 2).Single();
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(currentUser);

            var students = Target.GetAllowedList(user);

            Assert.IsNotNull(students);
        }

        [TestMethod]
        public void WhenResetApprovals_ThenNewContextCanFindNoStudentsWithApprovedProviders()
        {
            Target.ResetApprovals();

            Context.SaveChanges();
            using (EducationDataContext assertContext = new EducationDataContext())
            {
                Assert.IsFalse(assertContext.Students.Where(s => s.ApprovedProviders.Count() > 0).Any());
            }
        }

        [TestMethod]
        public void GivenSingleSchoolId_WhenResetApprovals_ThenNewContextCanFindNoStudentsWithApprovedProviders()
        {
            Target.ResetApprovals(new[] { 1 });

            Context.SaveChanges();
            using (EducationDataContext assertContext = new EducationDataContext())
            {
                Assert.IsFalse(assertContext.Students.Where(s => s.SchoolId == 1 && s.ApprovedProviders.Count() > 0).Any());
            }
        }

        [TestMethod]
        public void GivenMultipleSchoolIds_WhenResetApprovals_ThenNewContextCanFindNoStudentsWithApprovedProviders()
        {
            Target.ResetApprovals(new[] { 1, 3 });

            Context.SaveChanges();
            using (EducationDataContext assertContext = new EducationDataContext())
            {
                Assert.IsFalse(assertContext.Students.Where(s => s.SchoolId == 1 && s.SchoolId == 3 && s.ApprovedProviders.Count() > 0).Any());
            }
        }
    }
}
