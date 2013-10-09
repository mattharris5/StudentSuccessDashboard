using System.Linq;

namespace SSD.Security.Permissions
{
    public class ManageProviderPermission : BasePermission
    {
        public ManageProviderPermission(int providerId)
        {
            ProviderId = providerId;
        }

        private int ProviderId { get; set; }

        public override void GrantAccess(EducationSecurityPrincipal user)
        {
            if (user.Identity.User.UserRoles.Any())
            {
                if (IsDataAdmin(user)
                    || IsSiteCoordinator(user)
                    || (IsProvider(user) && user.Identity.User.UserRoles.SelectMany(u => u.Providers).Any(p => p.Id == ProviderId)))
                {
                    return;
                }
            }
            throw new EntityAccessUnauthorizedException("Not authorized to manage the specified provider.");
        }
    }
}
