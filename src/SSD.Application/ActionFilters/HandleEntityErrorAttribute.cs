using SSD.Business;
using System;
using System.Web.Mvc;

namespace SSD.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class HandleEntityErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            if (filterContext.ExceptionHandled)
            {
                return;
            }
            if (filterContext.Exception.GetType().Equals(typeof(EntityNotFoundException)))
            {
                HandleEntityNotFound(filterContext);
                return;
            }
            if (filterContext.Exception.GetType().Equals(typeof(EntityAccessUnauthorizedException)))
            {
                HandleEntityAccessUnauthorized(filterContext);
                return;
            }
            base.OnException(filterContext);
        }

        private void HandleEntityNotFound(ExceptionContext filterContext)
        {
            filterContext.Result = new HttpNotFoundResult(filterContext.Exception.Message);
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 404;
        }

        private void HandleEntityAccessUnauthorized(ExceptionContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult(filterContext.Exception.Message);
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 401;
        }
    }
}
