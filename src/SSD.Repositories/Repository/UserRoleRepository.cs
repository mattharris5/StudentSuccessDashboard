using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class UserRoleRepository : IUserRoleRepository
    {
        public UserRoleRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(UserRole item)
        {
            Context.UserRoles.Add(item);
        }

        public void Remove(UserRole item)
        {
            Context.UserRoles.Remove(item);
        }

        public void Update(UserRole item)
        {
            Context.SetModified(item);
        }

        public IQueryable<UserRole> Items
        {
            get { return Context.UserRoles; }
        }

        public void AddLink(UserRole userRole, Provider provider)
        {
            if (userRole == null)
            {
                throw new ArgumentNullException("userRole");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            userRole.Providers.Add(provider);
            provider.UserRoles.Add(userRole);
        }

        public void AddLink(UserRole userRole, School school)
        {
            if (userRole == null)
            {
                throw new ArgumentNullException("userRole");
            }
            if (school == null)
            {
                throw new ArgumentNullException("school");
            }
            userRole.Schools.Add(school);
            school.UserRoles.Add(userRole);
        }

        public void DeleteLink(UserRole userRole, Provider provider)
        {
            if (userRole == null)
            {
                throw new ArgumentNullException("userRole");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            userRole.Providers.Remove(provider);
            provider.UserRoles.Remove(userRole);
        }

        public void DeleteLink(UserRole userRole, School school)
        {
            if (userRole == null)
            {
                throw new ArgumentNullException("userRole");
            }
            if (school == null)
            {
                throw new ArgumentNullException("school");
            }
            userRole.Schools.Remove(school);
            school.UserRoles.Remove(userRole);
        }
    }
}
