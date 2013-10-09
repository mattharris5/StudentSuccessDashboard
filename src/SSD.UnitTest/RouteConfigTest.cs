using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SSD
{
    [TestClass]
    public class RouteConfigTest
    {
        private RouteCollection Routes { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Routes = new RouteCollection();
            RouteConfig.RegisterRoutes(Routes);
        }

        [TestMethod]
        public void TestIncomingBundlesRoutes()
        {
            Routes.AssertRouteFail("~/Content/bundle");
            Routes.AssertRouteFail("~/Content/blah/Bundle");
            Routes.AssertRouteFail("~/Content/blah/css/bundle");

            Routes.AssertRouteFail("~/Scripts/bundle");
            Routes.AssertRouteFail("~/Scripts/blah/Bundle");
            Routes.AssertRouteFail("~/Scripts/blah/css/bundle");
        }

        [TestMethod]
        public void TestIncomingNotFoundRoutes()
        {
            Routes.AssertRouteMatch("~/NotFound", "Error", "NotFound");
            Routes.AssertRouteMatch("~/Error/NotFound", "Error", "NotFound");
        }

        [TestMethod]
        public void TestIncomingUnauthorizedRoutes()
        {
            Routes.AssertRouteMatch("~/Unauthorized", "Error", "Unauthorized");
            Routes.AssertRouteMatch("~/Error/Unauthorized", "Error", "Unauthorized");
        }

        [TestMethod]
        public void TestIncomingDefaultRoutes()
        {
            Routes.AssertRouteMatch("~/", "Home", "Index");
            Routes.AssertRouteMatch("~/Customer", "Customer", "Index");
            Routes.AssertRouteMatch("~/Customer/Index", "Customer", "Index");
            Routes.AssertRouteMatch("~/Customer/Index/Bob", "Customer", "Index", new { id = "Bob" });
            Routes.AssertRouteMatch("~/Customer/Index/1", "Customer", "Index", new { id = 1 });
            Routes.AssertRouteMatch("~/One/Two", "One", "Two");
            Routes.AssertRouteFail("~/Customer/Index/ID/Segment");
        }

        [TestMethod]
        public void TestWebApiIncomingDefaultRoutes()
        {
            Routes.AssertRouteMatch("~/api/Customer", "Customer");
            Routes.AssertRouteMatch("~/api/Customer/Index", "Customer");
            Routes.AssertRouteMatch("~/api/Customer/Bob", "Customer", new { id = "Bob" });
            Routes.AssertRouteMatch("~/api/Customer/1", "Customer", new { id = 1 });
            Routes.AssertRouteMatch("~/api/One/Two", "One", new { id = "Two" });
            Routes.AssertRouteFail("~/api/Customer/ID/Segment");
        }
    }
}
