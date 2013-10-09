using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using SSD.Security;
using System.Web;

namespace SSD.Controllers
{
    [TestClass]
    public abstract class BaseControllerTest
    {
        protected EducationSecurityPrincipal User { get; private set; }
        protected HttpContextBase MockHttpContext { get; private set; }

        [TestInitialize]
        public void BaseInitializeTest()
        {
            User = new EducationSecurityPrincipal(new User { UserKey = "whatever" });
            MockHttpContext = MockHttpContextFactory.Create();
        }
    }
}
