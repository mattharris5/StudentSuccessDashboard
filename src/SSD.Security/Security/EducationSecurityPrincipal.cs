using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace SSD.Security
{
    public sealed class EducationSecurityPrincipal : IPrincipal
    {
        public EducationSecurityPrincipal(User userEntity)
        {
            if (userEntity == null)
            {
                throw new ArgumentNullException("userEntity");
            }
            if (userEntity.UserKey == null)
            {
                throw new ArgumentException("UserKey property cannot be null.", "userEntity");
            }
            Claim claim = new Claim(ClaimTypes.NameIdentifier, userEntity.UserKey);
            Identity = new EducationSecurityIdentity(new ClaimsIdentity(new List<Claim> { claim }, "Custom"), userEntity);
            Configuration = new DefaultSecurityConfiguration();
        }

        public EducationSecurityPrincipal(EducationSecurityIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }
            Identity = identity;
            Configuration = new DefaultSecurityConfiguration();
        }

        public EducationSecurityIdentity Identity { get; private set; }
        public ISecurityConfiguration Configuration { get; set; }

        IIdentity IPrincipal.Identity
        {
            get { return Identity; }
        }

        public bool IsInRole(string role)
        {
            if (role == SecurityRoles.Administrator)
            {
                return IsAdministrator(Identity.UserEntity, Configuration);
            }
            if (Identity.User == null)
            {
                return false;
            }
            return Identity.User.UserRoles.Select(r => r.Role.Name).Contains(role);
        }

        public static bool IsAdministrator(User userEntity, ISecurityConfiguration configuration)
        {
            if (userEntity == null)
            {
                throw new ArgumentNullException("userEntity");
            }
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            return configuration.AdministratorEmailAddresses.Contains(userEntity.EmailAddress, StringComparer.OrdinalIgnoreCase);
        }

        public static string FindUserKey(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                throw new ArgumentNullException("claimsPrincipal");
            }
            return EducationSecurityIdentity.FindUserKey((ClaimsIdentity)claimsPrincipal.Identity);
        }
    }
}
