using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using SSD.ActionFilters;
using SSD.Security;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SSD
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            if (!RoleEnvironment.IsAvailable)
            {
                throw new InvalidOperationException("This application must run in Windows Azure or the Windows Azure Emulator.");
            }
            Trace.Listeners.Add(new DiagnosticMonitorTraceListener());
            DependencyInjectionConfig.AssemblySearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
            DependencyInjectionConfig.RegisterDependencyInjection();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            PrepareAuthentication();
        }

        private static void PrepareAuthentication()
        {
            AuthenticationModuleProvider.Initialize(SignedOnCallback);
        }

        private static void SignedOnCallback()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.Current.User as ClaimsPrincipal;
            string userKey = EducationSecurityPrincipal.FindUserKey(claimsPrincipal);
            UserIdentityMapAttribute.AuditOnMap(userKey);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            DependencyInjectionConfig.ReleaseDependencyInjection();
        }

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (custom == "Host")
            {
                return context.Request.Url.Host;
            }
            return String.Empty;
        }
    }
}