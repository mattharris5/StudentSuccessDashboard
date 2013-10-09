using System.Web.Mvc;

namespace SSD.Areas.Partners
{
    public class PartnersAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Partners";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Partners_default",
                "Partners/{controller}/{action}/{id}",
                new { controller = "Partners", action = "Index", id = UrlParameter.Optional },
                namespaces: new [] { "SSD.Controllers" }
            );
        }
    }
}
