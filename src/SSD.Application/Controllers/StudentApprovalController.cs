using SSD.ActionFilters;
using SSD.Business;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Web.Mvc;
using System.Web.UI;

namespace SSD.Controllers
{
    [AuthenticateAndAuthorize(Roles = SecurityRoles.DataAdmin)]
    [RequireHttps]
    public class StudentApprovalController : Controller
    {
        public StudentApprovalController(ISchoolDistrictManager logicManager)
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
            StudentApprovalListOptionsModel viewModel = LogicManager.GenerateApprovalListOptionsViewModel();
            return View(viewModel);
        }

        public JsonResult DataTableAjaxHandler(DataTableRequestModel requestModel)
        {
            StudentApprovalClientDataTable dataTable = new StudentApprovalClientDataTable(Request);
            DataTableResultModel viewModel = LogicManager.GenerateApprovalDataTableResultViewModel(requestModel, dataTable);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddProviders(int id)
        {
            AddStudentApprovalModel viewModel = LogicManager.GenerateAddStudentApprovalViewModel(id);
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult AddProviders(AddStudentApprovalModel viewModel)
        {
            if (ModelState.IsValid)
            {
                LogicManager.AddProviders(viewModel);
                return Json(true);
            }
            LogicManager.PopulateViewModelLists(viewModel);
            return PartialView(viewModel);
        }

        public ActionResult RemoveProvider(int id, int providerId)
        {
            var viewModel = LogicManager.GenerateRemoveProviderViewModel(id, providerId);
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult RemoveProvider(RemoveApprovedProviderModel viewModel)
        {
            LogicManager.RemoveProvider(viewModel);
            return Json(true);
        }

        public PartialViewResult RemoveAllProvidersBySchool()
        {
            RemoveApprovedProvidersBySchoolModel viewModel = LogicManager.GenerateRemoveProvidersBySchoolViewModel();
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult RemoveAllProvidersBySchool(RemoveApprovedProvidersBySchoolModel viewModel)
        {
            LogicManager.RemoveAllProviders(viewModel.SelectedSchools);
            return Json(true);
        }

        [HttpPost]
        public ActionResult RemoveAllProviders()
        {
            LogicManager.RemoveAllProviders();
            return Json(true);
        }

        [HttpPost]
        public ActionResult SetOptOut(int id, bool hasParentalOptOut)
        {
            LogicManager.SetStudentOptOutState(id, hasParentalOptOut);
            return Json(true);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public JsonResult CountStudentsWithApprovedProviders()
        {
            int count = LogicManager.CountStudentsWithApprovedProviders();
            return Json(count);
        }
    }
}
