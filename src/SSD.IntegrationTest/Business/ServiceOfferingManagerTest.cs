using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Data;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSD.Business
{
    [TestClass]
    public class ServiceOfferingManagerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private ServiceOfferingManager Target { get; set; }
        private EducationSecurityPrincipal User { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            Target = new ServiceOfferingManager(repositoryContainer, new DataTableBinder());
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
        public void WhenGenerateDataTableResultViewModel_ThenSucceed()
        {
            DataTableRequestModel model = new DataTableRequestModel { iDisplayLength = 10 };
            ServiceOfferingClientDataTable dataTable = new ServiceOfferingClientDataTable(MockHttpContextFactory.CreateRequest(), User);

            Target.GenerateDataTableResultViewModel(model, dataTable);
        }

        [TestMethod]
        public void GivenUserIsProvider_WhenGenerateDataTableResultViewModel_ThenSucceed()
        {
            DataTableRequestModel model = new DataTableRequestModel { iDisplayLength = 10 };
            User.Identity.User.UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = new Role { Name = SecurityRoles.Provider },
                    Providers = new List<Provider> { EducationContext.Providers.First() }
                }
            };
            ServiceOfferingClientDataTable dataTable = new ServiceOfferingClientDataTable(MockHttpContextFactory.CreateRequest(), User);
            
            Target.GenerateDataTableResultViewModel(model, dataTable);
        }

        [TestMethod]
        public void WhenGenerateListOptionsViewModel_ThenTypeFilterListPopulated()
        {
            var expected = new[] { "Provide College Access", "Mentoring", "Test service typ,e" };

            ServiceOfferingListOptionsModel actual = Target.GenerateListOptionsViewModel(User);

            CollectionAssert.AreEquivalent(expected, actual.TypeFilterList.ToList());
        }

        [TestMethod]
        public void WhenGenerateListOptionsViewModel_ThenCategoryFilterListPopulated()
        {
            var expected = new[] { "Basic Needs", "Consumer Services", "Criminal Justice and Legal Services", "Education", "Environmental Quality", "Health Care", "Income Support and Employment", "Individual and Family Life", "Mental Health Care and Counseling", "Organizational/Community Services", "Support Groups", "Target Populations", "Test Category," };

            ServiceOfferingListOptionsModel actual = Target.GenerateListOptionsViewModel(User);

            CollectionAssert.AreEquivalent(expected, actual.CategoryFilterList.ToList());
        }
    }
}
