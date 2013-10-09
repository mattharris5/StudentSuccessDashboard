using SSD.ActionFilters;
using SSD.Business;
using SSD.Security;
using SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;

namespace SSD.Controllers
{
    [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator,Provider")]
    [RequireHttps]
    public class ServiceController : Controller
    {
        public ServiceController(IScheduledServiceManager logicManager)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            LogicManager = logicManager;
        }

        private IScheduledServiceManager LogicManager { get; set; }

        [HttpPost]
        [KeepTempDataAttribute(TempDataKey="ScheduleOfferingIds")]
        public JsonResult SaveStudentIds(IEnumerable<int> students, string returnUrl)
        {
            TempData["ScheduleOfferingIds"] = students;
            TempData["ScheduleOfferingReturnUrl"] = returnUrl;
            return Json(true);
        }

        [KeepTempDataAttribute(TempDataKey="ScheduleOfferingIds,ScheduleOfferingReturnUrl")]
        public ActionResult ScheduleOffering()
        {
            if (!TempData.ContainsKey("ScheduleOfferingIds"))
            {
                var redirectingUrl = Request.QueryString["ReturnUrl"] as string;
                if (redirectingUrl != null)
                {
                    return Redirect(redirectingUrl);
                }
                return RedirectToAction("Finder", "Student");
            }
            IEnumerable<int> students = (IEnumerable<int>)TempData["ScheduleOfferingIds"];
            ScheduleServiceOfferingListOptionsModel viewModel = LogicManager.GenerateScheduleOfferingViewModel((EducationSecurityPrincipal)User, students);
            viewModel.ReturnUrl = string.Empty;
            if (TempData.ContainsKey("ScheduleOfferingReturnUrl"))
            {
                viewModel.ReturnUrl = (string)TempData["ScheduleOfferingReturnUrl"];
            }
            return View(viewModel);
        }

        public ActionResult CreateScheduledOffering(int id)
        {
            ServiceOfferingScheduleModel viewModel = LogicManager.GenerateCreateViewModel(id);
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult CreateScheduledOffering(ServiceOfferingScheduleModel viewModel)
        {
            LogicManager.Create((EducationSecurityPrincipal)User, viewModel);
            return Json(true);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult EditScheduledOffering(int id)
        {
            StudentServiceOfferingScheduleModel viewModel = LogicManager.GenerateEditViewModel((EducationSecurityPrincipal)User, id);
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult EditScheduledOffering(StudentServiceOfferingScheduleModel viewModel)
        {
            LogicManager.Edit((EducationSecurityPrincipal)User, viewModel);
            return Json(true);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult DeleteScheduledOffering(int id)
        {
            DeleteServiceOfferingScheduleModel viewModel = LogicManager.GenerateDeleteViewModel((EducationSecurityPrincipal)User, id);
            return PartialView(viewModel);
        }

        [HttpPost, ActionName("DeleteScheduledOffering")]
        public ActionResult DeleteScheduledOfferingConfirmed(int id)
        {
            LogicManager.Delete((EducationSecurityPrincipal)User, id);
            return Json(true);
        }
    }
}
