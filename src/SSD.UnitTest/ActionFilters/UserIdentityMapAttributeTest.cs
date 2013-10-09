using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.Controllers;
using SSD.Domain;
using SSD.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace SSD.ActionFilters
{
    [TestClass]
    public class UserIdentityMapAttributeTest
    {
        private HttpContext MockHttpContext { get; set; }
        private HttpContextBase MockHttpContextBase { get; set; }
        private IAccountManager MockAccountManager { get; set; }
        private IDependencyResolver MockDependecyResolver { get; set; }
        private UserIdentityMapAttribute Target { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            MockHttpContext = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));
            MockHttpContextBase = new HttpContextWrapper(MockHttpContext);
            MockAccountManager = MockRepository.GenerateMock<IAccountManager>();
            MockDependecyResolver = MockRepository.GenerateMock<IDependencyResolver>();
            MockDependecyResolver.Expect(m => m.GetService(typeof(IAccountManager))).Return(MockAccountManager);
            DependencyResolver.SetResolver(MockDependecyResolver);
            HttpContext.Current = MockHttpContext;
            Target = new UserIdentityMapAttribute();
        }

        [TestMethod]
        public void GivenNullAuthorizationContext_WhenOnAuthorization_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.OnAuthorization(null));
        }

        [TestMethod]
        public void GivenNullContext_WhenMapFrom_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => UserIdentityMapAttribute.MapFrom(null));
        }

        [TestMethod]
        public void GivenPrincipalIsNotClaimsPrincipal_WhenMapFrom_ThenReturnNull()
        {
            MockHttpContext.User = MockRepository.GenerateMock<IPrincipal>();
            AuthorizationContext filterContext = ControllerContextFactory.CreateAuthorizationContext(MockHttpContextBase);

            EducationSecurityPrincipal actual = UserIdentityMapAttribute.MapFrom(filterContext);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GivenPrincipalIsNull_WhenMapFrom_ThenReturnNull()
        {
            AuthorizationContext filterContext = ControllerContextFactory.CreateAuthorizationContext(MockHttpContextBase);

            Assert.IsNull(UserIdentityMapAttribute.MapFrom(filterContext));
        }

        [TestMethod]
        public void GivenUserIsCorrectPrincipal_AndNameIdentifierClaimDoesNotMatchAnyUserKey_WhenAuthorizing_ThenContextUserUpdatedForNewUser()
        {
            string expectedUserKey = "random text";
            AuthorizationContext filterContext = CreateAuthorizationContextWithExpectations(expectedUserKey, new User { UserKey = expectedUserKey });

            Target.OnAuthorization(filterContext);

            EducationSecurityPrincipal actual = MockHttpContext.User as EducationSecurityPrincipal;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedUserKey, actual.Identity.User.UserKey);
        }

        [TestMethod]
        public void GivenUserIsCorrectPrincipalOnRootLevel_AndNameIdentifierClaimDoesNotMatchAnyUserKey_WhenAuthorizing_ThenContextUserUpdatedForNewUser()
        {
            string expectedUserKey = "random text";
            AuthorizationContext filterContext = CreateAuthorizationContextWithExpectations(expectedUserKey, new User { UserKey = expectedUserKey });

            Target.OnAuthorization(filterContext);

            EducationSecurityPrincipal actual = MockHttpContext.User as EducationSecurityPrincipal;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedUserKey, actual.Identity.User.UserKey);
        }

        [TestMethod]
        public void GivenUserIsCorrectPrincipal_AndNameIdentifierClaimDoesNotMatchAnyUserKey_WhenAuthorizing_ThenNewUserHasExpectedCreateTime()
        {
            string expectedUserKey = "random text";
            User expectedUser = new User { CreateTime = DateTime.Now };
            AuthorizationContext filterContext = CreateAuthorizationContextWithExpectations(expectedUserKey, expectedUser);

            Target.OnAuthorization(filterContext);

            EducationSecurityPrincipal actual = MockHttpContext.User as EducationSecurityPrincipal;
            Assert.IsNotNull(actual);
            Assert.AreEqual((double)DateTime.Now.Ticks, (double)actual.Identity.User.CreateTime.Ticks, TimeSpan.FromMilliseconds(25).Ticks);
        }

        [TestMethod]
        public void GivenUserIsCorrectPrincipal_AndNameIdentifierClaimMatchesAUserKey_WhenAuthorizing_ThenContextUserUpdatedForExistingUser()
        {
            User expectedUserEntity = new User { UserKey = "whatever" };
            AuthorizationContext filterContext = CreateAuthorizationContextWithExpectations(expectedUserEntity.UserKey, expectedUserEntity);

            Target.OnAuthorization(filterContext);

            EducationSecurityPrincipal actual = MockHttpContext.User as EducationSecurityPrincipal;
            Assert.IsNotNull(actual);
            Assert.AreSame(expectedUserEntity, actual.Identity.User);
        }

        [TestMethod]
        public void GivenUserIsCorrectPrincipal_AndMissingNameIdentifier_WhenAuthorizing_ThenContextUserUpdatedForExistingUser()
        {
            AuthorizationContext filterContext = CreateAuthorizationContextWithExpectations(null, null);

            Target.OnAuthorization(filterContext);

            EducationSecurityPrincipal actual = MockHttpContext.User as EducationSecurityPrincipal;
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GivenLogicManagerThrowsLicenseAgreementException_WhenAuthorizing_ThenRedirectToLicenseAgreement()
        {
            string userKey = "whatever";
            MockHttpContext.User = CreateClaimsPrincipal(userKey);
            AuthorizationContext filterContext = ControllerContextFactory.CreateAuthorizationContext(MockHttpContextBase);
            MockAccountManager.Expect(m => m.EnsureUserEntity(filterContext.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity)).Throw(new LicenseAgreementException());

            Target.OnAuthorization(filterContext);

            var actual = filterContext.Result as RedirectToRouteResult;
            actual.AssertActionRedirection("Index", "Agreement");
        }

        [TestMethod]
        public void GivenAuditOnMapCalled_AndClaimsPrincipalMatches_WhenAuthorizing_ThenCallAuditLoginOnAccountManager()
        {
            User expectedUserEntity = new User { UserKey = "whatever" };
            AuthorizationContext filterContext = CreateAuthorizationContextWithExpectations(expectedUserEntity.UserKey, expectedUserEntity);
            UserIdentityMapAttribute.AuditOnMap(expectedUserEntity.UserKey);

            Target.OnAuthorization(filterContext);

            EducationSecurityPrincipal actual = MockHttpContext.User as EducationSecurityPrincipal;
            MockAccountManager.AssertWasCalled(m => m.AuditLogin(actual));
        }

        [TestMethod]
        public void GivenAuditOnMapCalled_AndClaimsPrincipalMisMatches_WhenAuthorizing_ThenDoNotCallAuditLoginOnAccountManager()
        {
            User expectedUserEntity = new User { UserKey = "whatever" };
            AuthorizationContext filterContext = CreateAuthorizationContextWithExpectations(expectedUserEntity.UserKey, expectedUserEntity);
            UserIdentityMapAttribute.AuditOnMap("something else");

            Target.OnAuthorization(filterContext);

            MockAccountManager.AssertWasNotCalled(m => m.AuditLogin(null), o => o.IgnoreArguments());
        }

        [TestMethod]
        public void GivenAuditOnMapCalled_AndClaimsPrincipalMatches_AndUserAuthorized_WhenAuthorizing_ThenDoNotRepeatCallToAuditLoginOnAccountManager()
        {
            User expectedUserEntity = new User { UserKey = "whatever" };
            AuthorizationContext filterContext = CreateAuthorizationContextWithExpectations(expectedUserEntity.UserKey, expectedUserEntity);
            System.Security.Claims.ClaimsPrincipal claimsPrincipal = MockHttpContext.User as System.Security.Claims.ClaimsPrincipal;
            UserIdentityMapAttribute.AuditOnMap(expectedUserEntity.UserKey);
            Target.OnAuthorization(filterContext);
            filterContext.HttpContext.User = claimsPrincipal;

            Target.OnAuthorization(filterContext);

            EducationSecurityPrincipal actual = MockHttpContext.User as EducationSecurityPrincipal;
            MockAccountManager.AssertWasCalled(m => m.AuditLogin(null), o => o.IgnoreArguments().Repeat.Once());
        }

        [TestMethod]
        public void GivenActionInvokedIsAccountLogOff_AndUserIsNotAnEducationSecurityPrincipal_AndEulaNotAccepted_WhenAuthorizing_ThenSucceed()
        {
            User expectedUserEntity = new User { UserKey = "whatever" };
            MockHttpContext.User = CreateClaimsPrincipal(expectedUserEntity.UserKey);
            AuthorizationContext filterContext = ControllerContextFactory.CreateAuthorizationContext(MockHttpContextBase, typeof(AccountController), "LogOff");
            System.Security.Claims.ClaimsPrincipal claimsPrincipal = MockHttpContext.User as System.Security.Claims.ClaimsPrincipal;
            filterContext.HttpContext.User = claimsPrincipal;
            MockAccountManager.Expect(m => m.EnsureUserEntity((System.Security.Claims.ClaimsIdentity)claimsPrincipal.Identity)).Throw(new LicenseAgreementException());

            Target.OnAuthorization(filterContext);
        }

        private AuthorizationContext CreateAuthorizationContextWithExpectations(string expectedUserKey, User expectedUser)
        {
            MockHttpContext.User = CreateClaimsPrincipal(expectedUserKey);
            AuthorizationContext filterContext = ControllerContextFactory.CreateAuthorizationContext(MockHttpContextBase);
            MockAccountManager.Expect(m => m.EnsureUserEntity(filterContext.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity)).Return(expectedUser);
            return filterContext;
        }

        private static System.Security.Claims.ClaimsPrincipal CreateClaimsPrincipal(string userKey)
        {
            System.Security.Claims.ClaimsIdentity claimIdentity;
            if (userKey == null)
            {
                claimIdentity = new System.Security.Claims.ClaimsIdentity();
            }
            else
            {
                var claim = new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userKey);
                var claims = new List<System.Security.Claims.Claim> { claim };
                claimIdentity = new System.Security.Claims.ClaimsIdentity(claims, "Test");
            }
            var identity = new System.Security.Claims.ClaimsIdentity[] { claimIdentity };
            return new System.Security.Claims.ClaimsPrincipal(identity);
        }
    }
}
