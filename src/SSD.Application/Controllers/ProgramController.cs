using SSD.ActionFilters;
using SSD.Business;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Web.Mvc;
using System.Web.UI;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace SSD.Controllers
{
    [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator,Provider")]
    [RequireHttps]
    public class ProgramController : Controller
    {
        public ProgramController(IProgramManager logicManager)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            LogicManager = logicManager;
        }

        private IProgramManager LogicManager { get; set; }

        public JsonResult DataTableAjaxHandler(DataTableRequestModel requestModel)
        {
            ProgramClientDataTable dataTable = new ProgramClientDataTable(Request);
            var viewModel = LogicManager.GenerateDataTableResultViewModel(requestModel, dataTable);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult Create()
        {
            ProgramModel viewModel = LogicManager.GenerateCreateViewModel();
            LogicManager.PopulateViewModelLists(viewModel);
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Create(ProgramModel viewModel)
        {
            ValidateModelState(viewModel);
            if (ModelState.IsValid)
            {
                LogicManager.Create(viewModel);
                return Json(true);
            }
            LogicManager.PopulateViewModelLists(viewModel);
            return PartialView(viewModel);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public PartialViewResult Edit(int id)
        {
            ProgramModel viewModel = LogicManager.GenerateEditViewModel(id);
            LogicManager.PopulateViewModelLists(viewModel);
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ProgramModel viewModel)
        {
            if (ModelState.IsValid)
            {
                LogicManager.Edit(viewModel);
                return Json(true);
            }
            LogicManager.PopulateViewModelLists(viewModel);
            return PartialView(viewModel);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public PartialViewResult Delete(int id)
        {
            ProgramModel viewModel = LogicManager.GenerateDeleteViewModel(id);
            return PartialView(viewModel);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try 
            {
                LogicManager.Delete(id);
            }
            catch (ValidationException e)
            {
                ModelState.AddModelError("StudentAssignedOfferings", e);
                ProgramModel viewModel = LogicManager.GenerateDeleteViewModel(id);
                return PartialView(viewModel);
            }
            return Json(true);
        }

        private void ValidateModelState(ProgramModel viewModel)
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
