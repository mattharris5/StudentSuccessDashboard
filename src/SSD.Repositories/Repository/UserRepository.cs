using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(User item)
        {
            Context.Users.Add(item);
        }

        public void Remove(User item)
        {
            throw new NotImplementedException();
        }

        public void Update(User item)
        {
            Context.SetModified(item);
        }

        public IQueryable<User> Items
        {
            get { return Context.Users; }
        }
    }
}
