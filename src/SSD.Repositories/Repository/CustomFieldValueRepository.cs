using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class CustomFieldValueRepository : ICustomFieldValueRepository
    {
        public CustomFieldValueRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(CustomFieldValue item)
        {
            Context.CustomFieldValues.Add(item);
        }

        public void Remove(CustomFieldValue item)
        {
            Context.CustomFieldValues.Remove(item);
        }

        public void Update(CustomFieldValue item)
        {
            Context.SetModified(item);
        }

        public IQueryable<CustomFieldValue> Items
        {
            get { return Context.CustomFieldValues; }
        }
    }
}
