using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class CustomFieldTypeRepository : ICustomFieldTypeRepository
    {
        public CustomFieldTypeRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(CustomFieldType item)
        {
            Context.CustomFieldTypes.Add(item);
        }

        public void Remove(CustomFieldType item)
        {
            Context.CustomFieldTypes.Remove(item);
        }

        public void Update(CustomFieldType item)
        {
            Context.SetModified(item);
        }

        public IQueryable<CustomFieldType> Items
        {
            get { return Context.CustomFieldTypes; }
        }
    }
}
