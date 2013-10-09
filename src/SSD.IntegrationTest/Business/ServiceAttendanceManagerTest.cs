using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Data.Entity;
using System.Linq;

namespace SSD.Business
{
    [TestClass]
    public class ServiceAttendanceManagerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private ServiceAttendanceManager Target { get; set; }
        private EducationSecurityPrincipal User { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            Target = new ServiceAttendanceManager(repositoryContainer, new DataTableBinder());
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
        public void GivenSort_WhenGenerateDataTableResultViewModel_ThenSucceed()
        {
            DataTableRequestModel model = new DataTableRequestModel();
            model.iDisplayLength = 10;
            var request = MockHttpContextFactory.CreateRequest();
            request.Expect(r => r["id"]).Return("1");
            request.Expect(r => r["iSortCol_0"]).Return("0");
            request.Expect(r => r["sSortDir_0"]).Return("asc");
            ServiceAttendanceClientDataTable dataTable = new ServiceAttendanceClientDataTable(request);

            var actual = Target.GenerateDataTableResultViewModel(model, dataTable);
        }

        [TestMethod]
        public void GivenSort_AndOneServiceAttendanceIsVeryLarge_WhenGenerateDataTableResultViewModel_ThenSucceed()
        {
            DataTableRequestModel model = new DataTableRequestModel();
            model.iDisplayLength = 10;
            var request = MockHttpContextFactory.CreateRequest();
            request.Expect(r => r["id"]).Return("1");
            request.Expect(r => r["iSortCol_0"]).Return("0");
            request.Expect(r => r["sSortDir_0"]).Return("asc");
            ServiceAttendanceClientDataTable dataTable = new ServiceAttendanceClientDataTable(request);
            ServiceAttendance attendance = new ServiceAttendance
            {
                DateAttended = DateTime.Now.AddYears(100),
                StudentAssignedOffering = EducationContext.StudentAssignedOfferings.Where(s => s.Id == 1).Single(),
                Subject = EducationContext.Subjects.First(),
                CreatingUser = User.Identity.User
            };
            EducationContext.ServiceAttendances.Add(attendance);
            EducationContext.SaveChanges();

            var actual = Target.GenerateDataTableResultViewModel(model, dataTable);
        }

        [TestMethod]
        public void GivenModifiedServiceAttendanceId_WhenGenerateEditViewModel_ThenViewModelContainsAuditData()
        {
            ServiceAttendance toEdit = EducationContext.ServiceAttendances.Single(a => a.LastModifyingUser != null);

            ServiceAttendanceModel actual = Target.GenerateEditViewModel(User, toEdit.Id);

            Assert.IsNotNull(actual.Audit.CreatedBy);
            Assert.AreNotEqual(DateTime.MinValue, actual.Audit.CreateTime);
            Assert.IsNotNull(actual.Audit.LastModifiedBy);
            Assert.IsTrue(actual.Audit.LastModifyTime.HasValue);
        }
    }
}
