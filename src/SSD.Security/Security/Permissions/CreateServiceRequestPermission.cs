using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.Security.Permissions
{
    public class CreateServiceRequestPermission : BasePermission
    {
        private IEnumerable<Student> Students { get; set; }

        public CreateServiceRequestPermission(IEnumerable<Student> students)
        {
            if (students == null)
            {
                throw new ArgumentNullException("students");
            }
            Students = students;
        }

        public override void GrantAccess(EducationSecurityPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (user.Identity.User.UserRoles.Any())
            {
                if (IsDataAdmin(user)
                    || IsSiteCoordinatorAssociatedToSchools(user, Students.Select(s => s.School)))
                {
                    return;
                }
            }
            throw new EntityAccessUnauthorizedException("Not authorized to create this request.");
        }
    }
}
