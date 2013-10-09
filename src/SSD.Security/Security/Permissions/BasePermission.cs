using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.Security.Permissions
{
    public abstract class BasePermission : IPermission
    {
        public abstract void GrantAccess(EducationSecurityPrincipal user);

        protected bool IsDataAdmin(EducationSecurityPrincipal user)
        {
            return user.IsInRole(SecurityRoles.DataAdmin);
        }

        protected bool IsSiteCoordinator(EducationSecurityPrincipal user)
        {
            return user.IsInRole(SecurityRoles.SiteCoordinator);
        }

        protected bool IsSiteCoordinatorAssociatedToSchools(EducationSecurityPrincipal user, IEnumerable<School> schools)
        {
            return IsSiteCoordinator(user) && schools.All(s => user.Identity.User.UserRoles.SelectMany(u => u.Schools).Contains(s));
        }

        protected bool IsProvider(EducationSecurityPrincipal user)
        {
            return user.IsInRole(SecurityRoles.Provider);
        }

        protected bool IsProviderAssociatedToOffering(EducationSecurityPrincipal user, ServiceOffering offering)
        {
            return IsProvider(user) && user.Identity.User.UserRoles.SelectMany(u => u.Providers).Contains(offering.Provider);
        }

        protected bool IsApprovedProviderAssociatedToStudentOfferings(EducationSecurityPrincipal user, Student student)
        {
            return (IsProviderAssociatedToStudentOfferings(user, student) &&
                user.Identity.User.UserRoles.SelectMany(u => u.Providers).Intersect(student.ApprovedProviders).Any());
        }

        protected bool IsProviderAssociatedToStudentOfferings(EducationSecurityPrincipal user, Student student)
        {
            return IsProvider(user) && 
                user.Identity.User.UserRoles.SelectMany(u => u.Providers).Intersect(student.StudentAssignedOfferings.Where(s => s.IsActive).Select(s => s.ServiceOffering.Provider)).Any();
        }

        protected bool IsCreatingUser(EducationSecurityPrincipal user, IAuditCreate entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            return entity.CreatingUserId == user.Identity.User.Id;
        }
    }
}
