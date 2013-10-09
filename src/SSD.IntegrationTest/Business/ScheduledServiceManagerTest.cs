using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Data;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;

namespace SSD.Business
{
    [TestClass]
    public class ScheduledServiceManagerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private ScheduledServiceManager Target { get; set; }
        private EducationSecurityPrincipal User { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            Target = new ScheduledServiceManager(repositoryContainer);
            User = new EducationSecurityPrincipal(new UserRepository(EducationContext).Items.Where(s => s.UserKey == "Bob").Include("UserRoles.Role").Single());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (Container != null)
            {
                Container.Dispose();
            }
            if (EducationContext != null)
            {
                EducationContext.Dispose();
            }
        }

        [TestMethod]
        public void GivenValidViewModel_AndUserIsNotAdministrator_AndUserIsNotCreator_WhenDelete_ThenThrowEntityAccessUnauthorizedException()
        {
            User nonAdminUserEntity = EducationContext.Users.Where(u => u.UserKey == "Fred").Include("UserRoles.Role").Single();
            EducationSecurityPrincipal nonAdminUser = new EducationSecurityPrincipal(nonAdminUserEntity);
            EducationContext.StudentAssignedOfferings.Single(a => a.Id == 4).IsActive = true;

            Target.ExpectException<EntityAccessUnauthorizedException>(() => Target.Delete(nonAdminUser, 4));
        }

        [TestMethod]
        public void GivenValidViewModel_WhenDelete_ThenServiceAttendanceHistoryKeptIntact()
        {
            ServiceAttendance expected = EducationContext.ServiceAttendances.Where(s => s.Id == 1).FirstOrDefault();

            Target.Delete(User, 1);

            Assert.IsTrue(EducationContext.ServiceAttendances.Any(s => s.Id == expected.Id));
        }

        [TestMethod]
        public void WhenDelete_ThenAssignedOfferingSetInactive()
        {
            Target.Delete(User, 3);

            using (EducationDataContext verificationContext = new EducationDataContext())
            {
                StudentAssignedOffering actual = verificationContext.StudentAssignedOfferings.Find(3);
                Assert.IsFalse(actual.IsActive);
            }
        }

        [TestMethod]
        public void WhenGenerateScheduleOfferingViewModel_ThenTypeFilterListPopulated()
        {
            var expected = new[] { "Provide College Access", "Mentoring", "Test service typ,e" };

            ScheduleServiceOfferingListOptionsModel actual = Target.GenerateScheduleOfferingViewModel(User, new[] { 3 });

            CollectionAssert.AreEquivalent(expected, actual.TypeFilterList.ToList());
        }

        [TestMethod]
        public void WhenGenerateScheduleOfferingViewModel_ThenCategoryFilterListPopulated()
        {
            var expected = new[] { "Basic Needs", "Consumer Services", "Criminal Justice and Legal Services", "Education", "Environmental Quality", "Health Care", "Income Support and Employment", "Individual and Family Life", "Mental Health Care and Counseling", "Organizational/Community Services", "Support Groups", "Target Populations", "Test Category," };

            ScheduleServiceOfferingListOptionsModel actual = Target.GenerateScheduleOfferingViewModel(User, new[] { 3 });

            CollectionAssert.AreEquivalent(expected, actual.CategoryFilterList.ToList());
        }

        [TestMethod]
        public void GivenStudentAssignedOfferingIsActive_WhenGenerateEditViewModel_ThenAuditDataPopulated()
        {
            int entityId;
            using (EducationDataContext setupContext = new EducationDataContext())
            {
                StudentAssignedOffering entity = setupContext.StudentAssignedOfferings.Single(a => a.LastModifyingUser != null);
                entity.IsActive = true;
                setupContext.SaveChanges();
                entityId = entity.Id;
            }

            StudentServiceOfferingScheduleModel actual = Target.GenerateEditViewModel(User, entityId);

            Assert.IsNotNull(actual.Audit.CreatedBy);
            Assert.AreNotEqual(DateTime.MinValue, actual.Audit.CreateTime);
            Assert.IsNotNull(actual.Audit.LastModifiedBy);
            Assert.IsTrue(actual.Audit.LastModifyTime.HasValue);
        }
    }
}
