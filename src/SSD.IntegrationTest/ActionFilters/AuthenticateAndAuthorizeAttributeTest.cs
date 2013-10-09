using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.Data;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels.DataTables;
using System;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.ActionFilters
{
    [TestClass]
    public class AuthenticateAndAuthorizeAttributeTest
    {
        private EducationDataContext EducationDataContext { get; set; }
        private IWindsorContainer MockWindsorContainer { get; set; }
        private AccountManager AccountManager { get; set; }
        private IDependencyResolver MockDependecyResolver { get; set; }
        private TransactionScope _TestTransaction;

        [TestInitialize]
        public void InitializeTest()
        {
            _TestTransaction = new TransactionScope();
            try
            {
                IRepositoryContainer educationContainer = MockRepository.GenerateMock<IRepositoryContainer>();
                EducationDataContext = new EducationDataContext();
                educationContainer.Expect(m => m.Obtain<IUserRepository>()).Return(new UserRepository(EducationDataContext));
                educationContainer.Expect(m => m.Obtain<IEulaAgreementRepository>()).Return(new EulaAgreementRepository(EducationDataContext));
                MockWindsorContainer = MockRepository.GenerateMock<IWindsorContainer>();
                MockWindsorContainer.Expect(m => m.Resolve<IRepositoryContainer>()).Return(educationContainer);
                AccountManager = new AccountManager(MockWindsorContainer, MockRepository.GenerateMock<IEmailConfirmationManager>(), new DataTableBinder(), new UserAuditor());
                MockDependecyResolver = MockRepository.GenerateMock<IDependencyResolver>();
                MockDependecyResolver.Expect(m => m.GetService(typeof(IAccountManager))).Return(AccountManager);
                DependencyResolver.SetResolver(MockDependecyResolver);
            }
            catch (Exception)
            {
                _TestTransaction.Dispose();
                _TestTransaction = null;
                if (EducationDataContext != null)
                {
                    EducationDataContext.Dispose();
                    EducationDataContext = null;
                }
                throw;
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (_TestTransaction != null)
            {
                _TestTransaction.Dispose();
            }
            if (EducationDataContext != null)
            {
                EducationDataContext.Dispose();
            }
        }

        [TestMethod]
        public void GivenUserHasRoles_WhenIAuthorize_ThenRolesAreLoaded()
        {
            HttpContext context = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));
            HttpContext.Current = context;
            AuthenticateAndAuthorizeAttribute target = new AuthenticateAndAuthorizeAttribute();
            ControllerContext controllerContext = new ControllerContext(new HttpContextWrapper(context), new RouteData(), new TestController());
            ActionDescriptor action = new ReflectedActionDescriptor(typeof(TestController).GetMethod("Index"), "Index", new ReflectedControllerDescriptor(typeof(TestController)));
            AuthorizationContext authContext = new AuthorizationContext(controllerContext, action);
            context.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity[]
            {
                new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[]
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "Bob")
                }, "Test")
            });
            controllerContext.RouteData.Values.Add("action", "bob");
            controllerContext.RouteData.Values.Add("controller", "fred");

            target.OnAuthorization(authContext);

            Assert.IsInstanceOfType(context.User, typeof(EducationSecurityPrincipal));
            User user = ((EducationSecurityPrincipal)context.User).Identity.User;
            Assert.IsNotNull(user.UserRoles.FirstOrDefault().Role);
            Assert.AreEqual(SecurityRoles.DataAdmin, user.UserRoles.First().Role.Name);
        }

        private class TestController : Controller
        {
            public ActionResult Index() { return null; }
        }
    }
}
