using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SSD.Business
{
    [TestClass]
    public class SchoolDistrictManagerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private SchoolDistrictManager Target { get; set; }
        private EducationSecurityPrincipal User { get; set; }
        private HttpContextBase MockContext { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            Target = new SchoolDistrictManager(repositoryContainer, new DataTableBinder(), new UserAuditor());
            User = new EducationSecurityPrincipal(EducationContext.Users.Include("UserRoles.Role").Single(u => u.UserKey == "Bob"));
            MockContext = MockHttpContextFactory.Create();
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
        public void WhenGenerateApprovalDataTableResultViewModel_ThenSucceed()
        {
            DataTableRequestModel model = new DataTableRequestModel { iDisplayLength = 10 };
            StudentApprovalClientDataTable dataTable = new StudentApprovalClientDataTable(MockContext.Request);

            Target.GenerateApprovalDataTableResultViewModel(model, dataTable);
        }

        [TestMethod]
        public void GivenSortOnId_WhenGenerateApprovalDataTableResultViewModel_ThenViewModelContainsData()
        {
            DataTableRequestModel model = new DataTableRequestModel { iDisplayLength = 10 };
            MockContext.Request.Expect(m => m["iSortCol_0"]).Return("0");
            MockContext.Request.Expect(m => m["sSortDir_0"]).Return("asc");
            StudentApprovalClientDataTable dataTable = new StudentApprovalClientDataTable(MockContext.Request);

            var actual = Target.GenerateApprovalDataTableResultViewModel(model, dataTable);

            Assert.AreEqual(10, actual.aaData.Count());
        }

        [TestMethod]
        public void GivenRequestStatusFilter_WhenGenerateDataTableResultViewModel_ThenRecordCountMatchesStudentsThatMatchStatus()
        {
            int expected = 3;
            MockContext.Request.Expect(m => m["requestStatuses"]).Return(Statuses.Open);
            DataTableRequestModel requestModel = new DataTableRequestModel { iDisplayLength = 10 };
            IEnumerable<Property> studentProperties = EducationContext.Properties.Where(p => p.EntityName == "Student");
            StudentClientDataTable dataTable = new StudentClientDataTable(MockContext.Request, User, studentProperties);

            var actual = Target.GenerateDataTableResultViewModel(requestModel, dataTable);

            Assert.AreEqual(expected, actual.iTotalDisplayRecords);
        }

        [TestMethod]
        public void GivenValidStudentId_WhenGenerateStudentDetailViewModel_ThenViewModelContainsClassData()
        {
            StudentDetailModel actual = Target.GenerateStudentDetailViewModel(User, 1);

            Assert.AreNotEqual(0, actual.Classes.Select(c => c.Teacher).Count());
        }

        [TestMethod]
        public void GivenValidStudentId_WhenGenerateStudentDetailViewModel_ThenViewModelContainsFulfillmentDetailsData()
        {
            StudentDetailModel actual = Target.GenerateStudentDetailViewModel(User, 1);

            Assert.AreNotEqual(0, actual.ServiceRequests.First().FulfillmentDetails.Count());
        }

        [TestMethod]
        public void GivenValidStudentId_AndDifferentUserCreatedOneOfTheServiceRequestFulfillments_WhenGenerateStudentDetailViewModel_ThenViewModelContainsPerformedByUserForEachServiceRequestFulfillmentDetail()
        {
            StudentDetailModel actual = Target.GenerateStudentDetailViewModel(User, 1);

            var fulfillmentDetails = actual.ServiceRequests.Single().FulfillmentDetails;
            var performedByUsers = actual.ServiceRequests.Single().FulfillmentDetails.Select(m => m.CreatingUser).Where(u => u != null);
            Assert.AreEqual(fulfillmentDetails.Count, performedByUsers.Count());
        }

        [TestMethod]
        public void GivenValidStudentId_WhenGenerateStudentDetailViewModel_ThenViewModelContainsRequestData()
        {
            StudentDetailModel actual = Target.GenerateStudentDetailViewModel(User, 1);

            Assert.AreNotEqual(0, actual.ServiceRequests.Select(r => r.CreatingUser).Count());
            Assert.AreNotEqual(0, actual.ServiceRequests.Select(r => r.ServiceType).Count());
            Assert.AreNotEqual(0, actual.ServiceRequests.Select(r => r.Subject).Count());
        }

        [TestMethod]
        public void GivenValidStudentId_WhenGenerateStudentDetailViewModel_ThenViewModelContainsServiceData()
        {
            AssemblySetup.ForceDeleteEducationDatabase("SSD");
            InitializeTest();
            StudentDetailModel actual = Target.GenerateStudentDetailViewModel(User, 1);

            Assert.AreNotEqual(0, actual.StudentAssignedOfferings.Select(s => s.ServiceOffering.Provider).Count());
            Assert.AreNotEqual(0, actual.StudentAssignedOfferings.Select(s => s.ServiceOffering.ServiceType).Count());
            Assert.AreNotEqual(0, actual.StudentAssignedOfferings.Select(s => s.CreatingUser).Count());
        }

        [TestMethod]
        public void GivenValidStudentId_WhenGenerateAddStudentApprovalViewModel_ThenProviderListHasCount()
        {
            var actual = Target.GenerateAddStudentApprovalViewModel(3);

            Assert.AreNotEqual(0, actual.Providers.Count());
        }

        [TestMethod]
        public void WhenGenerateListOptionsViewModel_ThenGradeFilterListPopulated()
        {
            var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            var actual = Target.GenerateListOptionsViewModel(User);

            CollectionAssert.AreEquivalent(expected, actual.GradeFilterList.ToList());
        }

        [TestMethod]
        public void WhenGenerateListOptionsViewModel_ThenSchoolFilterListPopulated()
        {
            var expected = new[] { "Local High School", "Community Middle School", "Springfield Elementary School" };

            var actual = Target.GenerateListOptionsViewModel(User);

            CollectionAssert.AreEquivalent(expected, actual.SchoolFilterList.ToList());
        }

        [TestMethod]
        public void WhenGenerateListOptionsViewModel_ThenPriorityFilterListPopulated()
        {
            var expected = new[] { "None", "Low", "Medium", "High", "Super, Duper High" };

            var actual = Target.GenerateListOptionsViewModel(User);

            CollectionAssert.AreEquivalent(expected, actual.PriorityFilterList.ToList());
        }

        [TestMethod]
        public void WhenGenerateListOptionsViewModel_ThenServiceTypeFilterListPopulated()
        {
            var expected = new[] { "Provide College Access", "Mentoring", "Test service typ,e" };

            var actual = Target.GenerateListOptionsViewModel(User);

            CollectionAssert.AreEquivalent(expected, actual.ServiceTypeFilterList.ToList());
        }

        [TestMethod]
        public void WhenGenerateListOptionsViewModel_ThenSubjectFilterListPopulated()
        {
            var expected = new[] { "None", "Math", "Reading", "Science", "Social Studies", "Hard, Hard Math" };

            var actual = Target.GenerateListOptionsViewModel(User);

            CollectionAssert.AreEquivalent(expected, actual.SubjectFilterList.ToList());
        }

        [TestMethod]
        public void WhenGenerateApprovalViewModel_ThenSchoolFilterListPopulated()
        {
            var expected = new[] { "Local High School", "Community Middle School", "Springfield Elementary School" };

            var actual = Target.GenerateApprovalListOptionsViewModel();

            CollectionAssert.AreEquivalent(expected, actual.SchoolFilterList.ToList());
        }

        [TestMethod]
        public void WhenGenerateApprovalViewModel_ThenProviderFilterListPopulated()
        {
            var expected = new[] { "YMCA", "Big Brothers, Big Sisters", "Boys and Girls Club", "Joe's World-class Tutoring Services and Eatery!" };

            var actual = Target.GenerateApprovalListOptionsViewModel();

            CollectionAssert.AreEquivalent(expected, actual.ProviderFilterList.ToList());
        }

        [TestMethod]
        public void WhenGenerateAddStudentApprovalViewModel_ThenViewModelContainsDistinctListOfSchools()
        {
            var actual = Target.GenerateRemoveProvidersBySchoolViewModel();

            CollectionAssert.AllItemsAreUnique(actual.Schools.Items.Cast<School>().ToList());
        }
    }
}
