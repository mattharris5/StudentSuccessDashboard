using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Data;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace SSD.Business
{
    [TestClass]
    public class ProviderManagerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private ProviderManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            Target = new ProviderManager(repositoryContainer, new DataTableBinder());
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
        public void WhenGenerateDataTableResultViewModel_ThenSucceed()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(EducationContext.Users.First());
            DataTableRequestModel model = new DataTableRequestModel();
            ProviderClientDataTable dataTable = new ProviderClientDataTable(MockHttpContextFactory.CreateRequest(), user);

            Target.GenerateDataTableResultViewModel(model, dataTable);
        }

        [TestMethod]
        public void GivenValidViewModel_WhenValidate_ThenSucceed()
        {
            ProviderModel viewModel = new ProviderModel { Id = 1, Name = "YMCA" };

            Target.Validate(viewModel);
        }

        [TestMethod]
        public void GivenViewModelOfDuplicate_WhenValidate_ThenThrowException()
        {
            ProviderModel viewModel = new ProviderModel { Id = 0 /* Default */, Name = "YMCA" };

            Target.ExpectException<ValidationException>(() => Target.Validate(viewModel));
        }

        [TestMethod]
        public void GivenProviderProgramAssociationsWereMade_WhenEdit_ThenProviderHasSelectedPrograms()
        {
            try
            {
                EducationSecurityPrincipal user = new EducationSecurityPrincipal(new UserRepository(EducationContext).Items.Where(s => s.UserKey == "Bob").Include("UserRoles.Role").Single());
                var expected = new int[] { 1, 2 };
                var provider = EducationContext.Providers.First();
                var viewModel = new ProviderModel { Id = provider.Id, Name = provider.Name, SelectedPrograms = expected, Address = provider.Address, Contact = provider.Contact, Website = provider.Website };

                Target.Edit(user, viewModel);

                using (EducationDataContext verificationContext = new EducationDataContext())
                {
                    var actual = verificationContext.Providers.Include(p => p.ServiceOfferings).Single(p => p.Id == viewModel.Id).ServiceOfferings.Where(s => s.IsActive).Select(s => s.ProgramId).Distinct();
                    CollectionAssert.AreEquivalent(expected, actual.ToList());
                }
            }
            finally
            {
                AssemblySetup.ForceDeleteEducationDatabase("SSD");
            }
        }

        [TestMethod]
        public void GivenProviderWithApprovingStudents_WhenDelete_ThenNoApprovedProviderMappingsToDeletedProvider()
        {
            int toDeleteId;
            using (EducationDataContext setupContext = new EducationDataContext())
            {
                Provider temp = new Provider
                {
                    Name = "blah blah blah",
                    IsActive = true,
                    ApprovingStudents = setupContext.Students.ToList()
                };
                foreach (Student student in temp.ApprovingStudents)
                {
                    student.ApprovedProviders.Add(temp);
                }
                setupContext.Providers.Add(temp);
                setupContext.SaveChanges();
                toDeleteId = temp.Id;
            }

            Target.Delete(toDeleteId);

            Assert.IsFalse(EducationContext.Students.Any(s => s.ApprovedProviders.Select(p => p.Id).Contains(toDeleteId)));
        }

        [TestMethod]
        public void GivenProviderWithUserAssociations_WhenDelete_ThenNoUserAssociationsToDeletedProvider()
        {
            int toDeleteId;
            using (EducationDataContext setupContext = new EducationDataContext())
            {
                Provider temp = new Provider
                {
                    Name = "halb halb halb",
                    IsActive = true,
                    UserRoles = setupContext.UserRoles.Where(u => u.Role.Name == SecurityRoles.Provider).ToList()
                };
                foreach (UserRole userRole in temp.UserRoles)
                {
                    userRole.Providers.Add(temp);
                }
                setupContext.Providers.Add(temp);
                setupContext.SaveChanges();
                toDeleteId = temp.Id;
            }

            Target.Delete(toDeleteId);

            Assert.IsFalse(EducationContext.UserRoles.Any(u => u.Providers.Select(p => p.Id).Contains(toDeleteId)));
        }
    }
}
