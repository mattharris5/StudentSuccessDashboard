using SSD.Domain;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    public class ProgramClientDataTable : BaseClientDataTable<Program>
    {
        public ProgramClientDataTable(HttpRequestBase request)
            : base(request)
        {
            SearchTerm = ExtractFilterValue("PartnerName");
        }

        private string SearchTerm { get; set; }

        public override Expression<Func<Program, string>> SortSelector
        {
            get { return p => p.Name; }
        }

        public override Expression<Func<Program, object>> DataSelector
        {
            get
            {
                return p => new
                {
                    Name = p.Name,
                    Contact = new
                    {
                        Name = p.ContactInfo.Name,
                        Phone = p.ContactInfo.Phone,
                        Email = p.ContactInfo.Email
                    },
                    Schools = p.Schools.Select(s => s.Name),
                    Providers = p.ServiceOfferings.Where(s => s.IsActive).Select(s => s.Provider.Name).Distinct(),
                    ServiceTypes = p.ServiceOfferings.Where(s => s.IsActive).Select(s => s.ServiceType.Name).Distinct(),
                    Id = p.Id
                };
            }
        }

        public override Expression<Func<Program, bool>> FilterPredicate
        {
            get
            {
                Expression<Func<Program, bool>> filterPredicate = s => s.IsActive;
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    filterPredicate = filterPredicate.
                        AndAlso(p => p.Name.ToLower().Contains(SearchTerm.ToLower()) || 
                            (p.ServiceOfferings.Where(s => s.IsActive)
                            .Select(s => s.Provider)
                            .Where(pr => pr.IsActive)
                            .Select(pr => pr.Name)
                            .Any(n => n.ToLower().Contains(SearchTerm.ToLower()))) ||
                            (p.ServiceOfferings.Where(s => s.IsActive)
                            .Select(s => s.ServiceType)
                            .Where(st => st.IsActive)
                            .Select(st => st.Name)
                            .Any(n => n.ToLower().Contains(SearchTerm.ToLower()))));
                }
                return filterPredicate;
            }
        }
    }
}
