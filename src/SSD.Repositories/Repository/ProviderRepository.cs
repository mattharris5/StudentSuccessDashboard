using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class ProviderRepository : IProviderRepository
    {
        public ProviderRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(Provider item)
        {
            Context.Providers.Add(item);
        }

        public void Remove(Provider item)
        {
            Context.Providers.Remove(item);
        }

        public void Update(Provider item)
        {
            Context.SetModified(item);
        }

        public IQueryable<Provider> Items
        {
            get { return Context.Providers; }
        }
    }
}
