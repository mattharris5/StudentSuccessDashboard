using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Security.Permissions
{
    public class CustomFieldDataPermission : BasePermission
    {
        public CustomFieldDataPermission(CustomField customField)
        {
            if (customField == null)
            {
                throw new ArgumentNullException("customField");
            }
            CustomField = customField;
        }

        private CustomField CustomField { get; set; }

        public override void GrantAccess(EducationSecurityPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (user.Identity.User.UserRoles.Any())
            {
                if (IsDataAdmin(user))
                {
                    return;
                }
                if (CustomField is PublicField)
                {
                    return;
                }
                else
                {
                    PrivateHealthField privateHealthField = CustomField as PrivateHealthField;
                    if (IsProvider(user) && privateHealthField.ProviderId != null && user.Identity.User.UserRoles.SelectMany(u => u.Providers).Contains(privateHealthField.Provider))
                    {
                        return;
                    }
                }
            }
            throw new EntityAccessUnauthorizedException("Not authorized to upload to this custom field.");
        }
    }
}
