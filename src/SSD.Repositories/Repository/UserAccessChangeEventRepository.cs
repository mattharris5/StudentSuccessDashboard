using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class UserAccessChangeEventRepository : IUserAccessChangeEventRepository
    {
        private IEducationContext Context { get; set; }

        public UserAccessChangeEventRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public void Add(UserAccessChangeEvent item)
        {
            Context.UserAccessChangeEvents.Add(item);
        }

        public void Remove(UserAccessChangeEvent item)
        {
            ThrowNotSupported();
        }

        public void Update(UserAccessChangeEvent item)
        {
            ThrowNotSupported();
        }

        public IQueryable<UserAccessChangeEvent> Items
        {
            get { return Context.UserAccessChangeEvents; }
        }

        private static void ThrowNotSupported()
        {
            throw new NotSupportedException(string.Format("{0} items are immutable and cannot be removed.", typeof(UserAccessChangeEvent).Name));
        }
    }
}
