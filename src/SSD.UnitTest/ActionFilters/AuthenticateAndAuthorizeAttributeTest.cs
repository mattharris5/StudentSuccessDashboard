using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.Domain;
using SSD.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace SSD.ActionFilters
{
    [TestClass]
    public class AuthenticateAndAuthorizeAttributeTest
    {
        private IAccountManager MockAccountManager { get; set; }
        private IDependencyResolver MockDependecyResolver { get; set; }
        private AuthenticateAndAuthorizeAttribute Target { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            MockAccountManager = MockRepository.GenerateMock<IAccountManager>();
            MockDependecyResolver = MockRepository.GenerateMock<IDependencyResolver>();
            MockDependecyResolver.Expect(m => m.GetService(typeof(IAccountManager))).Return(MockAccountManager);
            DependencyResolver.SetResolver(MockDependecyResolver);
            Target = new AuthenticateAndAuthorizeAttribute();
        }

        [TestMethod]
        public void GivenNullAuthorizationContext_WhenOnAuthorization_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.OnAuthorization(null));
        }

        [TestMethod]
        public void GivenAClaimsIdentityExists_AndUserIsAuthenticated_AndUserExists_WhenIAuthorize_ThenHttpContextUserIsSetToEducationPrincipal()
        {
            string userKey = "whatever";
            AuthorizationContext authorizationContext = CreateAuthorizationContext(false, userKey);
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("hsdjkfhdkjhsfkjhdkjsf");
            MockAccountManager.Expect(m => m.EnsureUserEntity(authorizationContext.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity)).Return(new User());

            Target.OnAuthorization(authorizationContext);

            EducationSecurityPrincipal actual = authorizationContext.HttpContext.User as EducationSecurityPrincipal;
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenAClaimsIdentityExistsOnRootLevel_AndUserIsAuthenticated_AndUserExists_WhenIAuthorize_ThenHttpContextUserIsSetToEducationPrincipal()
        {
            string userKey = "whatever";
            AuthorizationContext authorizationContext = CreateAuthorizationContext(false, userKey);
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("hsdjkfhdkjhsfkjhdkjsf");
            MockAccountManager.Expect(m => m.EnsureUserEntity(authorizationContext.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity)).Return(new User());

            Target.OnAuthorization(authorizationContext);

            EducationSecurityPrincipal actual = authorizationContext.HttpContext.User as EducationSecurityPrincipal;
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenHttpContextUserIsAlreadySet_WhenIAuthorize_ThenHttpContextUserReused()
        {
            string userKey = "whatever";
            EducationSecurityPrincipal expected = new EducationSecurityPrincipal(new User { UserKey = userKey });
            AuthorizationContext authorizationContext = CreateAuthorizationContext(false, userKey);
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("hsdjkfhdkjhsfkjhdkjsf");
            authorizationContext.HttpContext.User = expected;

            Target.OnAuthorization(authorizationContext);

            Assert.AreSame(expected, authorizationContext.HttpContext.User);
        }

        [TestMethod]
        public void GivenAClaimsIdentityExists_AndUserIsAuthenticated_AndUserExists_WhenIAuthorize_ThenUserEntityIsSet()
        {
            string userKey = "whatever";
            AuthorizationContext authorizationContext = CreateAuthorizationContext(false, userKey);
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("hsdjkfhdkjhsfkjhdkjsf");
            MockAccountManager.Expect(m => m.EnsureUserEntity(authorizationContext.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity)).Return(new User { UserKey = userKey });

            Target.OnAuthorization(authorizationContext);

            Assert.IsNotNull(Target.UserEntity);
            Assert.AreEqual("whatever", Target.UserEntity.UserKey);
        }

        [TestMethod]
        public void GivenAClaimsIdentityExists_AndUserIsAuthenticated_AndUserExists_AndRequestIsForUserProfile_WhenIAuthorize_ThenResultIsNotRedirectResult()
        {
            string userKey = "whatever";
            AuthorizationContext authorizationContext = CreateAuthorizationContext(false, userKey);
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("/Account/UserProfile");
            MockAccountManager.Expect(m => m.EnsureUserEntity(authorizationContext.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity)).Return(new User());

            Target.OnAuthorization(authorizationContext);

            Assert.IsNotInstanceOfType(authorizationContext.Result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void GivenAClaimsIdentityExists_AndUserIsAuthenticated_AndUserExists_AndUserIsValid_WhenIAuthorize_ThenResultIsNotRedirectResult()
        {
            string userKey = "whatever";
            AuthorizationContext authorizationContext = CreateAuthorizationContext(false, userKey);
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("hsdjkfhdkjhsfkjhdkjsf");
            MockAccountManager.Expect(m => m.EnsureUserEntity(authorizationContext.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity)).Return(new User { UserKey = userKey, DisplayName = "not null", EmailAddress = "not null", FirstName = "not null", LastName = "not null" });
            
            Target.OnAuthorization(authorizationContext);

            Assert.IsNotInstanceOfType(authorizationContext.Result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void GivenAClaimsIdentityExists_AndUserIsAuthenticated_AndUserExists_AndUserDoesNotHaveRole_WhenIAuthorize_ThenResultIsUnauthorized()
        {
            string userKey = "whatever";
            AuthorizationContext authorizationContext = CreateAuthorizationContext(false, userKey);
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("hsdjkfhdkjhsfkjhdkjsf");
            MockAccountManager.Expect(m => m.EnsureUserEntity(authorizationContext.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity)).Return(new User());

            Target.Roles = "admin";
            Target.OnAuthorization(authorizationContext);

            ViewResult result = authorizationContext.Result as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Unauthorized", result.ViewName);
            authorizationContext.HttpContext.Response.AssertWasCalled(m => m.StatusCode = 401);
        }

        [TestMethod]
        public void GivenAClaimsIdentityExists_AndUserIsAuthenticated_AndUserExists_AndUserDoesNotHaveRole_WhenIAuthorize_ThenViewResultDataContainsRoles()
        {
            User expectedUser = new User { UserKey = "whatever", UserRoles = new List<UserRole> { new UserRole { Role = new Role { Name = "supervisor" } } } };
            AuthorizationContext authorizationContext = CreateAuthorizationContext(false, expectedUser.UserKey);
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("hsdjkfhdkjhsfkjhdkjsf");
            MockAccountManager.Expect(m => m.EnsureUserEntity(authorizationContext.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity)).Return(expectedUser);

            Target.Roles = "admin";
            Target.OnAuthorization(authorizationContext);

            ViewResult result = authorizationContext.Result as ViewResult;
            IEnumerable<string> viewModel = result.AssertGetViewModel<IEnumerable<string>>();
            CollectionAssert.AreEqual(expectedUser.UserRoles.Select(m => m.Role.Name).ToList(), viewModel.ToList());
        }

        [TestMethod]
        public void GivenAClaimsIdentityExists_AndUserIsAuthenticated_AndUserExists_AndUserHasRole_WhenIAuthorize_ThenResultIsNotUnauthorized()
        {
            string userKey = "whatever";
            AuthorizationContext authorizationContext = CreateAuthorizationContext(false, userKey);
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("hsdjkfhdkjhsfkjhdkjsf");
            MockAccountManager.Expect(m => m.EnsureUserEntity(authorizationContext.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity)).Return(new User());

            Target.Roles = "supervisor";
            Target.OnAuthorization(authorizationContext);

            Assert.IsNotInstanceOfType(authorizationContext.Result, typeof(HttpUnauthorizedResult));
        }

        [TestMethod]
        public void GivenAClaimsIdentityExists_AndUserIsAuthenticated_AndUserDoesNotExist_WhenIAuthorize_ThenRedirectedToProfile()
        {
            string userKey = "new user";
            AuthorizationContext authorizationContext = CreateAuthorizationContext(false, userKey);
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("hsdjkfhdkjhsfkjhdkjsf");
            MockAccountManager.Expect(m => m.EnsureUserEntity(authorizationContext.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity)).Return(new User { UserKey = userKey, DisplayName = User.AnonymousValue, EmailAddress = "Anonymous@sample.com" });

            Target.OnAuthorization(authorizationContext);

            RedirectToRouteResult result = authorizationContext.Result as RedirectToRouteResult;
            result.AssertActionRedirection("UserProfile", "Account");
        }

        [TestMethod]
        public void GivenAClaimsIdentityExists_AndUserIsAuthenticated_AndUserDoesNotExist_WhenIAuthorize_ThenUserIsCreatedWithValidState()
        {
            string userKey = "new user";
            AuthorizationContext authorizationContext = CreateAuthorizationContext(false, userKey);
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("hsdjkfhdkjhsfkjhdkjsf");
            MockAccountManager.Expect(m => m.EnsureUserEntity(authorizationContext.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity)).Return(new User { UserKey = userKey, DisplayName = User.AnonymousValue, EmailAddress = "Anonymous@sample.com" });

            Target.OnAuthorization(authorizationContext);

            User actualUser = Target.UserEntity;
            Assert.IsNotNull(actualUser);
            Assert.AreEqual("new user", actualUser.UserKey);
            Assert.AreEqual(User.AnonymousValue, actualUser.DisplayName);
            Assert.AreEqual(User.AnonymousEmailValue, actualUser.EmailAddress);
        }

        [TestMethod]
        public void GivenAClaimsIdentityExists_AndUserIsAuthenticated_AndUserDoesNotExist_WhenIAuthorize_ThenHttpContextUserIsSetToEducationPrincipal()
        {
            string userKey = "whatever";
            AuthorizationContext authorizationContext = CreateAuthorizationContext(false, userKey);
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("hsdjkfhdkjhsfkjhdkjsf");
            MockAccountManager.Expect(m => m.EnsureUserEntity(authorizationContext.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity)).Return(new User());

            Target.OnAuthorization(authorizationContext);

            EducationSecurityPrincipal actual = authorizationContext.HttpContext.User as EducationSecurityPrincipal;
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenNoClaimsIdentityExists_WhenIAuthenticate_ThenCorrectRedirectResponseIssued()
        {
            AuthorizationContext authorizationContext = CreateAuthorizationContext(true, "whatever");
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("hsdjkfhdkjhsfkjhdkjsf");

            WSFederationAuthenticationModule module = new WSFederationAuthenticationModule { Issuer = "http://blah.com", HomeRealm = "http://blah.com", Realm = "http://site.com" };
            IAuthenticationModuleProvider provider = MockRepository.GenerateMock<IAuthenticationModuleProvider>();
            provider.Expect(m => m.GetModule()).Return(module);
            AuthenticationUtility.SetModuleProvider(provider);

            Target.OnAuthorization(authorizationContext);
            Assert.IsInstanceOfType(authorizationContext.Result, typeof(RedirectToRouteResult));
            ((RedirectToRouteResult)authorizationContext.Result).AssertActionRedirection("Login", "Account");
        }

        [TestMethod]
        public void GivenNoClaimsIdentityExists_WhenIAuthenticate_ThenNoUserEntityIsSet()
        {
            AuthorizationContext authorizationContext = CreateAuthorizationContext(true, "whatever");
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("hsdjkfhdkjhsfkjhdkjsf");

            WSFederationAuthenticationModule module = new WSFederationAuthenticationModule { Issuer = "http://blah.com", HomeRealm = "http://blah.com", Realm = "http://site.com" };
            IAuthenticationModuleProvider provider = MockRepository.GenerateMock<IAuthenticationModuleProvider>();
            provider.Expect(m => m.GetModule()).Return(module);
            AuthenticationUtility.SetModuleProvider(provider);

            Target.OnAuthorization(authorizationContext);

            Assert.IsNull(Target.UserEntity);
        }

        [TestMethod]
        public void GivenAnAuthenticatedUserWasAlreadyAuthorized_AndNoClaimsIdentityExists_WhenIAuthenticate_ThenNoUserEntityIsSet()
        {
            string userKey = "whatever";
            AuthorizationContext authorizationContext = CreateAuthorizationContext(false, userKey);
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("hsdjkfhdkjhsfkjhdkjsf");
            MockAccountManager.Expect(m => m.EnsureUserEntity(authorizationContext.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity)).Return(new User());
            Target.OnAuthorization(authorizationContext);
            authorizationContext = CreateAuthorizationContext(true, userKey);
            authorizationContext.HttpContext.Request.Expect(m => m.FilePath).Return("hsdjkfhdkjhsfkjhdkjsf");

            WSFederationAuthenticationModule module = new WSFederationAuthenticationModule { Issuer = "http://blah.com", HomeRealm = "http://blah.com", Realm = "http://site.com" };
            IAuthenticationModuleProvider provider = MockRepository.GenerateMock<IAuthenticationModuleProvider>();
            provider.Expect(m => m.GetModule()).Return(module);
            AuthenticationUtility.SetModuleProvider(provider);

            Target.OnAuthorization(authorizationContext);

            Assert.IsNull(Target.UserEntity);
        }

        private static AuthorizationContext CreateAuthorizationContext(bool mockableContext, string userKey)
        {
            HttpContextBase httpContext = CreateHttpContextBase(mockableContext, userKey);
            return ControllerContextFactory.CreateAuthorizationContext(httpContext);
        }

        private static HttpContextBase CreateHttpContextBase(bool mockableContext, string userKey)
        {
            if (mockableContext)
            {
                HttpContextBase httpContext = MockHttpContextFactory.Create();
                httpContext.Expect(m => m.User).Return(MockRepository.GenerateMock<IPrincipal>());
                httpContext.User.Expect(m => m.Identity).Return(MockRepository.GenerateMock<IIdentity>());
                httpContext.User.Identity.Expect(m => m.IsAuthenticated).Return(false);
                httpContext.Request.Expect(m => m.Url).Return(new Uri("http://site.com/Bob/123"));
                httpContext.Request.Expect(m => m.RawUrl).Return("Bob/123");
                httpContext.Request.Headers.Add("Host", "site.com");
                return httpContext;
            }
            else
            {
                return new UserHttpContext(userKey);
            }
        }

        private class UserHttpContext : HttpContextBase
        {
            private HttpRequestBase _Request;
            private HttpResponseBase _Response;

            public UserHttpContext(string userKey)
            {
                var claim = new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userKey);
                var claimIdentity = new System.Security.Claims.ClaimsIdentity(new List<System.Security.Claims.Claim> { claim }, "Test");
                var identity = new System.Security.Claims.ClaimsIdentity[] { claimIdentity };
                var principal = new System.Security.Claims.ClaimsPrincipal(identity);
                User = principal;
                _Request = MockHttpContextFactory.CreateRequest();
                _Response = MockHttpContextFactory.CreateResponse();
            }

            public override HttpRequestBase Request
            {
                get { return _Request; }
            }

            public override HttpResponseBase Response
            {
                get { return _Response; }
            }

            public override IPrincipal User { get; set; }
        }
    }
}
