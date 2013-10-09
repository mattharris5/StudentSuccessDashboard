using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Security.Permissions
{
    public class ManageCustomFieldPermission : BasePermission
    {
        private Student Student { get; set; }

        public ManageCustomFieldPermission(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException("student");
            }
            Student = student;
        }

        public override void GrantAccess(EducationSecurityPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (user.Identity.User.UserRoles.Any())
            {
                if (IsDataAdmin(user) ||
                    IsSiteCoordinatorAssociatedToSchools(user, new[] { Student.School }) ||
                    IsProviderAssociatedToStudentOfferings(user, Student))
                {
                    return;
                }
            }
            throw new EntityAccessUnauthorizedException("You do not have access to this CustomField");
        }
    }
}
