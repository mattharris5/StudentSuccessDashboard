using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD
{
    public static class TestExtensions
    {
        public static TException ExpectException<TException>(Action action) where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException ex)
            {
                return ex;
            }
            Assert.Fail("Expected " + typeof(TException).Name);
            return null;
        }

        public static TException ExpectException<TException>(this object target, Action action) where TException : Exception
        {
            return ExpectException<TException>(action);
        }

        public static void AssertActionRedirection(this RedirectToRouteResult result, string action, string controller)
        {
            Assert.IsNotNull(result, "Redirection result is null.");
            if (!string.IsNullOrEmpty(controller))
            {
                Assert.AreEqual(controller, result.RouteValues["controller"]);
            }
            Assert.AreEqual(action, result.RouteValues["action"]);
        }

        public static void AssertActionRedirection(this RedirectToRouteResult result, string action)
        {
            AssertActionRedirection(result, action, null);
        }

        public static TData AssertGetData<TData>(this JsonResult result)
        {
            Assert.IsNotNull(result, "Json result is null.");
            Assert.IsNotNull(result.Data, "Json result's data is null.");
            Assert.IsInstanceOfType(result.Data, typeof(TData), "Json result's data is not of the expected type.");
            return (TData)result.Data;
        }

        public static TData AssertGetData<TData>(this JsonResult result, TData expectedData)
        {
            TData actualData = AssertGetData<TData>(result);
            Assert.AreEqual(expectedData, actualData, "Json's data is different than expected.");
            return actualData;
        }

        public static TViewModel AssertGetViewModel<TViewModel>(this ViewResultBase result)
        {
            Assert.IsNotNull(result, "View result is null.");
            Assert.IsNotNull(result.Model, "View result's model is null.");
            Assert.IsInstanceOfType(result.Model, typeof(TViewModel), "View result's model is not of the expected type.");
            return (TViewModel)result.Model;
        }

        public static TViewModel AssertGetViewModel<TViewModel>(this ViewResultBase result, TViewModel expectedModel)
        {
            TViewModel actualModel = AssertGetViewModel<TViewModel>(result);
            Assert.AreEqual(expectedModel, actualModel, "View's model is different than expected.");
            return actualModel;
        }

        public static void AssertItemsEqual(this IList<string[]> actualEnumerableOfEnumerables, IList<string[]> expectedEnumerableOfEnumerables)
        {
            for (int i = 0; i < actualEnumerableOfEnumerables.Count; i++)
            {
                IList<string> itemList = actualEnumerableOfEnumerables[i].ToList();
                for (int j = 0; j < itemList.Count; j++)
                {
                    Assert.AreEqual(expectedEnumerableOfEnumerables[i].ToList()[j], itemList[j]);
                }
            }
        }

        public static RouteData AssertRouteMatch(this RouteCollection routes, string url, string controller, object routeProperties = null, string httpMethod = "GET")
        {
            HttpContextBase httpContext = MockHttpContextFactory.Create(url, httpMethod);
            RouteData result = routes.GetRouteData(httpContext);
            Assert.IsNotNull(result);
            Assert.AreEqual(controller, result.Values["controller"] as string, true);
            AssertRouteData(result, routeProperties);
            return result;
        }

        public static RouteData AssertRouteMatch(this RouteCollection routes, string url, string controller, string action, object routeProperties = null, string httpMethod = "GET")
        {
            RouteData result = AssertRouteMatch(routes, url, controller, routeProperties, httpMethod);
            Assert.AreEqual(action, result.Values["action"] as string, true);
            return result;
        }

        public static void AssertRouteFail(this RouteCollection routes, string url)
        {
            HttpContextBase httpContext = MockHttpContextFactory.Create(url);
            RouteData result = routes.GetRouteData(httpContext);
            if (result != null)
            {
                Assert.IsInstanceOfType(result.RouteHandler, typeof(StopRoutingHandler));
            }
        }

        public static void AssertRouteData(this RouteData routeResult, object propertySet)
        {
            if (propertySet != null)
            {
                PropertyInfo[] properties = propertySet.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (routeResult.Values.ContainsKey(property.Name))
                    {
                        Assert.AreEqual(property.GetValue(propertySet, null).ToString(), routeResult.Values[property.Name] as string, true);
                    }
                }
            }
        }
    }
}
