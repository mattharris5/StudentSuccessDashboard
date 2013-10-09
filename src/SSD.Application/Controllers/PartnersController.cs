using SSD.ActionFilters;
using SSD.Business;
using SSD.Security;
using SSD.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SSD.Controllers
{
    [AuthenticateAndAuthorize(Roles = "Data Admin,Site Coordinator,Provider")]
    [RequireHttps]
    public class PartnersController : Controller
    {
        public PartnersController(IProviderManager providerLogicManager, IProgramManager programLogicManager, IServiceTypeManager serviceTypeManager)
        {
            if (providerLogicManager == null)
            {
                throw new ArgumentNullException("providerLogicManager");
            }
            if (programLogicManager == null)
            {
                throw new ArgumentNullException("programLogicManager");
            }
            if (serviceTypeManager == null)
            {
                throw new ArgumentNullException("serviceTypeManager");
            }
            ProviderLogicManager = providerLogicManager;
            ProgramLogicManager = programLogicManager;
            ServiceTypeManager = serviceTypeManager;
        }

        private IProviderManager ProviderLogicManager { get; set; }
        private IProgramManager ProgramLogicManager { get; set; }
        private IServiceTypeManager ServiceTypeManager { get; set; }

        public ViewResult Index()
        {
            var user = (EducationSecurityPrincipal)User;
            PartnerListOptionsModel viewModel = new PartnerListOptionsModel
            {
                ShowAddProvider = user.IsInRole(SecurityRoles.DataAdmin) || user.IsInRole(SecurityRoles.SiteCoordinator)
            };
            return View(viewModel);
        }

        public JsonResult AutocompletePartnerName(string term)
        {
            var providerNames = ProviderLogicManager.SearchProviderNames(term);
            var programNames = ProgramLogicManager.SearchProgramNames(term);
            var serviceTypeNames = ServiceTypeManager.SearchNames(term);
            var filteredItems = providerNames.Union(programNames).Union(serviceTypeNames).Distinct().OrderBy(n => n);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
    }
}
