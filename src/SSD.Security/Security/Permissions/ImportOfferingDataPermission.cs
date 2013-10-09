using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Security.Permissions
{
    public class ImportOfferingDataPermission : BasePermission
    {
        public ImportOfferingDataPermission(ServiceOffering offering)
        {
            if (offering == null)
            {
                throw new ArgumentNullException("offering");
            }
            Offering = offering;
        }

        private ServiceOffering Offering { get; set; }

        public override void GrantAccess(EducationSecurityPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (user.Identity.User.UserRoles.Any())
            {
                if (IsDataAdmin(user)
                    || IsSiteCoordinator(user)
                    || IsProviderAssociatedToOffering(user, Offering))
                {
                    return;
                }
            }
            throw new EntityAccessUnauthorizedException("User not authorized to access service offering.");
        }
    }
}
