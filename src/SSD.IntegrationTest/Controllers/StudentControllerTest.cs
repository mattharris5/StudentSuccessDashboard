using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.Data;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;

namespace SSD.Controllers
{
    [TestClass]
    public class StudentControllerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private StudentController Target { get; set; }
        private EducationSecurityPrincipal User { get; set; }
        private HttpContextBase MockHttpContext { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            SchoolDistrictManager logicManager = new SchoolDistrictManager(repositoryContainer, new DataTableBinder(), new UserAuditor());
            Target = new StudentController(logicManager);
            User userEntity = EducationContext.Users.Include("UserRoles.Role").Include("UserRoles.Schools").Include("UserRoles.Providers").Single(s => s.UserKey == "Bob");
            User = new EducationSecurityPrincipal(userEntity);
            MockHttpContext = MockHttpContextFactory.Create();
            MockHttpContext.Expect(m => m.User).Return(User);
            ControllerContext context = new ControllerContext(MockHttpContext, new RouteData(), Target);
            Target.ControllerContext = context;
        }

        [TestCleanup]
        public void CleanupTest()
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
        public void GivenStudentWithClassesAndTeachers_WhenGettingStudentDetails_ThenViewModelContainsClassesWithTeachers()
        {
            ViewResult result = Target.Details(1) as ViewResult;
            StudentDetailModel actual = result.AssertGetViewModel<StudentDetailModel>();

            Assert.IsTrue(actual.Classes.Any());
            Assert.IsTrue(actual.Classes.All(c => c.Teacher != null));
        }

        [TestMethod]
        public void GivenStudentWithSchool_WhenGettingStudentDetails_ThenViewModelContainsSchoolName()
        {
            ViewResult result = Target.Details(1) as ViewResult;
            StudentDetailModel actual = result.AssertGetViewModel<StudentDetailModel>();

            Assert.IsNotNull(actual.SchoolName);
        }

        [TestMethod]
        public void GivenStudentsWithSchools_WhenGettingStudentFinder_ThenViewModelContainsSchoolFilterList_AndViewModelContainsGradeFilterList()
        {
            ViewResult result = Target.Index() as ViewResult;
            StudentListOptionsModel actual = result.AssertGetViewModel<StudentListOptionsModel>();

            Assert.IsTrue(actual.SchoolFilterList.Any());
            Assert.IsTrue(actual.GradeFilterList.Any());
        }

        [TestMethod]
        public void GivenStudentsWithSchools_WhenGettingStudentFinder_ThenViewModelContainsServiceRequestFilterLists()
        {
            ViewResult result = Target.Index() as ViewResult;
            StudentListOptionsModel actual = result.AssertGetViewModel<StudentListOptionsModel>();

            Assert.IsTrue(actual.PriorityFilterList.Any());
            Assert.IsTrue(actual.RequestStatusFilterList.Any());
            Assert.IsTrue(actual.ServiceTypeFilterList.Any());
            Assert.IsTrue(actual.SubjectFilterList.Any());
        }

        [TestMethod]
        public void GivenStudentsWithSchools_WhenGettingDataTableAjaxHandler_ThenDataTableStateContainsColumns()
        {
            MockHttpContext.Request.Expect(m => m["iSortCol_0"]).Return("0");
            MockHttpContext.Request.Expect(m => m["sSortDir_0"]).Return("asc");
            DataTableRequestModel request = new DataTableRequestModel { iDisplayLength = 10 };

            JsonResult result = Target.DataTableAjaxHandler(request) as JsonResult;

            DataTableResultModel actual = result.AssertGetData<DataTableResultModel>();
            Assert.IsTrue(actual.aaData.Any());
        }
    }
}
