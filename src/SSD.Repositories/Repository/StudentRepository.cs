using SSD.Data;
using SSD.Domain;
using SSD.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSD.Repository
{
    public class StudentRepository : IStudentRepository
    {
        public StudentRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(Student item)
        {
            throw new NotImplementedException();
        }

        public void Remove(Student item)
        {
            throw new NotImplementedException();
        }

        public void Update(Student item)
        {
            Context.SetModified(item);
        }

        public IQueryable<Student> Items
        {
            get { return Context.Students; }
        }

        public void AddLink(Student student, Provider provider)
        {
            if (student == null)
            {
                throw new ArgumentNullException("student");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            student.ApprovedProviders.Add(provider);
            provider.ApprovingStudents.Add(student);
        }

        public void DeleteLink(Student student, Provider provider)
        {
            if (student == null)
            {
                throw new ArgumentNullException("student");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            student.ApprovedProviders.Remove(provider);
            provider.ApprovingStudents.Remove(student);
        }

        public IQueryable<Student> GetAllowedList(EducationSecurityPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var associatedSchoolsIds = user.Identity.User.UserRoles.SelectMany(ur => ur.Schools).Select(s => s.Id);
            var isAdministrator = user.IsInRole(SecurityRoles.DataAdmin);
            return Items.Where(s => isAdministrator || !s.HasParentalOptOut || associatedSchoolsIds.Contains(s.SchoolId));
        }

        public void ResetApprovals()
        {
            var studentsWithApprovals = Items.Include(s => s.ApprovedProviders).Where(s => s.ApprovedProviders.Any());
            ResetApprovals(studentsWithApprovals);
        }

        public void ResetApprovals(IEnumerable<int> schoolIds)
        {
            if (schoolIds == null)
            {
                throw new ArgumentNullException("schoolIds");
            }
            var studentsWithApprovals = Items.Include(s => s.ApprovedProviders).Where(s => schoolIds.Contains(s.SchoolId) && s.ApprovedProviders.Any());
            ResetApprovals(studentsWithApprovals);
        }

        private void ResetApprovals(IQueryable<Student> studentsWithApprovals)
        {
            foreach (Student student in studentsWithApprovals)
            {
                foreach (Provider approved in student.ApprovedProviders)
                {
                    approved.ApprovingStudents.Clear();
                }
                student.ApprovedProviders.Clear();
            }
        }
    }
}
