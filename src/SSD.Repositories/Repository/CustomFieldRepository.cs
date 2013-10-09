using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class CustomFieldRepository : ICustomFieldRepository
    {
        public CustomFieldRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(CustomField item)
        {
            Context.CustomFields.Add(item);
        }

        public void Remove(CustomField item)
        {
            Context.CustomFields.Remove(item);
        }

        public void Update(CustomField item)
        {
            Context.SetModified(item);
        }

        public IQueryable<CustomField> Items
        {
            get { return Context.CustomFields; }
        }
    }
}
