using SSD.ActionFilters;
using System.Web.Mvc;

namespace SSD.Controllers
{
    public class HomeController : Controller
    {
        [AuthenticateAndAuthorize]
        public ViewResult Index()
        {
            return View();
        }

        public ViewResult About()
        {
            return View();
        }

        public ViewResult Terms()
        {
            return View();
        }

        public ViewResult Privacy()
        {
            return View();
        }
    }
}
