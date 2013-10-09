using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.Security.Permissions
{
    public class ScheduleOfferingPermission : BasePermission
    {
        public ScheduleOfferingPermission(IEnumerable<Student> students)
        {
            if (students == null)
            {
                throw new ArgumentNullException("students");
            }
            if (!students.Any())
            {
                throw new ArgumentException("List cannot be empty.", "students");
            }
            Students = students;
        }

        public ScheduleOfferingPermission(IEnumerable<Student> students, ServiceOffering offering)
        {
            if (students == null)
            {
                throw new ArgumentNullException("students");
            }
            if (offering == null)
            {
                throw new ArgumentNullException("offering");
            }
            if (!students.Any())
            {
                throw new ArgumentException("List cannot be empty.", "students");
            }
            Students = students;
            Offering = offering;
        }

        private IEnumerable<Student> Students { get; set; }
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
                    || ((Offering == null && IsProvider(user)) ||IsProviderAssociatedToOffering(user, Offering))
                    || IsSiteCoordinatorAssociatedToSchools(user, Students.Select(s => s.School)))
                {
                    return;
                }
            }
            throw new EntityAccessUnauthorizedException("Not authorized to schedule offerings to all students.");
        }
    }
}
