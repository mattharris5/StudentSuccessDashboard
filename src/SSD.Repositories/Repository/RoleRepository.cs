using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class RoleRepository : IRoleRepository
    {
        public RoleRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(Role item)
        {
            throw new NotSupportedException("Add is not supported in this repository");
        }

        public void Remove(Role item)
        {
            throw new NotSupportedException("Remove is not supported in this repository");
        }

        public void Update(Role item)
        {
            throw new NotSupportedException("Update is not supported in this repository");
        }

        public IQueryable<Role> Items
        {
            get { return Context.Roles; }
        }
    }
}
