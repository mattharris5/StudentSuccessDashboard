using SSD.Business;
using SSD.Domain;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace SSD.Controllers
{
    public class CustomFieldController : Controller
    {
        private ICustomFieldManager LogicManager { get; set; }

        public CustomFieldController(ICustomFieldManager logicManager)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            LogicManager = logicManager;
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public PartialViewResult Selector()
        {
            CustomFieldSelectorModel viewModel = LogicManager.GenerateSelectorViewModel((EducationSecurityPrincipal)User);
            return PartialView(viewModel);
        }
    }

    public abstract class CustomFieldController<TViewModel> : Controller where TViewModel : CustomFieldModel
    {
        protected ICustomFieldManager LogicManager { get; set; }

        protected CustomFieldController(ICustomFieldManager logicManager)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            LogicManager = logicManager;
        }

        public ViewResult Index()
        {
            return View();
        }

        public JsonResult DataTableAjaxHandler(DataTableRequestModel requestModel)
        {
            IClientDataTable<CustomField> dataTable = CreateClientDataTable();
            var viewModel = LogicManager.GenerateDataTableResultViewModel(requestModel, dataTable);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult Create()
        {
            CustomFieldModel viewModel = LogicManager.GenerateCreateViewModel();
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Create(TViewModel viewModel)
        {
            try
            {
                LogicManager.Validate(viewModel);
            }
            catch (ValidationException e)
            {
                ModelState.AddModelErrors(e);
            }
            if (ModelState.IsValid)
            {
                LogicManager.Create(viewModel, (EducationSecurityPrincipal)User);
                return Json(true);
            }
            LogicManager.PopulateViewModel(viewModel);
            return PartialView(viewModel);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult Edit(int id)
        {
            var viewModel = LogicManager.GenerateEditViewModel(id, (EducationSecurityPrincipal)User);
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(TViewModel viewModel)
        {
            try
            {
                LogicManager.Validate(viewModel);
            }
            catch (ValidationException e)
            {
                ModelState.AddModelErrors(e);
            }
            try
            {
                if (ModelState.IsValid)
                {
                    LogicManager.Edit(viewModel, (EducationSecurityPrincipal)User);
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
        public ActionResult Delete(int id)
        {
            var viewModel = LogicManager.GenerateDeleteViewModel(id);
            return PartialView(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                LogicManager.Delete(id);
                return Json(true);
            }
            catch (ValidationException e)
            {
                ModelState.AddModelErrors(e);
                return Delete(id);
            }
        }

        public ViewResult UploadWizard()
        {
            return View();
        }

        public abstract ViewResult UploadWizardConfirmed(UploadWizardFileViewModel model);

        [HttpPost]
        public ViewResult UploadWizard2(UploadWizardModel model, string submitButton)
        {
            if (submitButton.Equals("previous"))
            {
                ModelState.AddModelError("File", "Please select a file for upload");
                return View("UploadWizard", model);
            }
            else if (submitButton.Equals("submit"))
            {
                if (!model.CustomFields.Select(c => c.SelectedCustomFieldId).Contains(0))
                {
                    ModelState.AddModelError("StudentId", "One column must contain student id");
                    return View(model);
                }
                var uploadComplete = LogicManager.GenerateUploadWizardCompleteViewModel((EducationSecurityPrincipal)User, model);
                return View("UploadWizard3", uploadComplete);
            }
            else
            {
                return View("UploadWizard");
            }
        }

        public ActionResult DownloadUploadErrors(string id)
        {
            var downloadFile = LogicManager.RetrieveUploadErrorsFile(id);
            if (downloadFile == null)
            {
                return HttpNotFound();
            }
            var contentType = "application/octet-stream";
            return File(downloadFile.FileContentStream, contentType, downloadFile.FileName);
        }

        protected abstract IClientDataTable<CustomField> CreateClientDataTable();
    }
}
