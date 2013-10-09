using SSD.Domain;
using SSD.Security;
using System.Collections.Generic;
using System.Linq;

namespace SSD.Repository
{
    public interface IStudentRepository : IRepository<Student>
    {
        void AddLink(Student student, Provider provider);
        void DeleteLink(Student student, Provider provider);
        IQueryable<Student> GetAllowedList(EducationSecurityPrincipal user);
        void ResetApprovals();
        void ResetApprovals(IEnumerable<int> schoolIds);
    }
}
