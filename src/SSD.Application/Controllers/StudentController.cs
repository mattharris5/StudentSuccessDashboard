using SSD.ActionFilters;
using SSD.Business;
using SSD.Domain;
using SSD.Security;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SSD.Controllers
{
    [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator,Provider")]
    [RequireHttps]
    public class StudentController : Controller
    {
        public StudentController(ISchoolDistrictManager logicManager)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            LogicManager = logicManager;
        }
        
        private ISchoolDistrictManager LogicManager { get; set; }

        public ViewResult Index()
        {
            var viewModel = LogicManager.GenerateListOptionsViewModel((EducationSecurityPrincipal)User);
            return View(viewModel);
        }

        public JsonResult AutocompleteFirstName(string term)
        {
            var filteredItems = term.Length > 2 ? LogicManager.SearchFirstNames((EducationSecurityPrincipal)HttpContext.User, term) : null;
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutocompleteLastName(string term)
        {
            var filteredItems = term.Length > 2 ? LogicManager.SearchLastNames((EducationSecurityPrincipal)HttpContext.User, term) : null;
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutocompleteId(string term)
        {
            var filteredItems = term.Length > 2 ? LogicManager.SearchIdentifiers((EducationSecurityPrincipal)HttpContext.User, term) : null;
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DataTableAjaxHandler(DataTableRequestModel requestModel)
        {
            var dataTable = CreateClientDataTable();
            var viewModel = LogicManager.GenerateDataTableResultViewModel(requestModel, dataTable);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id)
        {
            var viewModel = LogicManager.GenerateStudentDetailViewModel((EducationSecurityPrincipal)User, id);
            return View(viewModel);
        }

        public JsonResult AllFilteredStudentIds()
        {
            StudentClientDataTable requests = CreateClientDataTable();
            var studentIds = LogicManager.GetFilteredFinderStudentIds((EducationSecurityPrincipal)User, requests);
            return new JsonResult
            {
                Data = studentIds,
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        private StudentClientDataTable CreateClientDataTable()
        {
            IList<Property> studentProperties = LogicManager.FindStudentProperties().ToList();
            var currentUser = ((EducationSecurityPrincipal)HttpContext.User);
            return new StudentClientDataTable(Request, currentUser, studentProperties);
        }
    }
}
