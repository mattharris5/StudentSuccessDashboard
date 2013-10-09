using SSD.ActionFilters;
using SSD.Business;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.UI;

namespace SSD.Controllers
{
    [AuthenticateAndAuthorize(Roles = "Administrator,Data Admin")]
    [RequireHttps]
    public class UserController : Controller
    {
        public UserController(IAccountManager logicManager, ISecurityConfiguration securityConfiguration)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            if (securityConfiguration == null)
            {
                throw new ArgumentNullException("securityConfiguration");
            }
            LogicManager = logicManager;
            SecurityConfiguration = securityConfiguration;
        }

        private IAccountManager LogicManager { get; set; }
        private ISecurityConfiguration SecurityConfiguration { get; set; }

        public ViewResult Index()
        {
            var userViewModel = LogicManager.GenerateListViewModel();
            return View(userViewModel);
        }

        public ViewResult AccessAudit(int id)
        {
            UserModel viewModel = LogicManager.GenerateUserAccessChangeEventModel(id);
            return View(viewModel);
        }

        public ViewResult LoginAudit(int id)
        {
            UserModel viewModel = LogicManager.GenerateUserLoginEventModel(id);
            return View(viewModel);
        }

        public JsonResult DataTableAjaxHandler(DataTableRequestModel requestModel)
        {
            UserClientDataTable dataTable = new UserClientDataTable(Request, SecurityConfiguration);
            var viewModel = LogicManager.GenerateDataTableResultViewModel(requestModel, dataTable);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AccessAuditDataTableAjaxHandler(DataTableRequestModel requestModel)
        {
            AuditAccessClientDataTable dataTable = new AuditAccessClientDataTable(Request);
            var viewModel = LogicManager.GenerateAuditAccessDataTableResultViewModel(requestModel, dataTable);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoginAuditDataTableAjaxHandler(DataTableRequestModel requestModel)
        {
            AuditLoginClientDataTable dataTable = new AuditLoginClientDataTable(Request);
            var viewModel = LogicManager.GenerateAuditLoginDataTableResultViewModel(requestModel, dataTable);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutocompleteFirstName(string term)
        {
            var filteredItems = LogicManager.SearchFirstNames(term);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutocompleteLastName(string term)
        {
            var filteredItems = LogicManager.SearchLastNames(term);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutocompleteEmail(string term)
        {
            var filteredItems = LogicManager.SearchEmails(term);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AllFilteredUserIds()
        {
            UserClientDataTable requests = new UserClientDataTable(Request, SecurityConfiguration);
            var items = LogicManager.GetFilteredUserIds(requests);
            return Json(items, JsonRequestBehavior.DenyGet);
        }

        public ActionResult CreateRole(int id)
        {
            var viewModel = LogicManager.GenerateCreateViewModel(id);
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult CreateRole(UserRoleModel viewModel)
        {
            try
            {
                LogicManager.Create(viewModel, (EducationSecurityPrincipal)User);
            }
            catch (ValidationException e)
            {
                ModelState.AddModelErrors(e);
                LogicManager.PopulateViewModel(viewModel);
                return PartialView(viewModel);
            }
            return Json(true);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult EditRole(int id)
        {
            var viewModel = LogicManager.GenerateEditViewModel(id);
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult EditRole(UserRoleModel viewModel)
        {
            LogicManager.Edit(viewModel, (EducationSecurityPrincipal)User);
            return Json(true);
        }

        public ActionResult UserAssociations(int id)
        {
            var viewModel = LogicManager.GenerateUserAssociationsViewModel(id);
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult ToggleActivation(int id, bool activeStatus)
        {
            LogicManager.UpdateActiveStatus(id, activeStatus, (EducationSecurityPrincipal)User);
            return Json(true);
        }

        [HttpPost]
        public ActionResult MultiToggleActivation(IEnumerable<int> ids, bool activeStatus)
        {
            LogicManager.UpdateActiveStatus(ids, activeStatus, (EducationSecurityPrincipal)User);
            return Json(true);
        }

        public PartialViewResult MultiUserActivation(bool activeStatus, string activationString) 
        {
            var model = LogicManager.GenerateMultiUserActivationViewModel(new List<int>(), activeStatus, activationString);
            return PartialView(model);
        } 
    }
}
