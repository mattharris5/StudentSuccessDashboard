using SSD.Domain;
using SSD.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    public class ServiceOfferingClientDataTable : BaseClientDataTable<ServiceOffering>
    {
        public ServiceOfferingClientDataTable(HttpRequestBase request, EducationSecurityPrincipal currentUser)
            : base(request)
        {
            ServiceTypeProviderOrProgramName = ExtractFilterValue("ServiceTypeProviderOrProgram");
            ServiceTypes = ExtractFilterList("ServiceTypeFilters");
            ServiceCategories = ExtractFilterList("ServiceCategoryFilters");
            CurrentUser = currentUser.Identity.User;
            CanManageAll = currentUser.IsInRole(SecurityRoles.DataAdmin) || currentUser.IsInRole(SecurityRoles.SiteCoordinator);
            UserAssociatedProviderIds = currentUser.Identity.User.UserRoles.SelectMany(u => u.Providers).Select(p => p.Id);
        }

        public string ServiceTypeProviderOrProgramName { get; private set; }
        public IEnumerable<string> ServiceTypes { get; private set; }
        public IEnumerable<string> ServiceCategories { get; private set; }
        public User CurrentUser { get; private set; }
        public bool CanManageAll { get; private set; }
        public IEnumerable<int> UserAssociatedProviderIds { get; private set; }

        public override Expression<Func<ServiceOffering, string>> SortSelector
        {
            get
            {
                if (SortColumnIndex == 0)
                {
                    return o => o.ServiceType.Name;
                }
                return o => o.Provider.Name;
            }
        }

        public override Expression<Func<ServiceOffering, object>> DataSelector
        {
            get
            {
                return s => new
                {
                    Id = s.Id,
                    IsFavorite = s.UsersLinkingAsFavorite.Select(f => f.Id).Contains(CurrentUser.Id),
                    IsPrivate = s.ServiceType.IsPrivate,
                    ServiceType = s.ServiceType.Name,
                    Provider = s.Provider.Name,
                    Program = s.Program.Name,
                    CanInteract = CanManageAll || UserAssociatedProviderIds.Contains(s.ProviderId)
                };
            }
        }

        public override Expression<Func<ServiceOffering, bool>> FilterPredicate
        {
            get
            {
                Expression<Func<ServiceOffering, bool>> filterPredicate = s => s.IsActive;
                if (ServiceTypeProviderOrProgramName != null)
                {
                    filterPredicate = filterPredicate.AndAlso(s => s.ServiceType.Name.ToLower().Contains(ServiceTypeProviderOrProgramName.ToLower()) ||
                                                                   s.Provider.Name.ToLower().Contains(ServiceTypeProviderOrProgramName.ToLower()) ||
                                                                   s.Program.Name.ToLower().Contains(ServiceTypeProviderOrProgramName.ToLower()));
                }
                if (ServiceTypes.Any())
                {
                    filterPredicate = filterPredicate.AndAlso(s => ServiceTypes.Any(t => t.Contains(s.ServiceType.Name)));
                }
                if (ServiceCategories.Any())
                {
                    filterPredicate = filterPredicate.AndAlso(s => ServiceCategories.Intersect(s.ServiceType.Categories.Select(c => c.Name)).Any());
                }
                return filterPredicate;
            }
        }
    }
}
