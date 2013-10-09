using SSD.Business;
using SSD.Domain;
using SSD.Security;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class UserIdentityMapAttribute : FilterAttribute, IAuthorizationFilter
    {
        private static SynchronizedCollection<string> _AuditOnMapCollection = new SynchronizedCollection<string>();

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            EducationSecurityPrincipal user = filterContext.HttpContext.User as EducationSecurityPrincipal;
            if (user == null)
            {
                user = MapFrom(filterContext);
                if (user != null)
                {
                    filterContext.HttpContext.User = user;
                }
            }
        }

        public static void AuditOnMap(string userKey)
        {
            _AuditOnMapCollection.Add(userKey);
        }

        public static EducationSecurityPrincipal MapFrom(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            try
            {
                System.Security.Claims.ClaimsPrincipal claimsPrincipal = filterContext.HttpContext.User as System.Security.Claims.ClaimsPrincipal;
                if ((claimsPrincipal == null || !filterContext.HttpContext.User.Identity.IsAuthenticated)
                    || (filterContext.ActionDescriptor.ActionName == "LogOff" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Account"))
                {
                    return null;
                }
                else
                {
                    return CreateEducationSecurityPrincipal(filterContext, claimsPrincipal);
                }
            }
            catch (LicenseAgreementException)
            {
                filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary { { "action", "Index" }, { "controller", "Agreement" } });
                return null;
            }
        }

        private static EducationSecurityPrincipal CreateEducationSecurityPrincipal(AuthorizationContext filterContext, System.Security.Claims.ClaimsPrincipal claimsPrincipal)
        {
            System.Security.Claims.ClaimsIdentity claimsIdentity = (System.Security.Claims.ClaimsIdentity)claimsPrincipal.Identity;
            IAccountManager manager = DependencyResolver.Current.GetService<IAccountManager>();
            User userEntity = manager.EnsureUserEntity(claimsIdentity);
            EducationSecurityIdentity identity = new EducationSecurityIdentity(claimsIdentity, userEntity);
            EducationSecurityPrincipal principal = new EducationSecurityPrincipal(identity);
            if (_AuditOnMapCollection.Contains(userEntity.UserKey))
            {
                _AuditOnMapCollection.Remove(userEntity.UserKey);
                manager.AuditLogin(principal);
            }
            if (!(filterContext.RequestContext.RouteData.Values["action"].ToString() == "Index" && filterContext.RequestContext.RouteData.Values["controller"].ToString() == "Agreement"))
            {
                manager.ValidateEulaAccepted(principal.Identity.User);
            }
            return principal;
        }
    }
}
