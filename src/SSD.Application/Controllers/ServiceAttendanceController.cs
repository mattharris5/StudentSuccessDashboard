using Microsoft.WindowsAzure;
using SSD.ActionFilters;
using SSD.Business;
using SSD.IO;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;

namespace SSD.Controllers
{
    [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator,Provider")]
    [RequireHttps]
    public class ServiceAttendanceController : Controller
    {
        public const string TemplateFile = "ServiceAttendance.xltx";
        private static string _TemplatePath;

        public ServiceAttendanceController(IServiceAttendanceManager logicManager, IFileProcessor fileProcessor)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            if (fileProcessor == null)
            {
                throw new ArgumentNullException("fileProcessor");
            }
            LogicManager = logicManager;
            FileProcessor = fileProcessor;
        }

        private IServiceAttendanceManager LogicManager { get; set; }
        private IFileProcessor FileProcessor { get; set; }
        private string TemplatePath
        {
            get
            {
                if (_TemplatePath == null)
                {
                    string serverPath = Server.MapPath(CloudConfigurationManager.GetSetting("FileUploadTemplatePath"));
                    _TemplatePath = Path.Combine(serverPath, TemplateFile);
                }
                return _TemplatePath;
            }
        }

        public ActionResult Index(int id) 
        {
            ServiceAttendanceModel viewModel = LogicManager.GenerateCreateViewModel((EducationSecurityPrincipal)User, id);
            return View(viewModel);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult Create(int id)
        {
            ServiceAttendanceModel viewModel = LogicManager.GenerateCreateViewModel((EducationSecurityPrincipal)User, id);
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Create(ServiceAttendanceModel viewModel)
        {
            if (ModelState.IsValid)
            {
                LogicManager.Create(viewModel, (EducationSecurityPrincipal)User);
                return Json(true);
            }
            return PartialView(viewModel);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult Edit(int id)
        {
            var viewModel = LogicManager.GenerateEditViewModel((EducationSecurityPrincipal)User, id);
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ServiceAttendanceModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    LogicManager.Edit(viewModel, (EducationSecurityPrincipal)User);
                    return Json(true);
                }
                else
                {
                    return PartialView(viewModel);
                }
            }
            catch (ValidationException e)
            {
                ModelState.AddModelErrors(e);
                return PartialView(viewModel);
            }
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult Delete(int id)
        {
            ServiceAttendanceModel viewModel = LogicManager.GenerateDeleteViewModel((EducationSecurityPrincipal)User, id);
            return PartialView(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            LogicManager.Delete(id, (EducationSecurityPrincipal)User);
            return Json(true);
        }

        public ActionResult DownloadTemplate(int id)
        {
            var downloadFile = FileProcessor.CreateTemplateDownload((EducationSecurityPrincipal)User, TemplatePath, id);
            if (downloadFile == null)
            {
                return HttpNotFound();
            }
            var contentType = ExcelWriter.ContentType;
            return File(downloadFile.FileContentStream, contentType, downloadFile.FileName);
        }

        public ActionResult FileUpload()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult FileUpload(UploadExcelFileModel uploadFile)
        {
            var model = new ServiceUploadModel();
            if (!ModelState.IsValid)
            {
                foreach (ModelState state in ModelState.Values)
                {
                    foreach (ModelError error in state.Errors)
                    {
                        model.RowErrors.Add(error.ErrorMessage);
                    }
                }
                return View("FileUploadComplete", model);
            }
            DataTable dataTable = null;
            try
            {
                dataTable = FileProcessor.ConsumeFile(uploadFile);
            }
            catch
            {
                model.RowErrors.Add("Corrupted file format.  Please redownload the template and try again.");
                model.ProcessedRowCount = model.SuccessfulRowsCount = 0;
                return View("FileUploadComplete", model);
            }
            model = FileProcessor.Import((EducationSecurityPrincipal)User, TemplatePath, dataTable);
            return View("FileUploadComplete", model);
        }

        public JsonResult DataTableAjaxHandler(DataTableRequestModel requestModel)
        {
            ServiceAttendanceClientDataTable dataTable = new ServiceAttendanceClientDataTable(Request);
            var viewModel = LogicManager.GenerateDataTableResultViewModel(requestModel, dataTable);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
    }
}
