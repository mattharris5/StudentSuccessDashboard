using Microsoft.WindowsAzure;
using SSD.ActionFilters;
using SSD.Business;
using SSD.IO;
using SSD.Security;
using SSD.ViewModels;
using System;
using System.IO;
using System.Web.Mvc;

namespace SSD.Controllers
{
    [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator,Provider")]
    [RequireHttps]
    public class DataFileController : Controller
    {
        public const string TemplateFile = "StudentProfileExportTemplate.xltx";

        private static string _TemplatePath;

        private ICustomFieldManager LogicManager { get; set; }

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

        public DataFileController(ICustomFieldManager logicManager)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            LogicManager = logicManager;
        }

        public ViewResult StudentProfileExport()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StudentProfileExport(StudentProfileExportModel model)
        {
            try
            {
                LogicManager.CheckStudentCount(model, 65000);
            }
            catch(ArgumentOutOfRangeException)
            {
                ModelState.AddModelError("RowCount", "Export will generate more than the maximum amount of records");
                return PartialView(model);
            }
            MemoryStream stream = LogicManager.GenerateStudentProfileExport((EducationSecurityPrincipal)User, model, TemplatePath) as MemoryStream;
            stream.Position = 0;
            return File(stream, ExcelWriter.ContentType);
        }
    }
}
