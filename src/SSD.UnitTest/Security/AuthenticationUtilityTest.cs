using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using System;
using System.Collections.Specialized;
using System.IdentityModel.Services;
using System.IO;
using System.Security.Principal;
using System.Web;

namespace SSD.Security
{
    [TestClass]
    public class AuthenticationUtilityTest
    {
        [TestMethod]
        public void GivenSpecifiedModuleProvider_WhenIGetCurrentProviderModule_ExpectedModuleReturned()
        {
            AuthenticationUtility.SetModuleProvider(new FakeModuleProvider());
            WSFederationAuthenticationModule actual = AuthenticationUtility.CurrentModuleProvider.GetModule();
            Assert.AreEqual(FakeModuleProvider.Singleton, actual);
        }

        [TestMethod]
        public void GivenNullHttpRequest_WhenIGetApplicationUri_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => AuthenticationUtility.GetApplicationUri(null));
        }

        [TestMethod]
        public void GivenHttpRequest_WhenIGetApplicationUri_ThenApplicationUriIsExtractedFromRequest()
        {
            HttpRequestBase request = MockRepository.GenerateMock<HttpRequestBase>();
            request.Expect(m => m.Url).Return(new Uri("http://bob.com/management/address"));
            request.Expect(m => m.ApplicationPath).Return("/management/");
            request.Expect(m => m.Headers).Return(new NameValueCollection());

            string actual = AuthenticationUtility.GetApplicationUri(request);
            Assert.AreEqual("http://bob.com/management/", actual);
        }

        [TestMethod]
        public void GivenHttpRequest_AndApplicationPathDoesNotTerminateWithSlash_WhenIGetApplicationUri_ThenApplicationUriIsExtractedFromRequest_AndApplicationUriIsterminatedWithSlash()
        {
            HttpRequestBase request = MockRepository.GenerateMock<HttpRequestBase>();
            request.Expect(m => m.Url).Return(new Uri("http://bob.com/management/address"));
            request.Expect(m => m.ApplicationPath).Return("/management");
            request.Expect(m => m.Headers).Return(new NameValueCollection());

            string actual = AuthenticationUtility.GetApplicationUri(request);
            Assert.AreEqual("http://bob.com/management/", actual);
        }

        [TestMethod]
        public void GivenNullHttpContext_WhenIGetContextPrincipal_ThenReturnNull()
        {
            HttpContext.Current = null;
            Assert.IsNull(AuthenticationUtility.GetHttpContextPrincipal());
        }

        [TestMethod]
        public void GivenHttpContextWithAlternatePrincipalType_WhenIGetContextPrincipal_ThenReturnNull()
        {
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));
            HttpContext.Current.User = new GenericPrincipal(new GenericIdentity("Bob"), new string[0]);
            Assert.IsNull(AuthenticationUtility.GetHttpContextPrincipal());
        }

        [TestMethod]
        public void GivenHttpContextWithNullUserPrincipal_WhenIGetContextPrincipal_ThenReturnNull()
        {
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));
            HttpContext.Current.User = null;
            Assert.IsNull(AuthenticationUtility.GetHttpContextPrincipal());
        }

        [TestMethod]
        public void GivenHttpWithCorrectPrincipalType_WhenIGetContextPrincipal_ThenPrincipalIsReturned()
        {
            EducationSecurityPrincipal expected = new EducationSecurityPrincipal(new User { UserKey = "sdklfjsw" });
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));
            HttpContext.Current.User = expected;
            EducationSecurityPrincipal actual = AuthenticationUtility.GetHttpContextPrincipal();
            Assert.AreEqual(expected, actual);
        }
    }
}
