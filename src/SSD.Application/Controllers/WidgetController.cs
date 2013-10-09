using SSD.ActionFilters;
using SSD.Business;
using System;
using System.Web.Mvc;
using System.Web.UI;

namespace SSD.Controllers
{
    [AuthenticateAndAuthorize]
    public class WidgetController : Controller
    {
        public WidgetController(IWidgetManager logicManager)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            LogicManager = logicManager;
        }

        private IWidgetManager LogicManager { get; set; }

        [OutputCache(Location = OutputCacheLocation.None)]
        public PartialViewResult ServiceTypeMetrics()
        {
            var viewModel = LogicManager.GenerateServiceTypeMetricModels();
            return PartialView(viewModel);
        }

        [AuthenticateAndAuthorize]
        [OutputCache(Location = OutputCacheLocation.None)]
        public PartialViewResult ServiceRequestsBySchool()
        {
            var viewModel = LogicManager.GenerateServiceRequestsBySchoolModel();
            return PartialView(viewModel);
        }
    }
}
