using SSD.Domain;
using SSD.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    public class UserClientDataTable : BaseClientDataTable<User>
    {
        public UserClientDataTable(HttpRequestBase request, ISecurityConfiguration securityConfiguration)
            : base(request)
        {
            if (securityConfiguration == null)
            {
                throw new ArgumentNullException("securityConfiguration");
            }
            SecurityConfiguration = securityConfiguration;
            FirstName = ExtractFilterValue("firstName");
            LastName = ExtractFilterValue("lastName");
            Email = ExtractFilterValue("email");
            Schools = ExtractFilterList("schools");
            Statuses = ExtractFilterList("status");
            Roles = ExtractFilterList("roles");
        }

        private ISecurityConfiguration SecurityConfiguration { get; set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public IEnumerable<string> Schools { get; private set; }
        public IEnumerable<string> Statuses { get; private set; }
        public IEnumerable<string> Roles { get; private set; }

        public override Expression<Func<User, string>> SortSelector
        {
            get
            {
                if (SortColumnIndex == 0)
                {
                    return u => u.Status;
                }
                if (SortColumnIndex == 1)
                {
                    return u => u.LastName;
                }
                if (SortColumnIndex == 2)
                {
                    return u => u.FirstName;
                }
                if (SortColumnIndex == 3)
                {
                    return u => RepositoryFunctions.StringConvert(RepositoryFunctions.DateDiff("mi", DateTime.MinValue, u.LastLoginTime));
                }
                return u => RepositoryFunctions.StringConvert((double)(u.UserRoles.SelectMany(r => r.Schools).Distinct().Count() + u.UserRoles.SelectMany(r => r.Providers).Distinct().Count()));
            }
        }

        public override Expression<Func<User, bool>> FilterPredicate
        {
            get
            {
                Expression<Func<User, bool>> filterPredicate = u => true;
                if (FirstName != null)
                {
                    filterPredicate = filterPredicate.AndAlso(u => u.FirstName.ToLower().Contains(FirstName.ToLower()));
                }
                if (LastName != null)
                {
                    filterPredicate = filterPredicate.AndAlso(u => u.LastName.ToLower().Contains(LastName.ToLower()));
                }
                if (Email != null)
                {
                    filterPredicate = filterPredicate.AndAlso(u => u.EmailAddress.ToLower().Contains(Email.ToLower()));
                }
                if (Statuses.Any())
                {
                    filterPredicate = filterPredicate.AndAlso(u => Statuses.Contains(u.Status));
                }
                if (Roles.Any())
                {
                    filterPredicate = filterPredicate.AndAlso(u => u.UserRoles.Select(r => r.Role.Name).Any(name => Roles.Contains(name)));
                }
                if (Schools.Any())
                {
                    filterPredicate = filterPredicate.AndAlso(u => u.UserRoles.SelectMany(s => s.Schools).Select(name => name.Name).Any(name => Schools.Contains(name)));
                }
                return filterPredicate;
            }
        }

        public override Expression<Func<User, object>> DataSelector
        {
            get 
            {
                return u => new
                {
                    Status = u.Status,
                    LastName = u.LastName,
                    FirstName = u.FirstName,
                    LastLoginTime = u.LastLoginTime,
                    Email = u.EmailAddress,
                    Roles = CreateRoleList(u),
                    Associations = u.UserRoles.SelectMany(r => r.Schools).Distinct().Count() + u.UserRoles.SelectMany(r => r.Providers).Distinct().Count(),
                    Comments = u.Comments,
                    Id = u.Id
                };
            }
        }

        private IEnumerable<string> CreateRoleList(User user)
        {
            var roles = user.UserRoles.Select(r => r.Role.Name).ToList();
            if (EducationSecurityPrincipal.IsAdministrator(user, SecurityConfiguration))
            {
                if (roles.Any())
                {
                    roles.Insert(0, "Administrator");
                }
                else
                {
                    roles.Add("Administrator");
                }
            }
            return roles;
        }
    }
}
