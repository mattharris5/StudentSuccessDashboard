using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("Scripts/{*bundle}", new { bundle = @"(.*/)?bundle(/.*)?" });
            routes.IgnoreRoute("Content/{*bundle}", new { bundle = @"(.*/)?bundle(/.*)?" });

            routes.MapRoute(
                name: "404",
                url: "NotFound",
                defaults: new { controller = "Error", action = "NotFound" }
            );

            routes.MapRoute(
                name: "401",
                url: "Unauthorized",
                defaults: new { controller = "Error", action = "Unauthorized" }
            );

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}