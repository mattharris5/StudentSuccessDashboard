using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.ViewHelpers
{
    public static class ActionExtensions
    {
        public static MvcHtmlString ActionIconLink(this HtmlHelper htmlHelper, string displayText, string action, string controller, string iconClass)
        {
            return ActionIconLink(htmlHelper, displayText, action, controller, iconClass, null, null);
        }

        public static MvcHtmlString ActionIconLink(this HtmlHelper htmlHelper, string displayText, string action, string controller, string iconClass, object routeValues, object htmlAttributes)
        {
            return ActionIconLink(htmlHelper, displayText, action, controller, iconClass, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ActionIconLink(this HtmlHelper htmlHelper, string displayText, string action, string controller, string iconClass, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            var actionIconLink = new TagBuilder("a");
            var iconTag = new TagBuilder("i");
            iconTag.AddCssClass(iconClass);
            actionIconLink.Attributes.Add("href", GetActionUrl(htmlHelper, action, controller, routeValues));
            actionIconLink.MergeAttributes(htmlAttributes);
            actionIconLink.InnerHtml = iconTag.ToString(TagRenderMode.Normal) + displayText;
            return new MvcHtmlString(actionIconLink.ToString(TagRenderMode.Normal));
        }

        private static string GetActionUrl(HtmlHelper htmlHelper, string action, string controller, RouteValueDictionary routeValues)
        {
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection);
            return urlHelper.Action(action, controller, routeValues);
        }

        public static string AbsoluteAction(this UrlHelper url, string actionName, string controllerName)
        {
            return AbsoluteAction(url, actionName, controllerName, null);
        }

        public static string AbsoluteAction(this UrlHelper url, string actionName, string controllerName, object routeValues)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
            string scheme = url.RequestContext.HttpContext.Request.Url.Scheme;
            return url.Action(actionName, controllerName, routeValues, scheme);
        }
    }
}