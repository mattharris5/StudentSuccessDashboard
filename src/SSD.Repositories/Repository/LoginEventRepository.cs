using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class LoginEventRepository : ILoginEventRepository
    {
        public LoginEventRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(LoginEvent item)
        {
            Context.LoginEvents.Add(item);
        }

        public void Remove(LoginEvent item)
        {
            ThrowNotSupported();
        }

        public void Update(LoginEvent item)
        {
            ThrowNotSupported();
        }

        public IQueryable<LoginEvent> Items
        {
            get { return Context.LoginEvents; }
        }

        private static void ThrowNotSupported()
        {
            throw new NotSupportedException(string.Format("{0} items are immutable and cannot be removed.", typeof(LoginEvent).Name));
        }
    }
}
