using System.Net;
using System.Web.Mvc;

namespace SSD.Controllers
{
    public class ErrorController : Controller
    {
        public ViewResult NotFound(string aspxerrorpath)
        {
            object viewModel = string.IsNullOrWhiteSpace(aspxerrorpath) ? Request.RawUrl : aspxerrorpath;
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            Response.TrySkipIisCustomErrors = true;
            return View(viewModel);
        }

        public ViewResult Unauthorized()
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }
    }
}
