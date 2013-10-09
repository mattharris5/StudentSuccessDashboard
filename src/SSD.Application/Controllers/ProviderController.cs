using SSD.ActionFilters;
using SSD.Business;
using SSD.Domain;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.UI;

namespace SSD.Controllers
{
    [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator,Provider")]
    [RequireHttps]
    public class ProviderController : Controller
    {
        public ProviderController(IProviderManager logicManager)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            LogicManager = logicManager;
        }

        private IProviderManager LogicManager { get; set; }

        [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator")]
        public ActionResult Create()
        {
            var viewModel = new ProviderModel();
            LogicManager.PopulateViewModel(viewModel);
            return PartialView(viewModel);
        }

        [HttpPost]
        [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator")]
        public ActionResult Create(ProviderModel viewModel)
        {
            ValidateModelState(viewModel);
            if (ModelState.IsValid)
            {
                LogicManager.Create((EducationSecurityPrincipal)User, viewModel);
                return Json(true);
            }
            LogicManager.PopulateViewModel(viewModel);
            return PartialView(viewModel);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult Edit(int id)
        {
            var viewModel = LogicManager.GenerateEditViewModel((EducationSecurityPrincipal)User, id);
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ProviderModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    LogicManager.Edit((EducationSecurityPrincipal)User, viewModel);
                    return Json(true);
                }
                else
                {
                    LogicManager.PopulateViewModel(viewModel);
                    return PartialView(viewModel);
                }
            }
            catch (ValidationException e)
            {
                ModelState.AddModelErrors(e);
                LogicManager.PopulateViewModel(viewModel);
                return PartialView(viewModel);
            }
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator")]
        public ActionResult Delete(int id)
        {
            ProviderModel viewModel = LogicManager.GenerateDeleteViewModel(id);
            return PartialView(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                LogicManager.Delete(id);
            }
            catch (ValidationException e)
            {
                ModelState.AddModelError("StudentAssignedOfferings", e);
                ProviderModel viewModel = LogicManager.GenerateDeleteViewModel(id);
                return PartialView(viewModel);
            }
            return Json(true);
        }

        public JsonResult DataTableAjaxHandler(DataTableRequestModel requestModel)
        {
            var currentUser = ((EducationSecurityPrincipal)HttpContext.User);
            ProviderClientDataTable dataTable = new ProviderClientDataTable(Request, currentUser);
            var viewModel = LogicManager.GenerateDataTableResultViewModel(requestModel, dataTable);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        private void ValidateModelState(ProviderModel viewModel)
        {
            try
            {
                LogicManager.Validate(viewModel);
            }
            catch (ValidationException e)
            {
                ModelState.AddModelErrors(e);
            }
        }
    }
}
