using SSD.Domain;
using SSD.Security;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class AuthenticateAndAuthorizeAttribute : AuthorizeAttribute
    {
        public AuthenticateAndAuthorizeAttribute()
        {
            Order = 20;
        }

        public User UserEntity { get; private set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                AuthorizeUser(filterContext);
            }
            else
            {
                AuthenticateUser(filterContext);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            ViewResult result = new ViewResult { ViewName = "Unauthorized" };
            User user = UserEntity as User;
            if (user != null)
            {
                result.ViewData.Model = user.UserRoles.Select(r => r.Role.Name);
            }
            filterContext.Result = result;
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }

        private void AuthorizeUser(AuthorizationContext filterContext)
        {
            EnsureUser(filterContext);
            if (AuthorizeCore(filterContext.HttpContext))
            {
                if (!filterContext.HttpContext.Request.FilePath.Equals("/Account/UserProfile") && !UserEntity.IsValidUserInformation)
                {
                    filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary { { "action", "UserProfile" }, { "controller", "Account" } });
                }
            }
            else
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }

        private void EnsureUser(AuthorizationContext filterContext)
        {
            EducationSecurityPrincipal user = filterContext.HttpContext.User as EducationSecurityPrincipal;
            if (user == null)
            {
                user = UserIdentityMapAttribute.MapFrom(filterContext);
                filterContext.HttpContext.User = user;
            }
            UserEntity = user.Identity.UserEntity;
        }

        private void AuthenticateUser(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.FilePath.Equals("/Account/Login"))
            {
                filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary { { "action", "Login" }, { "controller", "Account" } });
            }
            UserEntity = null;
        }
    }
}