using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Security.Permissions
{
    public class ManageServiceAttendancePermission : BasePermission
    {
        public ManageServiceAttendancePermission(StudentAssignedOffering studentAssignedOffering)
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
            if (user.Identity.User.UserRoles.Any())
            {
                if (IsDataAdmin(user)
                    || IsSiteCoordinatorAssociatedToSchools(user, new[] { StudentAssignedOffering.Student.School })
                    || IsProviderAssociatedToOffering(user, StudentAssignedOffering.ServiceOffering))
                {
                    return;
                }
            }
            throw new EntityAccessUnauthorizedException("Not authorized to manage attendance for this assigned offering.");
        }
    }
}
