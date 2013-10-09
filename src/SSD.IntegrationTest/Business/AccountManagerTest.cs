using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.Security.Net;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace SSD.Business
{
    [TestClass]
    public class AccountManagerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private AccountManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            Target = new AccountManager(Container, new EmailConfirmationManager(new Mailer()), new DataTableBinder(), new UserAuditor());
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
        public void WhenGenerateDataTableResultViewModel_ThenViewModelContainsData()
        {
            HttpContextBase mockHttpContext = MockHttpContextFactory.Create();
            DataTableRequestModel viewModel = new DataTableRequestModel { iDisplayLength = 10 };
            UserClientDataTable dataTable = new UserClientDataTable(mockHttpContext.Request, new DefaultSecurityConfiguration());

            DataTableResultModel actual = Target.GenerateDataTableResultViewModel(viewModel, dataTable);

            Assert.AreNotEqual(0, actual.aaData.Count());
        }

        [TestMethod]
        public void GivenSortOnStatus_WhenGenerateDataTableResultViewModel_ThenViewModelContainsData()
        {
            HttpContextBase mockHttpContext = MockHttpContextFactory.Create();
            DataTableRequestModel viewModel = new DataTableRequestModel { iDisplayLength = 10 };
            mockHttpContext.Request.Expect(m => m["iSortCol_0"]).Return("0");
            mockHttpContext.Request.Expect(m => m["sSortDir_0"]).Return("acs");
            UserClientDataTable dataTable = new UserClientDataTable(mockHttpContext.Request, new DefaultSecurityConfiguration());

            DataTableResultModel actual = Target.GenerateDataTableResultViewModel(viewModel, dataTable);

            Assert.AreNotEqual(0, actual.aaData.Count());
        }

        [TestMethod]
        public void GivenValidUserId_WhenGenerateUserAssociationsViewModel_ThenViewModelReturned()
        {
            UserAssociationsModel actual = Target.GenerateUserAssociationsViewModel(1);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenValidUserId_AndUserHasAssociatedSchool_WhenGenerateUserAssociationsViewModel_ThenViewModelContainsSchoolName()
        {
            UserAssociationsModel actual = Target.GenerateUserAssociationsViewModel(2);

            CollectionAssert.AreEqual(new[] { "Local High School" }, actual.SchoolNames.ToList());
        }

        [TestMethod]
        public void GivenValidUserId_AndUserHasAssociatedProvider_WhenGenerateUserAssociationsViewModel_ThenViewModelContainsProviderName()
        {
            UserAssociationsModel actual = Target.GenerateUserAssociationsViewModel(3);

            CollectionAssert.AreEqual(new[] { "Big Brothers, Big Sisters" }, actual.ProviderNames.ToList());
        }

        [TestMethod]
        public void GivenValidUserId_WhenGenerateEditViewModel_ThenViewModelReturned()
        {
            UserRoleModel actual = Target.GenerateEditViewModel(1);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenValidViewModel_WhenCreate_ThenSucceed()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(EducationContext.Users.First());
            UserRoleModel viewModel = new UserRoleModel { UserId = 1, PostedRoles = new[] { 1 } };
            
            Target.Create(viewModel, user);
        }

        [TestMethod]
        public void GivenValidViewModel_WhenEdit_ThenSucceed()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(EducationContext.Users.First());
            UserRoleModel viewModel = new UserRoleModel { UserId = 1, PostedRoles = new[] { 1 } };

            Target.Edit(viewModel, user);
        }

        [TestMethod]
        public void WhenGenerateListViewModel_ThenSchoolFilterListPopulated()
        {
            var expected = EducationContext.Schools.Select(s => s.Name).ToList();

            UserModel actual = Target.GenerateListViewModel();

            CollectionAssert.AreEquivalent(expected, actual.SchoolFilterList.ToList());
        }

        [TestMethod]
        public void WhenGenerateListViewModel_ThenRoleFilterListPopulated()
        {
            var expected = EducationContext.Roles.Select(s => s.Name).ToList();

            UserModel actual = Target.GenerateListViewModel();

            CollectionAssert.AreEquivalent(expected, actual.RoleFilterList.ToList());
        }

        [TestMethod]
        public void GivenUserId_WhenUpdateActiveStatus_ThenAuditRecordIncludesRoles_AndAuditRecordIncludesProviders_AndAuditRecordIncludesSchools()
        {
            User userEntity = EducationContext.Users.First();
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(userEntity);
            User newUser = new User
            {
                UserKey = "apples!",
                DisplayName = "apples!",
                EmailAddress = "apples@fruit.com",
                FirstName = "Yum",
                LastName = "Good"
            };
            using (EducationDataContext setupContext = new EducationDataContext())
            {
                newUser.UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = setupContext.Roles.First(),
                        Providers = setupContext.Providers.Take(2).ToList(),
                        Schools = setupContext.Schools.Take(2).ToList(),
                        CreatingUserId = userEntity.Id
                    }
                };
                setupContext.Users.Add(newUser);
                setupContext.SaveChanges();
            }

            Target.UpdateActiveStatus(newUser.Id, true, user);

            using (EducationDataContext verifyContext = new EducationDataContext())
            {
                var actual = verifyContext.UserAccessChangeEvents.Single(e => e.UserId == newUser.Id);
                Assert.IsFalse(actual.AccessXml.Element("roles").IsEmpty);
                Assert.IsFalse(actual.AccessXml.Element("providers").IsEmpty);
                Assert.IsFalse(actual.AccessXml.Element("schools").IsEmpty);
            }
        }

        [TestMethod]
        public void GivenUserIds_WhenUpdateActiveStatus_ThenAuditRecordsIncludesRoles_AndAuditRecordsIncludesProviders_AndAuditRecordsIncludesSchools()
        {
            User userEntity = EducationContext.Users.First();
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(userEntity);
            User newUser1 = new User
            {
                UserKey = "oranges!",
                DisplayName = "oranges!",
                EmailAddress = "oranges@fruit.com",
                FirstName = "Yum",
                LastName = "Good"
            };
            User newUser2 = new User
            {
                UserKey = "bananas!",
                DisplayName = "bananas!",
                EmailAddress = "bananas@fruit.com",
                FirstName = "Yum",
                LastName = "Good"
            };
            using (EducationDataContext setupContext = new EducationDataContext())
            {
                Role role = setupContext.Roles.First();
                List<Provider> providers = setupContext.Providers.Take(2).ToList();
                List<School> schools = setupContext.Schools.Take(2).ToList();
                newUser1.UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        User = newUser1,
                        Role = role,
                        Providers = providers,
                        Schools = schools,
                        CreatingUserId = userEntity.Id
                    }
                };
                newUser2.UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        User = newUser2,
                        Role = role,
                        Providers = providers,
                        Schools = schools,
                        CreatingUserId = userEntity.Id
                    }
                };
                setupContext.Users.Add(newUser1);
                setupContext.Users.Add(newUser2);
                setupContext.SaveChanges();
            }
            int[] ids = new [] { newUser1.Id, newUser2.Id };

            Target.UpdateActiveStatus(ids, true, user);

            using (EducationDataContext verifyContext = new EducationDataContext())
            {
                var auditEventEntries = verifyContext.UserAccessChangeEvents.Where(e => ids.Contains(e.UserId));
                Assert.AreEqual(2, auditEventEntries.Count());
                foreach (var actual in auditEventEntries)
                {
                    Assert.IsFalse(actual.AccessXml.Element("roles").IsEmpty);
                    Assert.IsFalse(actual.AccessXml.Element("providers").IsEmpty);
                    Assert.IsFalse(actual.AccessXml.Element("schools").IsEmpty);
                }
            }
        }

        [TestMethod]
        public void GivenClaimsIdentityWithNameIdentifier_AndUserAcceptedAgreements_WhenEnsureUserEntity_ThenSucceed()
        {
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "Bob")
            });

            Target.EnsureUserEntity(identity);
        }
    }
}
