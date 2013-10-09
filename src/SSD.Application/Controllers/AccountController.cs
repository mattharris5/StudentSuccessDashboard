using Microsoft.WindowsAzure;
using SSD.ActionFilters;
using SSD.Business;
using SSD.Security;
using SSD.ViewModels;
using System;
using System.Web;
using System.Web.Mvc;

namespace SSD.Controllers
{
    public class AccountController : Controller
    {
        private IAccountManager LogicManager { get; set; }

        public AccountController(IAccountManager logicManager)
        {
            if (logicManager == null)
            {
                throw new ArgumentNullException("logicManager");
            }
            LogicManager = logicManager;
        }

        public ViewResult Login()
        {
            LoginModel model = new LoginModel();
            model.Realm = AuthenticationUtility.GetApplicationUri(HttpContext.Request);
            model.Namespace = CloudConfigurationManager.GetSetting("AcsNamespace");
            return View(model);
        }

        [AuthenticateAndAuthorize]
        [RequireHttps]
        public ViewResult UserProfile()
        {
            UserModel viewModel = LogicManager.GenerateUserProfileViewModel((EducationSecurityPrincipal)User);
            return View(viewModel);
        }

        [HttpPost]
        [AuthenticateAndAuthorize]
        [RequireHttps]
        public ActionResult UserProfile(UserModel viewModel)
        {
            if (ModelState.IsValid)
            {
                UrlHelper helper = new UrlHelper(Request.RequestContext);
                LogicManager.Edit(viewModel, helper);
                return RedirectToAction("Index", "Home");
            }
            return View(viewModel);
        }

        public ViewResult ConfirmEmail(Guid identifier)
        {
            ConfirmEmailModel viewModel = LogicManager.GenerateConfirmEmailViewModel(identifier);
            return View(viewModel);
        }

        public RedirectResult LogOn(string federationLocation)
        {
            var module = AuthenticationUtility.CurrentModuleProvider.GetModule();
            var realm = AuthenticationUtility.GetApplicationUri(HttpContext.Request);
            AuthenticationUtility.EnsureRealmAudienceUri(module, realm);
            return new RedirectResult(federationLocation);
        }

        public RedirectToRouteResult LogOff()
        {
            AuthenticationUtility.CurrentModuleProvider.GetModule().SignOut(false);
            return RedirectToAction("Index", "Home");
        }
    }
}
