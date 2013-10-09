using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Security.Permissions
{
    public class ManageAssignedOfferingPermission : BasePermission
    {
        public ManageAssignedOfferingPermission(StudentAssignedOffering studentAssignedOffering)
        {
            if (studentAssignedOffering == null)
            {
                throw new ArgumentNullException("assignedOffering");
            }
            StudentAssignedOffering = studentAssignedOffering;
        }

        private StudentAssignedOffering StudentAssignedOffering { get; set; }

        public override void GrantAccess(EducationSecurityPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (StudentAssignedOffering.IsActive)
            {
                if (user.Identity.User.UserRoles.Any())
                {
                    if (IsDataAdmin(user)
                        || IsCreatingUser(user, StudentAssignedOffering)
                        || (!StudentAssignedOffering.ServiceOffering.ServiceType.IsPrivate && IsSiteCoordinatorAssociatedToSchools(user, new[] { StudentAssignedOffering.Student.School })
                            || IsApprovedProviderAssociatedToStudentOfferings(user, StudentAssignedOffering.Student)))
                    {
                        return;
                    }
                }
            }
            else
            {
                throw new EntityNotFoundException("StudentAssignedOffering");
            }
            throw new EntityAccessUnauthorizedException("Not authorized to manage this assigned offering.");
        }
    }
}
