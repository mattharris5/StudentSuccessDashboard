using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class ErrorControllerTest : BaseControllerTest
    {
        private ErrorController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new ErrorController();
            ControllerContext controllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
            Target.ControllerContext = controllerContext;
        }

        [TestMethod]
        public void GivenAspxErrorPath_WhenNotFound_ThenViewModelIsAspxErrorPath()
        {
            string expected = "apples/3";
            ViewResult result = Target.NotFound(expected);
            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenNullAspxErrorPath_WhenNotFound_ThenViewModelIsRawUrl()
        {
            string expected = "oranges/4";
            MockHttpContext.Request.Expect(m => m.RawUrl).Return(expected);
            ViewResult result = Target.NotFound(null);
            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenEmptyAspxErrorPath_WhenNotFound_ThenViewModelIsRawUrl()
        {
            string expected = "oranges/4";
            MockHttpContext.Request.Expect(m => m.RawUrl).Return(expected);
            ViewResult result = Target.NotFound(string.Empty);
            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenWhitespaceAspxErrorPath_WhenNotFound_ThenViewModelIsRawUrl()
        {
            string expected = "oranges/4";
            MockHttpContext.Request.Expect(m => m.RawUrl).Return(expected);
            ViewResult result = Target.NotFound("       \r\n");
            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenNotFound_ThenResponseIs404()
        {
            ViewResult result = Target.NotFound(null);
            MockHttpContext.Response.AssertWasCalled(m => m.StatusCode = 404);
        }

        [TestMethod]
        public void WhenNotFound_ThenIisDefaultErrorsSkipped()
        {
            ViewResult result = Target.NotFound(null);
            MockHttpContext.Response.AssertWasCalled(m => m.TrySkipIisCustomErrors = true);
        }

        [TestMethod]
        public void WhenUnauthorized_ThenViewModelIsNull()
        {
            ViewResult result = Target.Unauthorized();
            Assert.IsNull(result.Model);
        }

        [TestMethod]
        public void WhenUnauthorized_ThenResponseIs401()
        {
            ViewResult result = Target.Unauthorized();
            MockHttpContext.Response.AssertWasCalled(m => m.StatusCode = 401);
        }

        [TestMethod]
        public void WhenUnauthorized_ThenIisDefaultErrorsSkipped()
        {
            ViewResult result = Target.Unauthorized();
            MockHttpContext.Response.AssertWasCalled(m => m.TrySkipIisCustomErrors = true);
        }
    }
}
