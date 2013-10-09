using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Business
{
    [TestClass]
    public class AccountManagerTest : BaseManagerTest
    {
        private List<User> Users { get; set; }
        private IWindsorContainer MockWindsorContainer { get; set; }
        private IEmailConfirmationManager MockEmailConfirmationManager { get; set; }
        private IUserAuditor MockUserAuditor { get; set; }
        private AccountManager Target { get; set; }
        private ISecurityConfiguration MockSecurityConfiguration { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Users = Data.Users;
            MockWindsorContainer = MockRepository.GenerateMock<IWindsorContainer>();
            MockWindsorContainer.Expect(m => m.Resolve<IRepositoryContainer>()).Return(Repositories.MockRepositoryContainer);
            MockEmailConfirmationManager = MockRepository.GenerateMock<IEmailConfirmationManager>();
            MockUserAuditor = MockRepository.GenerateMock<IUserAuditor>();
            Target = new AccountManager(MockWindsorContainer, MockEmailConfirmationManager, MockDataTableBinder, MockUserAuditor);
            MockSecurityConfiguration = MockRepository.GenerateMock<ISecurityConfiguration>();
            MockSecurityConfiguration.Expect(m => m.AdministratorEmailAddresses).Return(Enumerable.Empty<string>());
        }

        [TestMethod]
        public void GivenNullWindsorContainer_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new AccountManager(null, MockEmailConfirmationManager, MockDataTableBinder, MockUserAuditor));
        }

        [TestMethod]
        public void GivenNullEmailConfirmationManager_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new AccountManager(MockWindsorContainer, null, MockDataTableBinder, MockUserAuditor));
        }

        [TestMethod]
        public void GivenNullDataTableBinder_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new AccountManager(MockWindsorContainer, MockEmailConfirmationManager, null, MockUserAuditor));
        }

        [TestMethod]
        public void GivenNullUserAuditor_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new AccountManager(MockWindsorContainer, MockEmailConfirmationManager, MockDataTableBinder, null));
        }

        [TestMethod]
        public void GivenClaimsIdentityWithNameIdentifier_AndUserExists_WhenEnsureUserEntity_ThenUserReturned_AndNewUserNotCreated()
        {
            string userKey = "testkey";
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userKey)
            });
            User expected = Data.Users.Single(u => u.UserKey == userKey);

            User actual = Target.EnsureUserEntity(identity);

            Assert.AreEqual(expected, actual);
            Repositories.MockUserRepository.AssertWasNotCalled(m => m.Add(actual));
            Repositories.MockRepositoryContainer.AssertWasNotCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenClaimsIdentityWithNameIdentifier_AndUserDoesNotExist_WhenEnsureUserEntity_ThenUserIsCreated()
        {
            string userKey = "new user";
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userKey)
            });
            AddAdditionalUsers();

            User actual = Target.EnsureUserEntity(identity);

            Repositories.MockUserRepository.AssertWasCalled(m => m.Add(actual));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenClaimsIdentityWithNameIdentifier_AndUserDoesNotExist_WhenEnsureUserEntity_ThenUserHasValidState()
        {
            string userKey = "new user";
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userKey)
            });
            AddAdditionalUsers();

            User actual = Target.EnsureUserEntity(identity);

            Assert.IsNotNull(actual);
            Assert.AreEqual(userKey, actual.UserKey);
            Assert.AreEqual("Anonymous", actual.FirstName);
            Assert.AreEqual("Anonymous", actual.LastName);
            Assert.AreEqual("Anonymous", actual.DisplayName);
            Assert.AreEqual("Anonymous@sample.com", actual.EmailAddress);
            Assert.IsTrue(actual.Active);
            Assert.AreEqual(DateTime.Now.Ticks / TimeSpan.TicksPerSecond, actual.CreateTime.Ticks / TimeSpan.TicksPerSecond);
        }

        [TestMethod]
        public void GivenClaimsIdentityWithNameIdentifier_AndClaimsIdentityWithEmail_AndUserDoesNotExist_WhenEnsureUserEntity_ThenUserEmailSetToClaimValue()
        {
            string email = "this is the email";
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "whatever"),
                new Claim(ClaimTypes.Email, email)
            });
            AddAdditionalUsers();

            User actual = Target.EnsureUserEntity(identity);

            Assert.AreEqual(email, actual.EmailAddress);
        }

        [TestMethod]
        public void GivenClaimsIdentityWithNameIdentifier_AndClaimsIdentityWithName_AndUserDoesNotExist_WhenEnsureUserEntity_ThenUserDisplayNameSetToClaimValue()
        {
            string name = "this is the name";
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "whatever"),
                new Claim(ClaimTypes.Name, name)
            });
            AddAdditionalUsers();

            User actual = Target.EnsureUserEntity(identity);

            Assert.AreEqual(name, actual.DisplayName);
        }

        [TestMethod]
        public void GivenClaimsIdentityWithNameIdentifier_AndClaimsIdentityWithEmail_AndEmailHasAlreadyBeenRegistered_AndUserDoesNotExist_WhenEnsureUserEntity_ThenUserEmailSetToDefaultAndIgnoresClaimValue()
        {
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "whatever"),
                new Claim(ClaimTypes.Email, Data.Users.First().EmailAddress)
            });
            AddAdditionalUsers();

            User actual = Target.EnsureUserEntity(identity);

            Assert.AreEqual("Anonymous@sample.com", actual.EmailAddress);
        }
        
        [TestMethod]
        public void GivenNullUser_WhenValidateEulaAccepted_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.ValidateEulaAccepted(null));
        }

        [TestMethod]
        public void GivenUserHasNoAgreementsAccepted_WhenValidateEulaAccepted_ThenExceptionThrown()
        {
            User userEntity = Data.Users.Single(u => u.UserKey == "testkey");
            userEntity.EulaAcceptances.Clear();
            
            Target.ExpectException<LicenseAgreementException>(() => Target.ValidateEulaAccepted(userEntity));
        }

        [TestMethod]
        public void GivenUserHasCurrentAgreementAccepted_WhenValidateEulaAccepted_ThenSucceed()
        {
            User userEntity = Data.Users.Single(u => u.UserKey == "testkey");
            userEntity.EulaAcceptances.Clear();
            userEntity.EulaAcceptances.Add(new EulaAcceptance { EulaAgreementId = Data.Eulas.OrderByDescending(e => e.CreateTime).Select(e => e.Id).First() });

            Target.ValidateEulaAccepted(userEntity);
        }

        [TestMethod]
        public void GivenUserHasOutdatedAgreementsAccepted_WhenValidateEulaAccepted_ThenExceptionThrown()
        {
            User userEntity = Data.Users.Single(u => u.UserKey == "testkey");
            userEntity.EulaAcceptances.Clear();
            userEntity.EulaAcceptances.Add(new EulaAcceptance { EulaAgreementId = Data.Eulas.OrderByDescending(e => e.CreateTime).Skip(1).Select(e => e.Id).First() });

            Target.ExpectException<LicenseAgreementException>(() => Target.ValidateEulaAccepted(userEntity));
        }

        [TestMethod]
        public void GivenNullUser_WhenIGenerateUserProfileViewModel_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.GenerateUserProfileViewModel(null));
        }

        [TestMethod]
        public void GivenUserHasIdentifier_WhenIGenerateUserProfileViewModel_ThenViewModelRetured()
        {
            UserModel actual = Target.GenerateUserProfileViewModel(new EducationSecurityPrincipal(new EducationSecurityIdentity(new ClaimsIdentity(), Users[1])));

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenUserHasIdentifier_WhenIGenerateUserProfileViewModel_ThenViewModelContainsValidUserKey()
        {
            AddAdditionalUsers();
            string userIdentifier = "29e8r2fj";
            User user = Users.Single(u => u.UserKey == userIdentifier);

            UserModel actual = Target.GenerateUserProfileViewModel(new EducationSecurityPrincipal(new EducationSecurityIdentity(new ClaimsIdentity(), user)));

            Assert.AreEqual(userIdentifier, actual.UserKey);
        }

        [TestMethod]
        public void GivenUser_WhenEditUserProfile_ThenUpdateUserInRepository()
        {
            User user = Users[1];
            AddAdditionalUsers();

            Target.Edit(new UserModel { Id = user.Id, PendingEmail = user.EmailAddress }, new UrlHelper(new RequestContext()));

            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenUser_WhenEditUserProfile_ThenUserPropertiesMatchViewModel()
        {
            User actualState = Users[1];
            var expectedState = new UserModel { Id = actualState.Id, PendingEmail = actualState.EmailAddress, UserKey = actualState.UserKey, DisplayName = "expected display name", EmailAddress = "expected email address" };
            AddAdditionalUsers();

            Target.Edit(expectedState, new UrlHelper(new RequestContext()));

            Assert.AreEqual(expectedState.DisplayName, actualState.DisplayName);
            Assert.AreEqual(expectedState.PendingEmail, actualState.PendingEmail);
        }

        [TestMethod]
        public void GivenUser_AndPendingEmailChanges_WhenEditUserProfile_ThenEmailConfirmationRequested()
        {
            User expected = Users[1];
            var expectedState = new UserModel { Id = expected.Id, UserKey = expected.UserKey, PendingEmail = "different!" };
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            MockHttpContext.Request.Expect(m => m.Url).Return(new Uri("http://tempuri.org"));
            AddAdditionalUsers();

            Target.Edit(expectedState, new UrlHelper(new RequestContext(MockHttpContext, new RouteData())));

            MockEmailConfirmationManager.AssertWasCalled(m => m.Request(expected, new Uri("http://tempuri.org/Account/ConfirmEmail")));
        }

        [TestMethod]
        public void GivenUser_AndPendingEmailStaysTheSameAsEmailAddress_WhenEditUserProfile_ThenEmailConfirmationNotRequested()
        {
            User notExpected = Users[1];
            var expectedState = new UserModel { Id = notExpected.Id, UserKey = notExpected.UserKey, PendingEmail = notExpected.EmailAddress };
            AddAdditionalUsers();

            Target.Edit(expectedState, new UrlHelper(new RequestContext()));

            MockEmailConfirmationManager.AssertWasNotCalled(m => m.Request(notExpected, new Uri("http://tempuri.org/Account/ConfirmEmail")));
        }

        [TestMethod]
        public void GivenUserHasGuidIdentifier_WhenGenerateConfirmEmailViewModel_ThenConfirmationProcessed()
        {
            Guid identifier = Guid.NewGuid();
            User user = Users[1];
            user.ConfirmationGuid = identifier;

            Target.GenerateConfirmEmailViewModel(identifier);

            MockEmailConfirmationManager.AssertWasCalled(m => m.Process(user));
        }

        [TestMethod]
        public void GivenUserHasGuidIdentifier_WhenGenerateConfirmEmailViewModel_ThenConfirmationNotProcessed()
        {
            Guid identifier = Guid.NewGuid();
            AddAdditionalUsers();

            Target.GenerateConfirmEmailViewModel(identifier);

            MockEmailConfirmationManager.AssertWasNotCalled(m => m.Process(new User()), options => options.IgnoreArguments());
        }

        [TestMethod]
        public void GivenUserHasGuidIdentifier_WhenGenerateConfirmEmailViewModel_ThenUserUpdated_AndSaved()
        {
            Guid identifier = Guid.NewGuid();
            User user = Users[1];
            user.ConfirmationGuid = identifier;

            Target.GenerateConfirmEmailViewModel(identifier);

            Repositories.MockUserRepository.AssertWasCalled(m => m.Update(user));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenUserHasGuidIdentifier_WhenGenerateConfirmEmailViewModel_ThenUserNotUpdatedOrSaved()
        {
            Guid identifier = Guid.NewGuid();
            AddAdditionalUsers();

            Target.GenerateConfirmEmailViewModel(identifier);

            Repositories.MockUserRepository.AssertWasNotCalled(m => m.Update(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasNotCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenUserHasGuidIdentifier_WhenGenerateConfirmEmailViewModel_ThenViewModelHasSuccessState()
        {
            Guid identifier = Guid.NewGuid();
            User user = Users[1];
            var expectedEmailAddress = "jim@jim.jim";
            user.ConfirmationGuid = identifier;
            user.PendingEmail = expectedEmailAddress;
            AddAdditionalUsers();

            ConfirmEmailModel actual = Target.GenerateConfirmEmailViewModel(identifier);

            Assert.IsTrue(actual.Success);
            Assert.AreEqual(expectedEmailAddress, actual.UserEmailAddress);
            Assert.AreEqual(user.DisplayName, actual.UserDisplayName);
        }

        [TestMethod]
        public void GivenUserHasGuidIdentifier_WhenGenerateConfirmEmailViewModel_ThenViewModelSuccessIsFalse()
        {
            Guid identifier = Guid.NewGuid();
            AddAdditionalUsers();

            ConfirmEmailModel actual = Target.GenerateConfirmEmailViewModel(identifier);

            Assert.IsFalse(actual.Success);
        }

        [TestMethod]
        public void GivenPendingEmailHasAlreadyBeenConfirmedWithAnotherAccount_WhenGenerateConfirmEmailViewModel_ThenViewModelSuccessIsFalse()
        {
            Guid identifier = Guid.NewGuid();
            User user = Users[1];
            user.ConfirmationGuid = identifier;
            user.PendingEmail = Data.Users.First(u => u != user).EmailAddress;
            AddAdditionalUsers();

            ConfirmEmailModel actual = Target.GenerateConfirmEmailViewModel(identifier);

            Assert.IsFalse(actual.Success);
        }

        [TestMethod]
        public void WhenSearchFirstNames_ThenArrayOfUserFirstNamesIsReturned()
        {
            var searchTerm = "J";
            var expected = new List<string> { "Jon" };
            AddAdditionalUsers();

            IEnumerable<string> actual = Target.SearchFirstNames(searchTerm);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenDuplicateFirstNames_WhenSearchFirstNames_ThenArrayOfUserFirstNamesIsReturned()
        {
            var searchTerm = "ni";
            var expected = new List<string> { "Nick" };
            AddAdditionalUsers();

            IEnumerable<string> actual = Target.SearchFirstNames(searchTerm);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void WhenSearchLastNames_ThenArrayOfUserLastNamesIsReturned()
        {
            var searchTerm = "G";
            var expected = new List<string> { "Gear" };
            AddAdditionalUsers();

            IEnumerable<string> actual = Target.SearchLastNames(searchTerm);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenDuplicateLastNames_WhenSearchLastNames_ThenArrayOfUserLastNamesIsReturned()
        {
            var searchTerm = "Martin";
            var expected = new List<string> { "Martin" };
            AddAdditionalUsers();

            IEnumerable<string> actual = Target.SearchLastNames(searchTerm);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void WhenSearchEmails_ThenArrayOfUserEmailsIsReturned()
        {
            var searchTerm = "G";
            var expected = new List<string> { "jgear@gear.com" };
            AddAdditionalUsers();

            IEnumerable<string> actual = Target.SearchEmails(searchTerm);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenDuplicateEmails_WhenSearchEmails_ThenArrayOfUserEmailsIsReturned()
        {
            var searchTerm = "Martin";
            var expected = new List<string> { "nmartin@martin.com" };
            AddAdditionalUsers();

            IEnumerable<string> actual = Target.SearchEmails(searchTerm);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenAnInvalidUserId_WhenGenerateCreateViewModel_ThenThrowEntityNotFound()
        {
            AddAdditionalUsers();

            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateCreateViewModel(878797));
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenAPartialViewResultWithAPopulatedSchoolList()
        {
            AddAdditionalUsers();

            UserRoleModel actual = Target.GenerateCreateViewModel(1);

            Assert.IsNotNull(actual.Schools);
            CollectionAssert.AreEquivalent(Data.Schools, actual.Schools.Items.Cast<School>().ToList());
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenViewModelUserRoleListColumnsAreCorrect()
        {
            AddAdditionalUsers();

            UserRoleModel actual = Target.GenerateCreateViewModel(1);

            Assert.AreEqual("Id", actual.Schools.DataValueField);
            Assert.AreEqual("Name", actual.Schools.DataTextField);
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenAPartialViewResultWithAPopulatedProviderList()
        {
            foreach (Provider p in Data.Providers)
            {
                p.IsActive = true;
            }
            AddAdditionalUsers();

            UserRoleModel actual = Target.GenerateCreateViewModel(1);

            Assert.IsNotNull(actual.Providers);
            CollectionAssert.AreEquivalent(Data.Providers, actual.Providers.Items.Cast<Provider>().ToList());
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenViewModelUserRoleProviderListColumnsAreCorrect()
        {
            AddAdditionalUsers();

            UserRoleModel actual = Target.GenerateCreateViewModel(1);

            Assert.AreEqual("Id", actual.Providers.DataValueField);
            Assert.AreEqual("Name", actual.Providers.DataTextField);
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenProviderSelectListIsSortedByName()
        {
            foreach (Provider p in Data.Providers)
            {
                p.IsActive = true;
            }
            var expected = Data.Providers.OrderBy(p => p.Name).Select(p => p.Name).ToList();

            UserRoleModel actual = Target.GenerateCreateViewModel(1);

            var actualList = actual.Providers.Items.Cast<Provider>().Select(p => p.Name).ToList();
            CollectionAssert.AreEqual(expected, actualList);
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenProviderSelectListDoesNotContainInactiveProviders()
        {
            AddAdditionalUsers();
            foreach (Provider p in Data.Providers)
            {
                p.IsActive = false;
            }

            UserRoleModel actual = Target.GenerateCreateViewModel(1);

            Assert.IsFalse(actual.Providers.Items.Cast<Provider>().Any());
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenSchoolSelectListIsSortedByName()
        {
            var expected = Data.Schools.OrderBy(p => p.Name).Select(s => s.Name).ToList();

            UserRoleModel actual = Target.GenerateCreateViewModel(1);

            var actualList = actual.Schools.Items.Cast<School>().Select(s => s.Name).ToList();
            CollectionAssert.AreEqual(expected, actualList);
        }

        [TestMethod]
        public void WhenGenerateEditViewModel_ThenThrowEntityNotFound()
        {
            AddAdditionalUsers();

            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateEditViewModel(7689987));
        }

        [TestMethod]
        public void WhenGenerateEditViewModel_ThenAPartialViewIsReturned()
        {
            var user = Users.Where(u => u.Id == 1).SingleOrDefault();
            UserRoleModel expectedViewModel = new UserRoleModel();
            expectedViewModel.CopyFrom(user);
            expectedViewModel.UserRoleIds = user.UserRoles.Select(u => u.Id);
            expectedViewModel.AvailableRoles = Repositories.MockRoleRepository.Items;
            expectedViewModel.SelectedRoles = user.UserRoles.Select(u => u.Role);
            AddAdditionalUsers();

            UserRoleModel actual = Target.GenerateEditViewModel(1);

            Assert.IsNotNull(actual);
            CollectionAssert.AreEqual(expectedViewModel.AvailableRoles.ToList(), actual.AvailableRoles.ToList());
            Assert.AreEqual(expectedViewModel.Comments, actual.Comments);
            Assert.AreEqual(expectedViewModel.PostedRoles, actual.PostedRoles);
            CollectionAssert.AreEqual(expectedViewModel.SelectedRoles.ToList(), actual.SelectedRoles.ToList());
            CollectionAssert.AreEqual(expectedViewModel.SelectedSchoolIds.ToList(), actual.SelectedSchoolIds.ToList());
            Assert.AreEqual(expectedViewModel.UserId, actual.UserId);
            CollectionAssert.AreEqual(expectedViewModel.UserRoleIds.ToList(), actual.UserRoleIds.ToList());
        }

        [TestMethod]
        public void WhenGenerateEditViewModel_ThenProviderSelectListIsSortedByName()
        {
            foreach (Provider p in Data.Providers)
            {
                p.IsActive = true;
            }
            var expected = Data.Providers.OrderBy(p => p.Name).Select(s => s.Name).ToList();

            UserRoleModel actual = Target.GenerateEditViewModel(1);

            var actualList = actual.Providers.Items.Cast<Provider>().Select(p => p.Name).ToList();
            CollectionAssert.AreEqual(expected, actualList);
        }

        [TestMethod]
        public void WhenGenerateEditViewModel_ThenProviderSelectListDoesNotContainInactiveProviders()
        {
            foreach (Provider p in Data.Providers)
            {
                p.IsActive = false;
            }

            UserRoleModel actual = Target.GenerateEditViewModel(1);

            Assert.IsFalse(actual.Providers.Items.Cast<Provider>().Any());
        }

        [TestMethod]
        public void WhenGenerateEditViewModel_ThenSchoolSelectListIsSortedByName()
        {
            var expected = Data.Schools.OrderBy(p => p.Name).Select(s => s.Name).ToList();

            UserRoleModel actual = Target.GenerateEditViewModel(1);

            var actualList = actual.Schools.Items.Cast<School>().Select(s => s.Name).ToList();
            CollectionAssert.AreEqual(expected, actualList);
        }

        [TestMethod]
        public void GivenNullViewModel_WhenCreate_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Create(null, User));
        }

        [TestMethod]
        public void WhenCreate_ThenChangesSaved()
        {
            var viewModel = new UserRoleModel
            {
                UserId = 3,
                Comments = "Test Comments",
                SelectedSchoolIds = new List<int> { 1 },
                SelectedProviderIds = new List<int>(),
                PostedRoles = new int[] { 2 }
            };
            AddAdditionalUsers();

            Target.Create(viewModel, User);

            Repositories.MockRepositoryContainer.AssertWasCalled(u => u.Save());
        }

        [TestMethod]
        public void WhenCreate_ThenChangesSaved_AndUserRoleIsUpdatedCorrectly()
        {
            AddAdditionalUsers();
            var expectedUser = Repositories.MockUserRepository.Items.Where(u => u.Id == 1).SingleOrDefault();
            var expectedRole = Repositories.MockRoleRepository.Items.Where(r => r.Id == 1).SingleOrDefault();
            var expectedComments = "Test Comments";
            var expectedSchools = Repositories.MockSchoolRepository.Items.Where(s => s.Id == 1).ToList();
            var viewModel = new UserRoleModel
            {
                UserId = 3,
                Comments = expectedComments,
                SelectedProviderIds = new List<int>(),
                SelectedSchoolIds = new List<int> { 1 },
                PostedRoles = new int[] { 2 }
            };

            Target.Create(viewModel, User);

            Repositories.MockUserRoleRepository.AssertWasCalled(u => u.Add(null), options => options.IgnoreArguments().Do(new Action<UserRole>(u =>
            {
                Assert.AreEqual(expectedUser, u.User);
                Assert.AreEqual(expectedRole, u.Role);
                Assert.AreEqual(expectedSchools, u.Schools);
            })));
            Repositories.MockRepositoryContainer.AssertWasCalled(u => u.Save());
        }

        [TestMethod]
        public void GivenUserRoleWithProvider_WhenCreate_ThenChangesSaved_AndAddedToUserRole()
        {
            AddAdditionalUsers();
            var expectedUser = Repositories.MockUserRepository.Items.Where(u => u.Id == 1).SingleOrDefault();
            var expectedRole = Repositories.MockRoleRepository.Items.Where(r => r.Name.Equals(SecurityRoles.Provider)).SingleOrDefault();
            var expectedComments = "Test Comments";
            var expectedProviders = Repositories.MockProviderRepository.Items.Where(p => p.Id == 1).SingleOrDefault();
            var expectedSchools = new List<School>();
            var viewModel = new UserRoleModel
            {
                UserId = 2,
                Comments = expectedComments,
                PostedRoles = new int[] { expectedRole.Id },
                SelectedProviderIds = new List<int> { 1 }
            };

            Target.Create(viewModel, User);

            Repositories.MockUserRoleRepository.AssertWasCalled(u => u.Add(null), options => options.IgnoreArguments().Do(new Action<UserRole>(u =>
            {
                Assert.AreEqual(expectedUser, u.User);
                Assert.AreEqual(expectedRole, u.Role);
                Assert.AreEqual(expectedProviders, u.Providers);
                Assert.AreEqual(true, u.User.Active);
            })));
            Repositories.MockRepositoryContainer.AssertWasCalled(u => u.Save());
        }

        [TestMethod]
        public void GivenUserRoleWithSiteCoordinatorAndNoPostedSchools_WhenCreate_ThenChangesSaved_AndAllSchoolsAreAddedToUserRole()
        {
            AddAdditionalUsers();
            var expectedUser = Repositories.MockUserRepository.Items.Where(u => u.Id == 1).SingleOrDefault();
            var expectedRole = Repositories.MockRoleRepository.Items.Where(r => r.Id == 1).SingleOrDefault();
            var expectedComments = "Test Comments";
            var expectedSchools = Repositories.MockSchoolRepository.Items.ToList();
            var viewModel = new UserRoleModel
            {
                UserId = 3,
                Comments = expectedComments,
                PostedRoles = new int[] { 2 },
                allSchoolsSelected = true
            };

            Target.Create(viewModel, User);

            Repositories.MockUserRoleRepository.AssertWasCalled(u => u.Add(null), options => options.IgnoreArguments().Do(new Action<UserRole>(u =>
            {
                Assert.AreEqual(expectedUser, u.User);
                Assert.AreEqual(expectedRole, u.Role);
                Assert.AreEqual(expectedSchools, u.Schools);
                Assert.AreEqual(true, u.User.Active);
            })));
            Repositories.MockRepositoryContainer.AssertWasCalled(u => u.Save());
        }

        [TestMethod]
        public void GivenUserRoleWithUserUserRole_WhenCreate_ThenCreatedUserRoleHasExpectedCreateTime()
        {
            AddAdditionalUsers();
            var viewModel = new UserRoleModel
            {
                UserId = 3,
                Comments = string.Empty,
                PostedRoles = new int[] { 2 }
            };

            Target.Create(viewModel, User);

            DateTime completeTime = DateTime.Now;
            TimeSpan maxDelta = TimeSpan.FromMilliseconds(5);
            Repositories.MockUserRoleRepository.AssertWasCalled(m => m.Add(Arg<UserRole>.Matches(u => u.CreateTime.WithinTimeSpanOf(maxDelta, completeTime))));
        }

        [TestMethod]
        public void GivenViewModel_WhenCreate_ThenUserRoleHasCreatingUserInfo()
        {
            AddAdditionalUsers();
            var viewModel = new UserRoleModel
            {
                UserId = 3,
                PostedRoles = new int[] { 2 }
            };

            Target.Create(viewModel, User);

            Repositories.MockUserRoleRepository.AssertWasCalled(m => m.Add(Arg<UserRole>.Matches(u => u.CreatingUser == User.Identity.User)));
        }

        [TestMethod]
        public void GivenViewModelHasNullPostedRoles_WhenCreate_ThenThrowValidationException()
        {
            AddAdditionalUsers();
            var viewModel = new UserRoleModel { UserId = 977878787, PostedRoles = null };

            Target.ExpectException<ValidationException>(() => Target.Create(viewModel, User));
        }

        [TestMethod]
        public void GivenViewModel_WhenCreate_ThenAuditorCreatesChangeEvent_AndEventAddedToRepository()
        {
            AddAdditionalUsers();
            var viewModel = new UserRoleModel
            {
                UserId = 3,
                PostedRoles = new int[] { 2 }
            };
            var expectedAuditEvent = new UserAccessChangeEvent();
            MockUserAuditor.Expect(m => m.CreateAccessChangeEvent(Data.Users.Single(u => u.Id == viewModel.UserId), User.Identity.User)).Return(expectedAuditEvent);

            Target.Create(viewModel, User);

            Repositories.MockUserAccessChangeEventRepository.AssertWasCalled(m => m.Add(expectedAuditEvent));
        }

        [TestMethod]
        public void GivenUserHasMultipleRoles_WhenEditUserProfile_ThenErrorThrown()
        {
            User notExpected = Users[3];
            var expectedState = new UserRoleModel { UserId = notExpected.Id, PostedRoles = notExpected.UserRoles.Select(u => u.Id) };

            TestExtensions.ExpectException<ValidationException>(() => Target.Edit(expectedState, User));
        }

        [TestMethod]
        public void WhenEditRoleIsPosted_ThenJsonResultReturned()
        {
            AddAdditionalUsers();
            var viewModel = new UserRoleModel { UserId = 1, PostedRoles = new int[] { } };

            Target.Edit(viewModel, User);

            Repositories.MockRepositoryContainer.AssertWasCalled(u => u.Save());
        }

        [TestMethod]
        public void WhenEditRoleIsPostedWithSchoolsAsSiteCoordinator_ThenJsonResultReturned()
        {
            int userId = 1;
            var expectedUpdate = Data.Users.Single(u => u.Id == userId);
            int newRoleId = 2;
            var expectedRemove = expectedUpdate.UserRoles.Single(ur => ur.RoleId != newRoleId);
            var viewModel = new UserRoleModel
            {
                UserId = userId,
                Comments = "Testing",
                SelectedSchoolIds = Repositories.MockSchoolRepository.Items.Select(s => s.Id).ToList(),
                PostedRoles = new int[] { newRoleId }
            };
            AddAdditionalUsers();

            Target.Edit(viewModel, User);

            Repositories.MockUserRoleRepository.AssertWasCalled(u => u.Remove(expectedRemove));
            Repositories.MockUserRepository.AssertWasCalled(u => u.Update(expectedUpdate));
            Repositories.MockRepositoryContainer.AssertWasCalled(u => u.Save());
        }

        [TestMethod]
        public void GivenRoleAssignmentDoesNotChange_WhenEditRoleIsPostedWithSchoolsAsSiteCoordinator_ThenUserRoleMappingAudited()
        {
            int userId = 1;
            var expectedUpdate = Data.Users.Single(u => u.Id == userId).UserRoles.Single();
            var viewModel = new UserRoleModel
            {
                UserId = userId,
                Comments = "Testing",
                PostedRoles = new int[] { expectedUpdate.RoleId }
            };
            AddAdditionalUsers();

            Target.Edit(viewModel, User);

            Assert.AreEqual(User.Identity.User, expectedUpdate.LastModifyingUser);
            Assert.IsTrue(expectedUpdate.LastModifyTime.Value.WithinTimeSpanOf(TimeSpan.FromSeconds(1), DateTime.Now));
        }

        [TestMethod]
        public void WhenEditRolePostedWithoutSchoolsAsSiteCoordinator_ThenJsonResultReturned()
        {
            int userId = 1;
            var expectedUpdate = Data.Users.Single(u => u.Id == userId);
            int newRoleId = 2;
            var expectedRemove = expectedUpdate.UserRoles.Single(ur => ur.RoleId != newRoleId);
            var viewModel = new UserRoleModel
            {
                UserId = userId,
                Comments = "Testing",
                PostedRoles = new int[] { newRoleId }
            };
            AddAdditionalUsers();

            Target.Edit(viewModel, User);

            Repositories.MockUserRoleRepository.AssertWasCalled(u => u.Remove(expectedRemove));
            Repositories.MockUserRepository.AssertWasCalled(u => u.Update(expectedUpdate));
            Repositories.MockRepositoryContainer.AssertWasCalled(u => u.Save());
        }

        [TestMethod]
        public void WhenEditRolePostedWithUserWhoHasNoUserRoles_ThenJsonResultReturned()
        {
            AddAdditionalUsers();
            var viewModel = new UserRoleModel
            {
                UserId = 5,
                Comments = "Testing",
                PostedRoles = new int[] { 2 }
            };

            Target.Edit(viewModel, User);

            Repositories.MockUserRoleRepository.AssertWasCalled(u => u.Add(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasCalled(u => u.Save());
        }

        [TestMethod]
        public void GivenUserRemovesProvider_WhenIEditRoles_ThenJsonResultReturned()
        {
            UserRoleModel viewModel = new UserRoleModel
            {
                UserId = 3,
                Comments = "Testing",
                PostedRoles = new int[] { Repositories.MockRoleRepository.Items.Where(m => m.Name.Equals(SecurityRoles.Provider)).Select(p => p.Id).SingleOrDefault() },
                SelectedProviderIds = new List<int> { 1 }
            };
            AddAdditionalUsers();

            Target.Edit(viewModel, User);

            Repositories.MockUserRoleRepository.AssertWasCalled(u => u.DeleteLink(Data.UserRoles[2], Data.Providers[2]));
            Repositories.MockRepositoryContainer.AssertWasCalled(u => u.Save());
        }

        [TestMethod]
        public void WhenIRemoveAllOfMyUserRoles_ThenIWillNotHaveAnyUserRoles()
        {
            var users = Data.Users;
            var userId = users[0].Id;
            var viewModel = new UserRoleModel
            {
                UserId = userId
            };
            var expectedRemove = users[0].UserRoles.Single();
            Repositories.MockUserRepository.Expect(m => m.Items).Return(users.AsQueryable());

            Target.Edit(viewModel, User);

            Repositories.MockUserRoleRepository.AssertWasCalled(u => u.Remove(expectedRemove));
        }

        [TestMethod]
        public void GivenViewModel_WhenEdit_ThenAuditorCreatesChangeEvent_AndEventAddedToRepository()
        {
            AddAdditionalUsers();
            var viewModel = new UserRoleModel
            {
                UserId = 3,
                PostedRoles = new int[] { 2 }
            };
            var expectedAuditEvent = new UserAccessChangeEvent();
            MockUserAuditor.Expect(m => m.CreateAccessChangeEvent(Data.Users.Single(u => u.Id == viewModel.UserId), User.Identity.User)).Return(expectedAuditEvent);

            Target.Edit(viewModel, User);

            Repositories.MockUserAccessChangeEventRepository.AssertWasCalled(m => m.Add(expectedAuditEvent));
        }

        [TestMethod]
        public void GivenNullViewModel_WhenPopulateViewModelIsCalled_ThenThrowNewArgumentNullException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.PopulateViewModel(null));
        }

        [TestMethod]
        public void GivenViewModel_WhenPopulateViewModelIsCalled_ThenListsArePopulatedOrderedByName()
        {
            var expectedSchools = Repositories.MockSchoolRepository.Items.OrderBy(s => s.Name);
            var actual = new UserRoleModel();

            Target.PopulateViewModel(actual);

            Assert.AreEqual(Repositories.MockRoleRepository.Items, actual.AvailableRoles);
            CollectionAssert.AreEqual(expectedSchools.ToList(), actual.Schools.Items.Cast<School>().ToList());
        }

        [TestMethod]
        public void WhenGenerateListViewModelIsCalled_ThenAViewModelIsReturned()
        {
            var actual = Target.GenerateListViewModel() as UserModel;

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenBinderCreatesResult_WhenGenerateDataTableResultViewModel_ThenReturnBindResult()
        {
            DataTableResultModel expected = new DataTableResultModel();
            DataTableRequestModel requestModel = new DataTableRequestModel();
            IClientDataTable<User> dataTable = MockRepository.GenerateMock<IClientDataTable<User>>();
            MockDataTableBinder.Expect(m => m.Bind(Repositories.MockUserRepository.Items, dataTable, requestModel)).Return(expected);

            DataTableResultModel actual = Target.GenerateDataTableResultViewModel(requestModel, dataTable);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenClientDataTableReturnsUsersAfterFilterAndSort_WhenGetFilteredUserIds_ThenReturnIdsFromUsers()
        {
            var expected = new[] { 3, 1, 2 };
            IClientDataTable<User> dataTable = MockRepository.GenerateMock<IClientDataTable<User>>();
            dataTable.Expect(m => m.ApplyFilters(Repositories.MockUserRepository.Items)).Return(Repositories.MockUserRepository.Items);
            dataTable.Expect(m => m.ApplySort(Repositories.MockUserRepository.Items)).Return(Repositories.MockUserRepository.Items.Where(u => expected.Contains(u.Id)));

            var actual = Target.GetFilteredUserIds(dataTable);

            CollectionAssert.AreEquivalent(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenAValidUserId_WhenIGenerateUserAssociationsViewModel_ThenAViewModelIsReturned()
        {
            int id = 1;
            AddAdditionalUsers();

            var result = Target.GenerateUserAssociationsViewModel(id);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenAValidUserId_AndUserHasAssociatedSchools_WhenIGenerateUserAssociationsViewModel_ThenViewModelContainsSchoolNames_AndSchoolNamesAlphabetical()
        {
            string[] expected = new[] { "Evergreen High School", "Wyoming High School" };
            int id = 2;

            var result = Target.GenerateUserAssociationsViewModel(id);

            CollectionAssert.AreEqual(expected, result.SchoolNames.ToList());
        }

        [TestMethod]
        public void GivenAValidUserId_AndUserHasAssociatedProviders_WhenIGenerateUserAssociationsViewModel_ThenViewModelContainsProviderNames_AndProviderNamesAlphabetical()
        {
            string[] expected = new[] { "Big Brothers, Big Sisters", "YMCA" };
            int id = 3;

            var result = Target.GenerateUserAssociationsViewModel(id);

            CollectionAssert.AreEqual(expected, result.ProviderNames.ToList());
        }

        [TestMethod]
        public void GivenAnInValidUserId_WhenIGenerateUserAssociationsViewModel_ThenThrowEntityNotFound()
        {
            AddAdditionalUsers();

            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateUserAssociationsViewModel(35331));
        }

        [TestMethod]
        public void GivenInvalidUserId_WhenUpdateActiveStatus_ThenThrowException()
        {
            AddAdditionalUsers();

            Target.ExpectException<EntityNotFoundException>(() => Target.UpdateActiveStatus(287, true, User));
        }

        [TestMethod]
        public void WhenUpdateActiveStatus_ThenAnActionResultIsReturned()
        {
            var expected = Data.Users[1];
            expected.Active = true;
            AddAdditionalUsers();

            Target.UpdateActiveStatus(2, true, User);

            Repositories.MockUserRepository.AssertWasCalled(u => u.Update(expected));
            Repositories.MockRepositoryContainer.AssertWasCalled(u => u.Save());
        }

        [TestMethod]
        public void WhenUpdateActiveStatus_ThenUserIsUpdated_AndSaved()
        {
            AddAdditionalUsers();

            Target.UpdateActiveStatus(1, true, User);

            Repositories.MockUserRepository.AssertWasCalled(u => u.Update(Data.Users[0]));
            Repositories.MockRepositoryContainer.AssertWasCalled(u => u.Save());
        }

        [TestMethod]
        public void WhenUpdateActiveStatus_ThenAuditorCreatesChangeEvent_AndEventAddedToRepository()
        {
            AddAdditionalUsers();
            int id = 1;
            var expectedAuditEvent = new UserAccessChangeEvent();
            MockUserAuditor.Expect(m => m.CreateAccessChangeEvent(Data.Users.Single(u => u.Id == id), User.Identity.User)).Return(expectedAuditEvent);

            Target.UpdateActiveStatus(id, true, User);

            Repositories.MockUserAccessChangeEventRepository.AssertWasCalled(m => m.Add(expectedAuditEvent));
        }

        [TestMethod]
        public void GivenInvalidUserIds_WhenUpdateActiveStatus_ThenThrowException()
        {
            AddAdditionalUsers();

            Target.ExpectException<EntityNotFoundException>(() => Target.UpdateActiveStatus(new[] { 287, 6354 }, true, User));
        }

        [TestMethod]
        public void WhenUpdateActiveStatus_ThenUsersAreUpdated()
        {
            AddAdditionalUsers();

            Target.UpdateActiveStatus(new List<int> { 1, 2 }, true, User);

            Repositories.MockUserRepository.AssertWasCalled(u => u.Update(Data.Users[0]));
            Repositories.MockUserRepository.AssertWasCalled(u => u.Update(Data.Users[1]));
            Repositories.MockRepositoryContainer.AssertWasCalled(u => u.Save());
        }

        [TestMethod]
        public void WhenUpdateActiveStatus_ThenAuditorCreatesChangeEvents_AndEventsAddedToRepository()
        {
            AddAdditionalUsers();
            var idEventDictionary = new Dictionary<int, UserAccessChangeEvent>
            {
                { 1, new UserAccessChangeEvent() },
                { 2, new UserAccessChangeEvent() }
            };
            foreach (var item in idEventDictionary)
            {
                MockUserAuditor.Expect(m => m.CreateAccessChangeEvent(Data.Users.Single(u => u.Id == item.Key), User.Identity.User)).Return(item.Value);
            }
            
            Target.UpdateActiveStatus(new List<int> { 1, 2 }, true, User);

            foreach (var value in idEventDictionary.Values)
            {
                Repositories.MockUserAccessChangeEventRepository.AssertWasCalled(m => m.Add(value));
            }
        }

        [TestMethod]
        public void WhenIGenerateMultiUserActivationViewModel_ThenAMultiUserActivationViewModelIsReturned()
        {
            var model = Target.GenerateMultiUserActivationViewModel(new List<int> { 1 }, true, "test");
            Assert.IsNotNull(model);
            CollectionAssert.AreEqual(new List<int> { 1 }, model.UserIds.ToList());
            Assert.AreEqual(true, model.ActiveFlag);
            Assert.AreEqual("test", model.ActivationString);
        }

        [TestMethod]
        public void GivenId_WhenGenerateUserAccessChangeEventModel_ThenUserModelReturned()
        {
            var result = Target.GenerateUserAccessChangeEventModel(1) as UserModel;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenId_WhenGenerateUserAccessChangeEventModel_ThenExpectedModelReturned()
        {
            var user = Data.Users[0];
            UserModel expected = new UserModel();
            expected.CopyFrom(user);

            UserModel actual = Target.GenerateUserAccessChangeEventModel(user.Id);

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.DisplayName, actual.DisplayName);
        }

        [TestMethod]
        public void GivenInvalidId_WhenGenerateUserAccessChangeEventModel_ThenExceptionThrown()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateUserAccessChangeEventModel(600));
        }

        [TestMethod]
        public void GivenNullRequestModel_WhenGenerateAuditAccessDataTableResultViewModel_ThenThrowException()
        {
            IClientDataTable<UserAccessChangeEvent> dataTable = MockRepository.GenerateMock<IClientDataTable<UserAccessChangeEvent>>();

            Target.ExpectException<ArgumentNullException>(() => Target.GenerateAuditAccessDataTableResultViewModel(null, dataTable));
        }

        [TestMethod]
        public void GivenBinderCreatesResult_WhenGenerateAuditAccessDataTableResultViewModel_ThenReturnBindResult()
        {
            DataTableResultModel expected = new DataTableResultModel();
            DataTableRequestModel requestModel = new DataTableRequestModel();
            IClientDataTable<UserAccessChangeEvent> dataTable = MockRepository.GenerateMock<IClientDataTable<UserAccessChangeEvent>>();
            MockDataTableBinder.Expect(m => m.Bind(Repositories.MockUserAccessChangeEventRepository.Items, dataTable, requestModel)).Return(expected);

            DataTableResultModel actual = Target.GenerateAuditAccessDataTableResultViewModel(requestModel, dataTable);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenNullUser_WhenAuditLogin_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => Target.AuditLogin(null));
        }

        [TestMethod]
        public void GivenUser_WhenAuditLogin_ThenEventAddedToRepository_AndRepositorySaved()
        {
            LoginEvent expected = new LoginEvent();
            MockUserAuditor.Expect(m => m.CreateLoginEvent(User.Identity.User)).Return(expected);

            Target.AuditLogin(User);

            Repositories.MockLoginEventRepository.AssertWasCalled(m => m.Add(expected));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenId_WhenGenerateUserLoginEventModel_ThenUserModelReturned()
        {
            var result = Target.GenerateUserLoginEventModel(1) as UserModel;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenId_WhenGenerateUserLoginEventModel_ThenExpectedModelReturned()
        {
            var user = Data.Users[0];
            UserModel expected = new UserModel();
            expected.CopyFrom(user);

            UserModel actual = Target.GenerateUserLoginEventModel(user.Id);

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.DisplayName, actual.DisplayName);
        }

        [TestMethod]
        public void GivenInvalidId_WhenGenerateUserLoginEventModel_ThenExceptionThrown()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateUserLoginEventModel(600));
        }

        [TestMethod]
        public void GivenNullRequestModel_WhenGenerateAuditLoginDataTableResultViewModel_ThenThrowException()
        {
            IClientDataTable<LoginEvent> dataTable = MockRepository.GenerateMock<IClientDataTable<LoginEvent>>();

            Target.ExpectException<ArgumentNullException>(() => Target.GenerateAuditLoginDataTableResultViewModel(null, dataTable));
        }

        [TestMethod]
        public void GivenBinderCreatesResult_WhenGenerateAuditLoginDataTableResultViewModel_ThenReturnBindResult()
        {
            DataTableResultModel expected = new DataTableResultModel();
            DataTableRequestModel requestModel = new DataTableRequestModel();
            IClientDataTable<LoginEvent> dataTable = MockRepository.GenerateMock<IClientDataTable<LoginEvent>>();
            MockDataTableBinder.Expect(m => m.Bind(Repositories.MockLoginEventRepository.Items, dataTable, requestModel)).Return(expected);

            DataTableResultModel actual = Target.GenerateAuditLoginDataTableResultViewModel(requestModel, dataTable);

            Assert.AreEqual(expected, actual);
        }

        private void AddAdditionalUsers()
        {
            Users.Add(new User { Id = 20, UserKey = "3u2e2" });
            Users.Add(new User { Id = 30, UserKey = "29e8r2fj", EmailAddress = "bob@bob.bob", DisplayName = "bob" });
            Users.Add(new User { Id = 40, UserKey = "w8iw2j2" });
        }
    }
}
