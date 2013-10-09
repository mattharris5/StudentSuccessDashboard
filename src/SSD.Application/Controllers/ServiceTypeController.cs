using SSD.ActionFilters;
using SSD.Business;
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
    public class ServiceTypeController : Controller
    {
        public ServiceTypeController(IServiceTypeManager logicManager)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            LogicManager = logicManager;
        }

        private IServiceTypeManager LogicManager { get; set; }

        public JsonResult AutocompleteServiceTypeName(string term)
        {
            var filteredItems = LogicManager.SearchNames(term);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public ViewResult Index()
        {
            ServiceTypeListOptionsModel viewModel = LogicManager.GenerateListOptionsViewModel((EducationSecurityPrincipal)User);
            return View(viewModel);
        }

        public JsonResult DataTableAjaxHandler(DataTableRequestModel requestModel)
        {
            var user = ((EducationSecurityPrincipal)HttpContext.User);
            ServiceTypeClientDataTable dataTable = new ServiceTypeClientDataTable(Request, user);
            var viewModel = LogicManager.GenerateDataTableResultViewModel(requestModel, dataTable);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator")]
        [OutputCache(Location = OutputCacheLocation.None)]
        public PartialViewResult Create()
        {
            ServiceTypeModel model = LogicManager.GenerateCreateViewModel((EducationSecurityPrincipal)HttpContext.User);
            return PartialView(model);
        }

        [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator")]
        [HttpPost]
        public ActionResult Create(ServiceTypeModel viewModel)
        {
            try
            {
                LogicManager.ValidateForDuplicate(viewModel);
            }
            catch (ValidationException e)
            {
                ModelState.AddModelErrors(e);
            }
            if (ModelState.IsValid)
            {
                LogicManager.Create(viewModel);
                return Json(true);
            }
            LogicManager.PopulateViewModel((EducationSecurityPrincipal)HttpContext.User, viewModel);
            return PartialView(viewModel);
        }

        [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator")]
        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult Edit(int id)
        {
            ServiceTypeModel viewModel = LogicManager.GenerateEditViewModel((EducationSecurityPrincipal)HttpContext.User, id);
            return PartialView(viewModel);
        }

        [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator")]
        [HttpPost]
        public ActionResult Edit(ServiceTypeModel viewModel)
        {
            try
            {
                LogicManager.ValidateForDuplicate(viewModel);
                LogicManager.Edit(viewModel);
                return Json(true);
            }
            catch (ValidationException e)
            {
                ModelState.AddModelErrors(e);
                LogicManager.PopulateViewModel((EducationSecurityPrincipal)HttpContext.User, viewModel);
                return PartialView(viewModel);
            }
        }

        [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator")]
        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult Delete(int id)
        {
            var serviceType = LogicManager.GenerateDeleteViewModel(id);
            return PartialView(serviceType);
        }

        [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator")]
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

        [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator")]
        [HttpPost]
        public ActionResult SetPrivate(int id, bool isPrivate)
        {
            LogicManager.SetPrivacy((EducationSecurityPrincipal)User, id, isPrivate);
            return Json(true);
        }

        public PartialViewResult Selector()
        {
            ServiceTypeSelectorModel viewModel = LogicManager.GenerateSelectorViewModel();
            return PartialView(viewModel);
        }
    }
}
