using SSD.ActionFilters;
using SSD.Business;
using SSD.Security;
using SSD.ViewModels;
using System;
using System.Web.Mvc;
using System.Web.UI;

namespace SSD.Controllers
{
    [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator,Provider")]
    [RequireHttps]
    public class ServiceRequestController : Controller
    {
        public ServiceRequestController(IServiceRequestManager logicManager)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            LogicManager = logicManager;
        }

        private IServiceRequestManager LogicManager { get; set; }

        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult Edit(int id)
        {
            ServiceRequestModel viewModel = LogicManager.GenerateEditViewModel((EducationSecurityPrincipal)HttpContext.User, id);
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ServiceRequestModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.OriginalStatusId != viewModel.SelectedStatusId && viewModel.FulfillmentNotes == null)
                {
                    ModelState.AddModelError(string.Empty, "You must put in Fulfillment Notes if you changed the Status");
                    viewModel = LogicManager.GenerateEditViewModel((EducationSecurityPrincipal)HttpContext.User, viewModel.Id);
                    return PartialView(viewModel);
                }
                LogicManager.Edit((EducationSecurityPrincipal)HttpContext.User, viewModel);
                return Json(true);
            }
            LogicManager.PopulateViewModel(viewModel);
            return PartialView(viewModel);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult Delete(int id)
        {
            ServiceRequestModel viewModel = LogicManager.GenerateDeleteViewModel((EducationSecurityPrincipal)HttpContext.User, id);
            return PartialView(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            LogicManager.Delete((EducationSecurityPrincipal)HttpContext.User, id);
            return Json(true);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult Create()
        {
            ServiceRequestModel viewModel = LogicManager.GenerateCreateViewModel();
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Create(ServiceRequestModel viewModel)
        {
            LogicManager.Create((EducationSecurityPrincipal)HttpContext.User, viewModel);
            return Json(true);
        }
    }
}
