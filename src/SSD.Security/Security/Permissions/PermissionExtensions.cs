using System;

namespace SSD.Security.Permissions
{
    public static class PermissionExtensions
    {
        public static bool TryGrantAccess(this IPermission permission, EducationSecurityPrincipal user)
        {
            if (permission == null)
            {
                throw new ArgumentNullException("permission");
            }
            try
            {
                permission.GrantAccess(user);
                return true;
            }
            catch (EntityAccessUnauthorizedException)
            {
                return false;
            }
        }
    }
}
