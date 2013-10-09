using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSD.Business
{
    public static class LookupHelper
    {
        public static IList<Subject> LoadSubjectList(ISubjectRepository repository)
        {
            var subjects = repository.Items.OrderBy(s => s.Name).ToList();
            var noneSubject = subjects.SingleOrDefault(s => s.Name.Equals(Subject.DefaultName));
            subjects.Remove(noneSubject);
            subjects.Insert(0, noneSubject);
            return subjects;
        }

        public static IList<ServiceOffering> LoadFavorites(IServiceOfferingRepository repository, EducationSecurityPrincipal user)
        {
            var materializedServiceOfferings = repository.Items.
                                               Include(s => s.Provider).
                                               Include(s => s.ServiceType).
                                               Include(s => s.Program).
                                               Include(s => s.UsersLinkingAsFavorite);
            return materializedServiceOfferings.Where(s => s.UsersLinkingAsFavorite.Select(u => u.Id).Contains(user.Identity.User.Id) && s.IsActive).ToList();
        }
    }
}
