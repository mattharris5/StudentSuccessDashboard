using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure;
using Rhino.Mocks;
using SSD.Business;
using SSD.Domain;
using SSD.Security;
using SSD.ViewModels;
using System;
using System.IdentityModel.Services;
using System.Security.Claims;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class AccountControllerTest : BaseControllerTest
    {
        private WSFederationAuthenticationModule MockAuthenticationModule { get; set; }
        private User TestUser { get; set; }
        private ClaimsIdentity Identity { get; set; }
        private IAccountManager MockAccountManager { get; set; }
        private AccountController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            FakeModuleProvider provider = new FakeModuleProvider();
            MockAuthenticationModule = MockRepository.GenerateMock<WSFederationAuthenticationModule>();
            provider.ModuleToReturn = MockAuthenticationModule;
            AuthenticationUtility.SetModuleProvider(provider);
            TestUser = new User { Id = 20, UserKey = "3u2e2" };
            Identity = new ClaimsIdentity();
            MockHttpContext.Request.Expect(m => m.RequestContext).Return(new RequestContext(MockHttpContext, new RouteData()));
            MockHttpContext.Request.Expect(m => m.Url).Return(new Uri("http://tempuri.org"));
            MockAccountManager = MockRepository.GenerateMock<IAccountManager>();
            Target = new AccountController(MockAccountManager);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
        }

        [TestMethod]
        public void GiveNullLogicManager_WhenIConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new AccountController(null));
        }

        [TestMethod]
        public void WhenIViewLogin_ThenViewResultReturned()
        {
            ViewResult actual = Target.Login();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void WhenIViewLogin_ThenViewModelRealmIsApplicationUrl()
        {
            string expected = AuthenticationUtility.GetApplicationUri(MockHttpContext.Request);

            ViewResult result = Target.Login();

            LoginModel actual = result.AssertGetViewModel<LoginModel>();
            Assert.AreEqual(expected, actual.Realm);
        }

        [TestMethod]
        public void WhenIViewLogin_ThenViewModelNamespaceIsFromConfiguration()
        {
            string expected = CloudConfigurationManager.GetSetting("AcsNamespace");

            ViewResult result = Target.Login();

            LoginModel actual = result.AssertGetViewModel<LoginModel>();
            Assert.AreEqual(expected, actual.Namespace);
        }

        [TestMethod]
        public void GivenUser_WhenIViewProfile_ThenViewResultReturned()
        {
            MockHttpContext.Expect(m => m.User).Return(new EducationSecurityPrincipal(new EducationSecurityIdentity(Identity, TestUser)));

            ViewResult actual = Target.UserProfile();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenUser_WhenIViewProfile_ThenViewHasViewModel()
        {
            UserModel expected = new UserModel();
            EducationSecurityPrincipal expectedUser = new EducationSecurityPrincipal(new EducationSecurityIdentity(Identity, TestUser));
            MockHttpContext.Expect(m => m.User).Return(expectedUser);
            MockAccountManager.Expect(m => m.GenerateUserProfileViewModel(expectedUser)).Return(expected);

            ViewResult actual = Target.UserProfile();

            actual.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenUser_WhenIViewProfile_ThenLogicManagerGeneratesViewModel()
        {
            EducationSecurityPrincipal expectedUser = new EducationSecurityPrincipal(new EducationSecurityIdentity(Identity, TestUser));
            MockHttpContext.Expect(m => m.User).Return(expectedUser);

            ViewResult actual = Target.UserProfile();

            MockAccountManager.AssertWasCalled(m => m.GenerateUserProfileViewModel(expectedUser));
        }

        [TestMethod]
        public void GivenUserIdentityIsInvalidType_WhenIViewProfile_ThenThrowException()
        {
            string userIdentifier = "thisisanewuser";
            Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userIdentifier));
            MockHttpContext.Expect(m => m.User).Return(new ClaimsPrincipal(new ClaimsIdentity[] { Identity }));

            Target.ExpectException<InvalidCastException>(() => Target.UserProfile());
        }

        [TestMethod]
        public void GivenInvalidUserProfile_WhenPostUserProfile_ThenViewResultHasViewModel()
        {
            UserModel expected = new UserModel();
            Target.ModelState.AddModelError("DisplayName", new Exception("this is a model binding exception"));
            var result = Target.UserProfile(expected) as ViewResult;
            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenUserProfile_WhenPostUserProfile_ThenRedirectToHome()
        {
            var result = Target.UserProfile(new UserModel { Id = TestUser.Id }) as RedirectToRouteResult;
            result.AssertActionRedirection("Index", "Home");
        }

        [TestMethod]
        public void GivenGuid_WhenIConfirmEmail_ThenGetView()
        {
            ViewResult actual = Target.ConfirmEmail(Guid.NewGuid());

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenGuid_WhenIConfirmEmail_ThenViewHasViewModel()
        {
            ConfirmEmailModel expected = new ConfirmEmailModel();
            Guid expectedIdentifier = Guid.NewGuid();
            MockAccountManager.Expect(m => m.GenerateConfirmEmailViewModel(expectedIdentifier)).Return(expected);

            ViewResult actual = Target.ConfirmEmail(expectedIdentifier);

            actual.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenGuid_WhenIConfirmEmail_ThenLogicManagerGeneratesViewModel()
        {
            Guid expectedIdentifier = Guid.NewGuid();

            Target.ConfirmEmail(expectedIdentifier);

            MockAccountManager.Expect(m => m.GenerateConfirmEmailViewModel(expectedIdentifier));
        }

        [TestMethod]
        public void WhenUserLogsOff_ThenRedirectToHomeIndex()
        {
            RedirectToRouteResult result = Target.LogOff();
            result.AssertActionRedirection("Index", "Home");
        }

        [TestMethod]
        public void WhenUserLogsOff_ThenSignOutCalledOnAuthenticationModule()
        {
            RedirectToRouteResult result = Target.LogOff();

            MockAuthenticationModule.AssertWasCalled(m => m.SignOut(false));
        }

        [TestMethod]
        public void WhenUserLogsOn_ThenSignInRequestRedirectCreated()
        {
            string expected = "http://site.com";

            RedirectResult result = Target.LogOn(expected);

            Assert.AreEqual(expected, result.Url);
        }

        [TestMethod]
        public void GivenHttpRequest_AndRealmFunctionReturnsValidUri_WhenUserLogsOn_ThenAllowedAudienceUrisContainsExpectedRealm()
        {
            Uri expected = new Uri("http://tempuri.org");

            Target.LogOn("http://harharhar.site.com");

            CollectionAssert.Contains(MockAuthenticationModule.FederationConfiguration.IdentityConfiguration.AudienceRestriction.AllowedAudienceUris, expected);
        }

        [TestMethod]
        public void GivenHttpRequest_AndRealmFunctionReturnsValidUri_AndSignInRequestAlreadyCreatedForRequest_WhenUserLogsOn_ThenAllowedAudienceUrisDoesNotContainDuplicate()
        {
            Target.LogOn("http://foobar.site.com");

            Target.LogOn("http://barfoo.site.com");

            CollectionAssert.AllItemsAreUnique(MockAuthenticationModule.FederationConfiguration.IdentityConfiguration.AudienceRestriction.AllowedAudienceUris);
        }
    }
}
