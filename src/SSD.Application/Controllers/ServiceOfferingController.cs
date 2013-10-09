using Microsoft.WindowsAzure;
using SSD.ActionFilters;
using SSD.Business;
using SSD.IO;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace SSD.Controllers
{
    [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator,Provider")]
    [RequireHttps]
    public class ServiceOfferingController : Controller
    {
        public const string TemplateFile = "AssignedServiceOffering.xltx";
        private static string _TemplatePath;

        public ServiceOfferingController(IServiceOfferingManager logicManager, IServiceTypeManager serviceTypeManager, IProviderManager providerManager, IProgramManager programManager, IFileProcessor fileProcessor)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            if (serviceTypeManager == null)
            {
                throw new ArgumentNullException("serviceTypeManager");
            }
            if (providerManager == null)
            {
                throw new ArgumentNullException("providerManager");
            }
            if (programManager == null)
            {
                throw new ArgumentNullException("programManager");
            }
            if (fileProcessor == null)
            {
                throw new ArgumentNullException("fileProcessor");
            }
            LogicManager = logicManager;
            ServiceTypeManager = serviceTypeManager;
            ProviderManager = providerManager;
            ProgramManager = programManager;
            FileProcessor = fileProcessor;
        }

        private IServiceOfferingManager LogicManager { get; set; }
        private IServiceTypeManager ServiceTypeManager { get; set; }
        private IProviderManager ProviderManager { get; set; }
        private IProgramManager ProgramManager { get; set; }
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

        public ViewResult Index()
        {
            var viewModel = LogicManager.GenerateListOptionsViewModel((EducationSecurityPrincipal)User);
            return View(viewModel);
        }

        public JsonResult DataTableAjaxHandler(DataTableRequestModel requestModel)
        {
            ServiceOfferingClientDataTable dataTable = new ServiceOfferingClientDataTable(Request, (EducationSecurityPrincipal)User);
            var viewModel = LogicManager.GenerateDataTableResultViewModel(requestModel, dataTable);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SetFavorite(int id, bool isFavorite)
        {
            LogicManager.SetFavoriteState((EducationSecurityPrincipal)User, id, isFavorite);
            return Json(true);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public PartialViewResult Favorites()
        {
            var userFavorites = LogicManager.LoadFavorites((EducationSecurityPrincipal)User);
            return PartialView(userFavorites);
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

        public ActionResult DownloadUploadErrors(string id)
        {
            var downloadFile = FileProcessor.RetrieveUploadErrorsFile(id);
            if (downloadFile == null)
            {
                return HttpNotFound();
            }
            return File(downloadFile.FileContentStream, ExcelWriter.ContentType, downloadFile.FileName);
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

        public JsonResult AutocompleteServiceTypeProviderOrProgram(string term)
        {
            var serviceTypeNames = ServiceTypeManager.SearchNames(term);
            var providerNames = ProviderManager.SearchProviderNames(term);
            var programNames = ProgramManager.SearchProgramNames(term);
            var filteredItems = serviceTypeNames.Union(providerNames).Union(programNames).Distinct().OrderBy(n => n);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
    }
}
