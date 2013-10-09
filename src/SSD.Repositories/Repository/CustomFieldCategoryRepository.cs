using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class CustomFieldCategoryRepository : ICustomFieldCategoryRepository
    {
        public CustomFieldCategoryRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(CustomFieldCategory item)
        {
            Context.CustomFieldCategories.Add(item);
        }

        public void Remove(CustomFieldCategory item)
        {
            Context.CustomFieldCategories.Remove(item);
        }

        public void Update(CustomFieldCategory item)
        {
            Context.SetModified(item);
        }

        public IQueryable<CustomFieldCategory> Items
        {
            get { return Context.CustomFieldCategories; }
        }
    }
}
