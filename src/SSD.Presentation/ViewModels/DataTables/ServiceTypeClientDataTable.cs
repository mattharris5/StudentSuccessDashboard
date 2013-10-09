using SSD.Domain;
using SSD.Security;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    public class ServiceTypeClientDataTable : BaseClientDataTable<ServiceType>
    {
        public ServiceTypeClientDataTable(HttpRequestBase request, EducationSecurityPrincipal user)
            : base(request)
        {
            ServiceTypeName = ExtractFilterValue("ServiceTypeName");
            IsAdministrator = user.IsInRole(SecurityRoles.DataAdmin);
        }

        public string ServiceTypeName { get; private set; }
        public bool IsAdministrator { get; private set; }

        public override Expression<Func<ServiceType, string>> SortSelector
        {
            get
            {
                if (SortColumnIndex == 0)
                {
                    return s => s.IsPrivate ? "True" : "False";
                }
                if (SortColumnIndex == 3)
                {
                    return s => s.Description;
                }
                return s => s.Name;
            }
        }

        public override Expression<Func<ServiceType, object>> DataSelector
        {
            get
            {
                return s => new
                {
                    IsPrivate = s.IsPrivate,
                    IsEditable = IsAdministrator,
                    Name = s.Name,
                    Programs = s.ServiceOfferings.Where(so => so.IsActive).Select(so => so.Program).Where(g => g.IsActive).Select(g => g.Name).Distinct(),
                    Description = s.Description,
                    Id = s.Id
                };
            }
        }

        public override Expression<Func<ServiceType, bool>> FilterPredicate
        {
            get
            {
                Expression<Func<ServiceType, bool>> filterPredicate = s => s.IsActive;
                if (ServiceTypeName != null)
                {
                    filterPredicate = filterPredicate.AndAlso(s => s.Name.ToLower().Contains(ServiceTypeName.ToLower()));
                }
                return filterPredicate;
            }
        }
    }
}
