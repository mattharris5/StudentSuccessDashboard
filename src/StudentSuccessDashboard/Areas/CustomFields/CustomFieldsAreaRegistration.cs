using System.Web.Mvc;

namespace SSD.Areas.CustomFields
{
    public class CustomFieldsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "CustomFields";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "CustomFields_default",
                "CustomFields/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "SSD.Controllers" }
            );
        }
    }
}
