using SSD.ActionFilters;
using SSD.Business;
using SSD.ViewModels;
using System;
using System.Web.Mvc;

namespace SSD.Controllers
{
    [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator,Provider")]
    [RequireHttps]
    public class SchoolController : Controller
    {
        public SchoolController(ISchoolDistrictManager logicManager)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            LogicManager = logicManager;
        }

        private ISchoolDistrictManager LogicManager { get; set; }

        public PartialViewResult Selector()
        {
            SchoolSelectorModel viewModel = LogicManager.GenerateSchoolSelectorViewModel();
            return PartialView(viewModel);
        }
    }
}
