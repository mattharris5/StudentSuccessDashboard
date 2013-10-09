using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.ViewHelpers
{
    [TestClass]
    public class ActionExtensionsTest
    {
        [TestMethod]
        public void GivenNullUrlHelper_WhenAbsoluteAction_ThenThrowException()
        {
            UrlHelper target = null;
            target.ExpectException<ArgumentNullException>(() => target.AbsoluteAction("whatever", "whatever"));
        }

        [TestMethod]
        public void WhenActionIconLink_ThenGetExpectedMarkup()
        {
            var routeCollection = CreateRouteCollection();
            var htmlHelper = MockHtmlHelperFactory.Create(new ViewDataDictionary(), routeCollection);
            var expected = "<a href=\"/ControllerName/ActionName\"><i class=\"icon-home\"></i> Navigate</a>";

            var actual = ActionExtensions.ActionIconLink(htmlHelper, " Navigate", "ActionName", "ControllerName", "icon-home").ToHtmlString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenHtmlAttributes_WhenActionIconLink_ThenGetExpectedMarkup()
        {
            var routeCollection = CreateRouteCollection();
            var htmlHelper = MockHtmlHelperFactory.Create(new ViewDataDictionary(), routeCollection);
            var expected = "<a class=\"anchor\" href=\"/ControllerName/ActionName\" id=\"linkToSomethingImportant\"><i class=\"icon-home\"></i> Navigate</a>";
            var htmlAttributes = new { id = "linkToSomethingImportant", @class = "anchor" };

            var actual = ActionExtensions.ActionIconLink(htmlHelper, " Navigate", "ActionName", "ControllerName", "icon-home", null, htmlAttributes).ToHtmlString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenRouteValues_WhenActionIconLink_ThenGetExpectedMarkup()
        {
            var routeCollection = CreateRouteCollection();
            var htmlHelper = MockHtmlHelperFactory.Create(new ViewDataDictionary(), routeCollection);
            var expected = "<a href=\"/ControllerName/ActionName/1?queryString=extraData\"><i class=\"icon-home\"></i> Navigate</a>";
            var routeValues = new { id = "1", queryString = "extraData" };

            var actual = ActionExtensions.ActionIconLink(htmlHelper, " Navigate", "ActionName", "ControllerName", "icon-home", routeValues, null).ToHtmlString();

            Assert.AreEqual(expected, actual);
        }

        private RouteCollection CreateRouteCollection()
        {
            var routeCollection = new RouteCollection();
            RouteConfig.RegisterRoutes(routeCollection);
            return routeCollection;
        }
    }
}
