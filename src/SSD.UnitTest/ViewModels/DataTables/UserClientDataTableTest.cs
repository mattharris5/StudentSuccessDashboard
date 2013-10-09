using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using SSD.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    [TestClass]
    public class UserClientDataTableTest
    {
        private HttpRequestBase MockRequest { get; set; }
        private ISecurityConfiguration SecurityConfiguration { get; set; }
        private TestData TestData { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockRequest = MockHttpContextFactory.CreateRequest();
            SecurityConfiguration = MockRepository.GenerateMock<ISecurityConfiguration>();
            TestData = new TestData();
        }

        [TestMethod]
        public void GivenNullSecurityConfiguration_WhenIConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new UserClientDataTable(MockRequest, null));
        }

        [TestMethod]
        public void GivenUserIsConfiguredAsAdministrator_WhenIExecuteDataSelector_ThenDataRolesIsAdministrator()
        {
            UserClientDataTable target = new UserClientDataTable(MockRequest, SecurityConfiguration);
            User user = new User { EmailAddress = "bob@bob.bob" };
            SecurityConfiguration.Expect(m => m.AdministratorEmailAddresses).Return(new string[] { user.EmailAddress });

            dynamic actual = target.DataSelector.Compile().Invoke(user);

            Assert.AreEqual(SecurityRoles.Administrator, ((IEnumerable<string>)actual.Roles).Single());
        }

        [TestMethod]
        public void GivenUserIsNotConfiguredAsAdministrator_WhenIExecuteDataSelector_ThenDataRolesDoesNotContainAdministrator()
        {
            UserClientDataTable target = new UserClientDataTable(MockRequest, SecurityConfiguration);
            User user = new User { EmailAddress = "bob@bob.bob" };
            SecurityConfiguration.Expect(m => m.AdministratorEmailAddresses).Return(new string[] { "jim@jim.jim" });

            dynamic actual = target.DataSelector.Compile().Invoke(user);

            Assert.IsFalse(((IEnumerable<string>)actual.Roles).Contains(SecurityRoles.Administrator));
        }

        [TestMethod]
        public void GivenUserIsConfiguredAsAdministrator_AndHasDataAdminRole_WhenIExecuteDataSelector_ThenDataRolesContainsAdministratorAndDataAdmin()
        {
            UserClientDataTable target = new UserClientDataTable(MockRequest, SecurityConfiguration);
            User user = new User
            {
                EmailAddress = "bob@bob.bob",
                UserRoles = new List<UserRole>
                {
                    new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } }
                }
            };
            string[] expected = new[] { "Administrator", SecurityRoles.DataAdmin };
            SecurityConfiguration.Expect(m => m.AdministratorEmailAddresses).Return(new string[] { user.EmailAddress });

            dynamic actual = target.DataSelector.Compile().Invoke(user);

            CollectionAssert.AreEqual(expected, ((IEnumerable<string>)actual.Roles).ToList());
        }

        [TestMethod]
        public void GivenUserHasProviderAssociations_WhenIExecuteDataSelector_ThenAssociationsIsProviderAssociationCount()
        {
            UserClientDataTable target = new UserClientDataTable(MockRequest, SecurityConfiguration);
            var providers = new List<Provider> { new Provider(), new Provider(), new Provider() };
            User user = new User
            {
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = new Role(),
                        Providers = providers
                    }
                }
            };
            SecurityConfiguration.Expect(m => m.AdministratorEmailAddresses).Return(new string[] { "jim@jim.jim" });

            dynamic actual = target.DataSelector.Compile().Invoke(user);

            Assert.AreEqual(providers.Count, actual.Associations);
        }

        [TestMethod]
        public void GivenUserHasSchoolAssociations_WhenIExecuteDataSelector_ThenAssociationsIsSchoolAssociationCount()
        {
            UserClientDataTable target = new UserClientDataTable(MockRequest, SecurityConfiguration);
            var schools = new List<School> { new School(), new School() };
            User user = new User
            {
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = new Role(),
                        Schools = schools
                    }
                }
            };
            SecurityConfiguration.Expect(m => m.AdministratorEmailAddresses).Return(new string[] { "jim@jim.jim" });

            dynamic actual = target.DataSelector.Compile().Invoke(user);

            Assert.AreEqual(schools.Count, actual.Associations);
        }

        [TestMethod]
        public void GivenUserHasProviderAssociations_AndUserHasSchoolAssociations_WhenIExecuteDataSelector_ThenAssociationsIsSumOfSchoolAssociationCountAndProviderAssociationCount()
        {
            UserClientDataTable target = new UserClientDataTable(MockRequest, SecurityConfiguration);
            var schools = new List<School> { new School(), new School() };
            var providers = new List<Provider> { new Provider(), new Provider(), new Provider() };
            User user = new User
            {
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = new Role(),
                        Schools = schools,
                        Providers = providers
                    }
                }
            };
            SecurityConfiguration.Expect(m => m.AdministratorEmailAddresses).Return(new string[] { "jim@jim.jim" });

            dynamic actual = target.DataSelector.Compile().Invoke(user);

            Assert.AreEqual(schools.Count + providers.Count, actual.Associations);
        }

        [TestMethod]
        public void GivenUserHasLoginEvents_WhenIExecuteDataSelector_ThenDataHasLastLoginTime()
        {
            UserClientDataTable target = new UserClientDataTable(MockRequest, SecurityConfiguration);
            User user = new User
            {
                LoginEvents = new List<LoginEvent>
                {
                    new LoginEvent { CreateTime = DateTime.Now }
                }
            };
            SecurityConfiguration.Expect(m => m.AdministratorEmailAddresses).Return(new string[] { user.EmailAddress });

            dynamic actual = target.DataSelector.Compile().Invoke(user);

            Assert.AreEqual(user.LastLoginTime, actual.LastLoginTime);
        }

        [TestMethod]
        public void GivenUserHasProviderAssociations_AndSortOnAssociationCount_WhenExecuteSortSelector_ThenReturnProviderAssociationCount()
        {
            MockRequest.Expect(m => m["iSortCol_0"]).Return("4");
            MockRequest.Expect(m => m["sSortDir_0"]).Return("asc");
            UserClientDataTable target = new UserClientDataTable(MockRequest, SecurityConfiguration);
            User user = new User
            {
                UserRoles = new List<UserRole>
                {
                    new UserRole { Providers = new List<Provider> { new Provider(), new Provider(), new Provider() } }
                }
            };

            var actual = target.SortSelector.Compile().Invoke(user);

            Assert.AreEqual("3", actual);
        }

        [TestMethod]
        public void GivenSortDirectionOnStatus_WhenExecuteSortSelector_ThenReturnStatus()
        {
            MockRequest.Expect(m => m["iSortCol_0"]).Return("0");
            MockRequest.Expect(m => m["sSortDir_0"]).Return("asc");
            UserClientDataTable target = new UserClientDataTable(MockRequest, SecurityConfiguration);
            User user = new User { Active = false };

            var actual = target.SortSelector.Compile().Invoke(user);

            Assert.AreEqual("Inactive", actual);
        }

        [TestMethod]
        public void GivenSortDirectionOnLastName_WhenExecuteSortSelector_ThenReturnLastName()
        {
            MockRequest.Expect(m => m["iSortCol_0"]).Return("1");
            MockRequest.Expect(m => m["sSortDir_0"]).Return("asc");
            UserClientDataTable target = new UserClientDataTable(MockRequest, SecurityConfiguration);
            User user = new User { LastName = "Deitz" };

            var actual = target.SortSelector.Compile().Invoke(user);

            Assert.AreEqual("Deitz", actual);
        }

        [TestMethod]
        public void GivenSortDirectionOnFirstName_WhenExecuteSortSelector_ThenReturnFirstName()
        {
            MockRequest.Expect(m => m["iSortCol_0"]).Return("2");
            MockRequest.Expect(m => m["sSortDir_0"]).Return("asc");
            UserClientDataTable target = new UserClientDataTable(MockRequest, SecurityConfiguration);
            User user = new User { FirstName = "Alec" };

            var actual = target.SortSelector.Compile().Invoke(user);

            Assert.AreEqual("Alec", actual);
        }

        [TestMethod]
        public void GivenSortDirectionOnLastLoginTime_WhenExecuteSortSelector_ThenReturnLastLoginTimeInMinutes()
        {
            MockRequest.Expect(m => m["iSortCol_0"]).Return("3");
            MockRequest.Expect(m => m["sSortDir_0"]).Return("asc");
            UserClientDataTable target = new UserClientDataTable(MockRequest, SecurityConfiguration);
            User user = new User
            {
                LoginEvents = new List<LoginEvent>
                {
                    new LoginEvent { CreateTime = DateTime.Now }
                }
            };
            string expected = ((int)new TimeSpan(user.LastLoginTime.Value.Ticks).TotalMinutes).ToString();

            var actual = target.SortSelector.Compile().Invoke(user);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenFirstNameSearchCriteria_AndUserNamePartiallyMatches_WhenInvokeFilterPredicate_ThenReturnTrue()
        {
            User user = new User { FirstName = "Alec" };
            MockRequest.Expect(m => m["firstName"]).Return("Ale");
            var target = new UserClientDataTable(MockRequest, SecurityConfiguration);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(user));
        }

        [TestMethod]
        public void GivenFirstNameSearchCriteria_AndUserNameDoesntPartiallyMatch_WhenInvokeFilterPredicate_ThenReturnFalse()
        {
            User user = new User { FirstName = "Alec" };
            MockRequest.Expect(m => m["firstName"]).Return("xyz");
            var target = new UserClientDataTable(MockRequest, SecurityConfiguration);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(user));
        }

        [TestMethod]
        public void GivenLastNameSearchCriteria_AndUserNamePartiallyMatches_WhenInvokeFilterPredicate_ThenReturnTrue()
        {
            User user = new User { LastName = "Deitz" };
            MockRequest.Expect(m => m["lastName"]).Return("Dei");
            var target = new UserClientDataTable(MockRequest, SecurityConfiguration);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(user));
        }

        [TestMethod]
        public void GivenLastNameSearchCriteria_AndUserNameDoesntPartiallyMatch_WhenInvokeFilterPredicate_ThenReturnFalse()
        {
            User user = new User { LastName = "Deitz" };
            MockRequest.Expect(m => m["lastName"]).Return("xyz");
            var target = new UserClientDataTable(MockRequest, SecurityConfiguration);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(user));
        }

        [TestMethod]
        public void GivenEmailSearchCriteria_AndUserEmailPartiallyMatches_WhenInvokeFilterPredicate_ThenReturnTrue()
        {
            User user = new User { EmailAddress = "alecdeitz@live.com" };
            MockRequest.Expect(m => m["email"]).Return("Dei");
            var target = new UserClientDataTable(MockRequest, SecurityConfiguration);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(user));
        }

        [TestMethod]
        public void GivenEmailSearchCriteria_AndUserEmailDoesntPartiallyMatch_WhenInvokeFilterPredicate_ThenReturnFalse()
        {
            User user = new User { EmailAddress = "alecdeitz@live.com" };
            MockRequest.Expect(m => m["email"]).Return("xyz");
            var target = new UserClientDataTable(MockRequest, SecurityConfiguration);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(user));
        }

        [TestMethod]
        public void GivenStatusFilterCriteria_AndUserHasStatus_WhenInvokeFilterPredicate_ThenReturnTrue()
        {
            User user = new User { Active = false };
            MockRequest.Expect(m => m["status"]).Return("Inactive");
            var target = new UserClientDataTable(MockRequest, SecurityConfiguration);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(user));
        }

        [TestMethod]
        public void GivenStatusFilterCriteria_AndUserDoesntHaveStatus_WhenInvokeFilterPredicate_ThenReturnFalse()
        {
            User user = new User { Active = true };
            MockRequest.Expect(m => m["status"]).Return("Inactive");
            var target = new UserClientDataTable(MockRequest, SecurityConfiguration);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(user));
        }

        [TestMethod]
        public void GivenRoleFilterCriteria_AndUserHasRole_WhenInvokeFilterPredicate_ThenReturnTrue()
        {
            User user = new User { UserRoles = new List<UserRole> { new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } } } };
            MockRequest.Expect(m => m["roles"]).Return("Data Admin");
            var target = new UserClientDataTable(MockRequest, SecurityConfiguration);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(user));
        }

        [TestMethod]
        public void GivenRoleFilterCriteria_AndUserDoesntHaveRole_WhenInvokeFilterPredicate_ThenReturnFalse()
        {
            User user = new User { UserRoles = new List<UserRole> { new UserRole { Role = new Role { Name = SecurityRoles.SiteCoordinator } } } };
            MockRequest.Expect(m => m["roles"]).Return("Data Admin");
            var target = new UserClientDataTable(MockRequest, SecurityConfiguration);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(user));
        }

        [TestMethod]
        public void GivenSchoolsFilterCriteria_AndUserIsAssociatedToSchool_WhenInvokeFilterPredicate_ThenReturnTrue()
        {
            User user = new User { UserRoles = new List<UserRole> { new UserRole { Schools = new List<School>{ TestData.Schools[0] } } } };
            MockRequest.Expect(m => m["schools"]).Return(TestData.Schools[0].Name);
            var target = new UserClientDataTable(MockRequest, SecurityConfiguration);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(user));
        }

        [TestMethod]
        public void GivenSchoolsFilterCriteria_AndUserIsNotAssociatedToSchool_WhenInvokeFilterPredicate_ThenReturnFalse()
        {
            User user = new User { UserRoles = new List<UserRole> { new UserRole { Schools = new List<School> { TestData.Schools[1] } } } };
            MockRequest.Expect(m => m["schools"]).Return(TestData.Schools[0].Name);
            var target = new UserClientDataTable(MockRequest, SecurityConfiguration);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(user));
        }
    }
}
