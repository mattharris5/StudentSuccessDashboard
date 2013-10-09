using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class CustomDataOriginRepository : ICustomDataOriginRepository
    {
        public CustomDataOriginRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(CustomDataOrigin item)
        {
            Context.CustomDataOrigins.Add(item);
        }

        public void Remove(CustomDataOrigin item)
        {
            Context.CustomDataOrigins.Remove(item);
        }

        public void Update(CustomDataOrigin item)
        {
            Context.SetModified(item);
        }

        public IQueryable<CustomDataOrigin> Items
        {
            get { return Context.CustomDataOrigins; }
        }
    }
}
