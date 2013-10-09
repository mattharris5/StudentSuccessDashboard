using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Security.Permissions
{
    public class ViewStudentDetailPermission : BasePermission, IViewStudentDetailPermission
    {
        public ViewStudentDetailPermission(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException("student");
            }
            Student = student;
        }

        private Student Student { get; set; }

        public override void GrantAccess(EducationSecurityPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            CustomFieldOnly = false;
            if (user.Identity.User.UserRoles.Any())
            {
                if (IsDataAdmin(user)
                    || IsSiteCoordinatorAssociatedToSchools(user, new[] { Student.School })
                    || IsApprovedProviderAssociatedToStudentOfferings(user, Student))
                {
                    return;
                }
                if (!Student.HasParentalOptOut)
                {
                    if (Student.CustomFieldValues.Any(f => IsCreatingUser(user, f.CustomDataOrigin)))
                    {
                        CustomFieldOnly = true;
                        return;
                    }
                }
            }
            throw new EntityAccessUnauthorizedException("Not authorized to view this student.");
        }

        public bool CustomFieldOnly { get; private set; }
    }
}
