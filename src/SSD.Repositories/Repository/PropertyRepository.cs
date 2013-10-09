using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class PropertyRepository : IPropertyRepository
    {
        public PropertyRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(Property item)
        {
            Context.Properties.Add(item);
        }

        public void Remove(Property item)
        {
            Context.Properties.Remove(item);
        }

        public void Update(Property item)
        {
            Context.SetModified(item);
        }

        public IQueryable<Property> Items
        {
            get { return Context.Properties; }
        }
    }
}
