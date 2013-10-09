using SSD.Domain;
using SSD.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    public class ProviderClientDataTable : BaseClientDataTable<Provider>
    {
        public ProviderClientDataTable(HttpRequestBase request, EducationSecurityPrincipal currentUser)
            : base(request)
        {
            PartnerName = ExtractFilterValue("PartnerName");
            IsAdministrator = currentUser.IsInRole(SecurityRoles.DataAdmin);
            ProviderIds = currentUser.Identity.User.UserRoles.SelectMany(u => u.Providers).Select(p => p.Id);
        }

        public bool IsAdministrator { get; private set; }
        public IEnumerable<int> ProviderIds { get; private set; }
        public string PartnerName { get; private set; }

        public override Expression<Func<Provider, string>> SortSelector
        {
            get
            {
                if (SortColumnIndex == 1)
                {
                    return p => p.Contact.Name;
                }
                return p => p.Name;
            }
        }

        public override Expression<Func<Provider, object>> DataSelector
        {
            get
            {
                return p => new
                {
                    Name = p.Name,
                    Address = new
                    {
                        Street = p.Address.Street,
                        City = p.Address.City,
                        State = p.Address.State,
                        Zip = p.Address.Zip
                    },
                    Contact = new
                    {
                        Name = p.Contact.Name,
                        Phone = p.Contact.Phone,
                        Email = p.Contact.Email
                    },
                    Website = p.Website,
                    Schools = p.ServiceOfferings.Where(s => s.IsActive).Select(s => s.Program).Where(g => g.IsActive).SelectMany(g => g.Schools.Select(s => s.Name)).Distinct(),
                    Programs = p.ServiceOfferings.Where(s => s.IsActive).Select(s => s.Program).Where(g => g.IsActive).Select(g => g.Name).Distinct(),
                    Id = p.Id,
                    AccessMode = IsAdministrator ? "All" : ProviderIds.Contains(p.Id) ? "Edit" : "View"
                };
            }
        }

        public override Expression<Func<Provider, bool>> FilterPredicate
        {
            get
            {
                Expression<Func<Provider, bool>> filterPredicate = p => p.IsActive;
                if (!string.IsNullOrEmpty(PartnerName))
                {
                    filterPredicate = filterPredicate.AndAlso(p => p.Name.ToLower().Contains(PartnerName.ToLower()) || p.ServiceOfferings.Select(s => s.Program).Select(g => g.Name).Any(n => n.ToLower().Contains(PartnerName.ToLower())));
                }
                return filterPredicate;
            }
        }
    }
}
