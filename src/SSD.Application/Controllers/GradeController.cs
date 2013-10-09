using SSD.ActionFilters;
using SSD.Business;
using SSD.ViewModels;
using System;
using System.Web.Mvc;

namespace SSD.Controllers
{
    [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator,Provider")]
    [RequireHttps]
    public class GradeController : Controller
    {
        public GradeController(ISchoolDistrictManager logicManager)
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
            GradeSelectorModel viewModel = LogicManager.GenerateGradeSelectorViewModel();
            return PartialView(viewModel);
        }
    }
}
