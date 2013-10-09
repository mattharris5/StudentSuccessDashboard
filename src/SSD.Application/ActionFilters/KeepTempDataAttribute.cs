using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Web.Mvc;

namespace SSD.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class KeepTempDataAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets the <see cref="ControllerBase.TempData"/> key of the data to keep after this action has been executed.
        /// </summary>
        /// <remarks>Use <see langword="null"/> to keep all temporary data after the action has been executed.</remarks>
        public string TempDataKey { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            base.OnActionExecuted(filterContext);
            if (TempDataKey == null)
            {
                KeepAll(filterContext);
            }
            else
            {
                KeepKey(filterContext);
            }
        }

        private void KeepAll(ActionExecutedContext filterContext)
        {
            filterContext.Controller.TempData.Keep();
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Keeping all TempData for controller '{0}' after execution of '{1}' action.", filterContext.Controller.GetType().Name, filterContext.ActionDescriptor.ActionName),
                            string.Format(CultureInfo.InvariantCulture, "{0}.{1}", GetType().Name, MethodInfo.GetCurrentMethod().Name));
        }

        private void KeepKey(ActionExecutedContext filterContext)
        {
            filterContext.Controller.TempData.Keep(TempDataKey);
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Keeping TempData [{2}] for controller '{0}' after execution of '{1}' action.", filterContext.Controller.GetType().Name, filterContext.ActionDescriptor.ActionName, TempDataKey),
                            string.Format(CultureInfo.InvariantCulture, "{0}.{1}", GetType().Name, MethodInfo.GetCurrentMethod().Name));
        }
    }
}
