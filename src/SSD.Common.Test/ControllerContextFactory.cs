using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD
{
    public static class ControllerContextFactory
    {
        private static ControllerContext CreateControllerContext(HttpContextBase httpContext)
        {
            ControllerContext controllerContext = new ControllerContext(httpContext, new RouteData(), new TestController());
            controllerContext.RouteData.Values["controller"] = "Test";
            controllerContext.RouteData.Values["action"] = "IAmAnAction";
            return controllerContext;
        }

        public static AuthorizationContext CreateAuthorizationContext(HttpContextBase httpContext)
        {
            ControllerContext controllerContext = CreateControllerContext(httpContext);
            ReflectedActionDescriptor actionDescriptor = CreateActionDescriptor(typeof(TestController), "IAmAnAction");
            return new AuthorizationContext(controllerContext, actionDescriptor);
        }

        public static AuthorizationContext CreateAuthorizationContext(HttpContextBase httpContext, Type controllerType, string actionName)
        {
            ControllerContext controllerContext = CreateControllerContext(httpContext);
            ReflectedActionDescriptor actionDescriptor = CreateActionDescriptor(controllerType, actionName);
            return new AuthorizationContext(controllerContext, actionDescriptor);
        }

        public static ActionExecutingContext CreateActionExecutingContext(HttpContextBase httpContext)
        {
            ControllerContext controllerContext = CreateControllerContext(httpContext);
            ReflectedActionDescriptor actionDescriptor = CreateActionDescriptor(typeof(TestController), "IAmAnAction");
            return new ActionExecutingContext(controllerContext, actionDescriptor, new Dictionary<string, object>());
        }

        public static ExceptionContext CreateExceptionContext(HttpContextBase httpContext, Exception exception)
        {
            ControllerContext controllerContext = CreateControllerContext(httpContext);
            return new ExceptionContext(controllerContext, exception);
        }

        private static ReflectedActionDescriptor CreateActionDescriptor(Type controllerType, string actionName)
        {
            ReflectedControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(controllerType);
            return new ReflectedActionDescriptor(controllerType.GetMethod(actionName), actionName, controllerDescriptor);
        }

        private class TestController : Controller
        {
            public ViewResult IAmAnAction()
            {
                return View();
            }
        }
    }
}
