using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SSD.Business
{
    [TestClass]
    public class ServiceTypeManagerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private ServiceTypeManager Target { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            Target = new ServiceTypeManager(repositoryContainer, new DataTableBinder());
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
        public void WhenGenerateDataTableResultViewModel_ThenSucceed()
        {
            DataTableRequestModel model = new DataTableRequestModel { iDisplayLength = 10 };
            User userEntity = EducationContext.Users.First(u => u.UserRoles.Select(ur => ur.Role).Any(r => r.Name == SecurityRoles.DataAdmin));
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(userEntity);
            ServiceTypeClientDataTable dataTable = new ServiceTypeClientDataTable(MockHttpContextFactory.CreateRequest(), user);

            Target.GenerateDataTableResultViewModel(model, dataTable);
        }

        [TestMethod]
        public void GivenSortOnFirstColumn_WhenGenerateDataTableResultViewModel_ThenSucceed()
        {
            DataTableRequestModel model = new DataTableRequestModel { iDisplayLength = 10 };
            User userEntity = EducationContext.Users.First(u => u.UserRoles.Select(ur => ur.Role).Any(r => r.Name == SecurityRoles.DataAdmin));
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(userEntity);
            HttpRequestBase mockRequest = MockHttpContextFactory.CreateRequest();
            mockRequest.Expect(m => m["iSortCol_0"]).Return("0");
            mockRequest.Expect(m => m["sSortDir_0"]).Return("asc");
            ServiceTypeClientDataTable dataTable = new ServiceTypeClientDataTable(mockRequest, user);

            Target.GenerateDataTableResultViewModel(model, dataTable);
        }

        [TestMethod]
        public void WhenGenerateListOptionsViewModel_ThenCategoryFilterListPopulated()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new UserRepository(EducationContext).Items.Where(s => s.UserKey == "Bob").Include("UserRoles.Role").Single());
            var expected = new[] { "Basic Needs", "Consumer Services", "Criminal Justice and Legal Services", "Education", "Environmental Quality", "Health Care", "Income Support and Employment", "Individual and Family Life", "Mental Health Care and Counseling", "Organizational/Community Services", "Support Groups", "Target Populations", "Test Category," };

            ServiceTypeListOptionsModel actual = Target.GenerateListOptionsViewModel(user);

            CollectionAssert.AreEquivalent(expected, actual.CategoryFilterList.ToList());
        }

        [TestMethod]
        public void GivenServiceTypeAssociationsWereMade_WhenEdit_ThenServiceTypeHasSelectedPrograms()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new UserRepository(EducationContext).Items.Where(s => s.UserKey == "Bob").Include("UserRoles.Role").Single());
            var selectedPrograms = new int[] { 1, 2 };
            var serviceType = EducationContext.ServiceTypes.Single(s => s.Id == 2);
            var expected = EducationContext.Programs.Where(s => selectedPrograms.Contains(s.Id)).ToList();
            var viewModel = new ServiceTypeModel { Id = serviceType.Id, Name = serviceType.Name, Description = serviceType.Description, SelectedPrograms = selectedPrograms };

            Target.Edit(viewModel);

            var actual = EducationContext.ServiceTypes.Single(s => s.Id == viewModel.Id).ServiceOfferings.Where(s => s.IsActive).Select(s => s.Program).Distinct();

            CollectionAssert.AreEquivalent(expected, actual.ToList());
        }
    }
}
