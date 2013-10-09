using System;

namespace SSD.Security.Permissions
{
    public class SetServiceTypePrivacyPermission : BasePermission
    {
        public override void GrantAccess(EducationSecurityPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (IsDataAdmin(user))
            {
                return;
            }
            throw new EntityAccessUnauthorizedException("Not authorized");
        }
    }
}
