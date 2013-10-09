using SSD.ActionFilters;
using SSD.Business;
using SSD.Security;
using SSD.ViewModels;
using System;
using System.Web.Mvc;

namespace SSD.Controllers
{
    [RequireHttps]
    public class AgreementController : Controller
    {
        public AgreementController(IAgreementManager logicManager)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            LogicManager = logicManager;
        }

        private IAgreementManager LogicManager { get; set; }

        public ViewResult Index()
        {
            EulaModel viewModel = LogicManager.GeneratePromptViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(EulaModel viewModel, string redirect)
        {
            LogicManager.Log(viewModel, (EducationSecurityPrincipal)User);
            if (redirect == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(redirect);
        }

        [AuthenticateAndAuthorize(Roles = "Data Admin")]
        public ViewResult Admin()
        {
            EulaModel model = LogicManager.GenerateEulaAdminModel();
            return View(model);
        }

        [AuthenticateAndAuthorize(Roles = "Data Admin")]
        [HttpPost]
        public ActionResult Admin(EulaModel viewModel)
        {
            if (ModelState.IsValid)
            {
                LogicManager.Create(viewModel, (EducationSecurityPrincipal)User);
                viewModel = LogicManager.GenerateEulaAdminModel();
                return View(viewModel);
            }
            return PartialView(viewModel);
        }

        public ActionResult UserEula(int id)
        {
            var viewModel = LogicManager.GenerateEulaModelByUser(id);
            return PartialView(viewModel);
        }
    }
}
