using System;
using System.IdentityModel.Services;
using System.Text;
using System.Web;

namespace SSD.Security
{
    public static class AuthenticationUtility
    {
        private static IAuthenticationModuleProvider _CurrentModuleProvider = new AuthenticationModuleProvider();

        public static IAuthenticationModuleProvider CurrentModuleProvider
        {
            get { return _CurrentModuleProvider; }
        }

        public static void EnsureRealmAudienceUri(WSFederationAuthenticationModule fam, string realm)
        {
            Uri realmUri = new Uri(realm);
            if (!fam.FederationConfiguration.IdentityConfiguration.AudienceRestriction.AllowedAudienceUris.Contains(realmUri))
            {
                fam.FederationConfiguration.IdentityConfiguration.AudienceRestriction.AllowedAudienceUris.Add(realmUri);
            }
        }

        public static void SetModuleProvider(IAuthenticationModuleProvider moduleProvider)
        {
            _CurrentModuleProvider = moduleProvider;
        }

        public static EducationSecurityPrincipal GetHttpContextPrincipal()
        {
            if (HttpContext.Current == null)
            {
                return null;
            }
            return HttpContext.Current.User as EducationSecurityPrincipal;
        }

        public static string GetApplicationUri(HttpRequestBase request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            return GetUrl(request, request.ApplicationPath);
        }

        private static string GetUrl(HttpRequestBase request, string finalUrlSegments)
        {
            Uri requestUrl = request.Url;
            var wreply = new StringBuilder();

            wreply.Append(requestUrl.Scheme);
            wreply.Append("://");
            wreply.Append(request.Headers["Host"] ?? requestUrl.Authority);
            wreply.Append(finalUrlSegments);

            if (!request.ApplicationPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                wreply.Append("/");
            }
            return wreply.ToString();
        }
    }
}
