using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace SSD.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class TraceActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            string userName = GetLogicalUserName(filterContext);
            StringBuilder message = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "User {0} has executed {1}.{2} with a result of type {3}.", userName, filterContext.Controller.GetType().Name, filterContext.ActionDescriptor.ActionName, filterContext.Result == null ? "[null]" : filterContext.Result.GetType().Name));
            AppendRouteData(filterContext, message);
            AppendExceptionDetails(filterContext.ExceptionHandled, filterContext.Exception, message);
            AppendChildParentActionDetails(filterContext, message);
            base.OnActionExecuted(filterContext);
            Trace.WriteLine(message.ToString(), MethodInfo.GetCurrentMethod().Name);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            string userName = GetLogicalUserName(filterContext);
            StringBuilder message = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "User {0} is executing {1}.{2}.", userName, filterContext.Controller.GetType().Name, filterContext.ActionDescriptor.ActionName));
            AppendRouteData(filterContext, message);
            AppendActionParameters(filterContext, message);
            AppendChildParentActionDetails(filterContext, message);
            Trace.WriteLine(message.ToString(), MethodInfo.GetCurrentMethod().Name);
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            string userName = GetLogicalUserName(filterContext);
            StringBuilder message = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "{0} executed result of type {1} on behalf of user {2}.", filterContext.Controller.GetType().Name, filterContext.Result == null ? "[null]" : filterContext.Result.GetType().Name, userName));
            AppendRouteData(filterContext, message);
            AppendExceptionDetails(filterContext.ExceptionHandled, filterContext.Exception, message);
            AppendChildParentActionDetails(filterContext, message);
            base.OnResultExecuted(filterContext);
            Trace.WriteLine(message.ToString(), MethodInfo.GetCurrentMethod().Name);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            string userName = GetLogicalUserName(filterContext);
            StringBuilder message = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "{0} executed result of type {1} on behalf of user {2}.", filterContext.Controller.GetType().Name, filterContext.Result == null ? "[null]" : filterContext.Result.GetType().Name, userName));
            AppendRouteData(filterContext, message);
            AppendChildParentActionDetails(filterContext, message);
            Trace.WriteLine(message.ToString(), MethodInfo.GetCurrentMethod().Name);
            base.OnResultExecuting(filterContext);
        }

        private static void AppendActionParameters(ActionExecutingContext filterContext, StringBuilder message)
        {
            if (filterContext.ActionParameters.Count > 0)
            {
                message.Append("\r\n\tAction parameters: [");
                AppendDictionaryList(filterContext.ActionParameters, message);
                message.Append("]");
            }
        }

        private static string GetLogicalUserName(ControllerContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                string identityName = filterContext.HttpContext.User.Identity.Name;
                return string.IsNullOrWhiteSpace(identityName) ? "[NameUnknown]" : identityName;
            }
            return "[Anonymous]";
        }

        private static void AppendRouteData(ControllerContext filterContext, StringBuilder message)
        {
            message.Append("\r\n\tRoute data: [");
            AppendDictionaryList(filterContext.RouteData.Values, message);
            message.Append("]");
        }

        private static void AppendDictionaryList(IDictionary<string, object> dictionary, StringBuilder message)
        {
            bool prependComma = false;
            foreach (KeyValuePair<string, object> entry in dictionary)
            {
                if (prependComma)
                {
                    message.Append(", ");
                }
                AppendKeyValueEntry(entry, message);
                prependComma = true;
            }
        }

        private static void AppendKeyValueEntry(KeyValuePair<string, object> entry, StringBuilder message)
        {
            IConvertible convertablePairValue = entry.Value as IConvertible;
            if (convertablePairValue != null)
            {
                message.Append(string.Format(CultureInfo.InvariantCulture, "({0}: {1})", entry.Key, convertablePairValue.ToString()));
            }
            else if (entry.Value == null)
            {
                message.Append(string.Format(CultureInfo.InvariantCulture, "({0}: [null])", entry.Key));
            }
            else
            {
                message.Append(string.Format(CultureInfo.InvariantCulture, "({0}: Instance of type {1})", entry.Key, entry.Value.GetType().Name));
            }
        }

        private static void AppendExceptionDetails(bool exceptionHandled, Exception exception, StringBuilder message)
        {
            if (exceptionHandled)
            {
                message.Append("\r\n\tException was handled during execution.");
            }
            if (exception != null)
            {
                message.Append("\r\n\tException details: ");
                message.Append(exception);
            }
        }

        private static void AppendChildParentActionDetails(ControllerContext filterContext, StringBuilder message)
        {
            if (filterContext.IsChildAction)
            {
                message.Append(string.Format(CultureInfo.InvariantCulture, "\r\n\tAction is a child of {0} from {1}.", filterContext.ParentActionViewContext.Controller.GetType().Name, filterContext.ParentActionViewContext.View.GetType().Name));
            }
        }
    }
}