using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class ServiceTypeCategoryRepository : IServiceTypeCategoryRepository
    {
        public ServiceTypeCategoryRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(Category item)
        {
            Context.Categories.Add(item);
        }

        public void Remove(Category item)
        {
            Context.Categories.Remove(item);
        }

        public void Update(Category item)
        {
            Context.SetModified(item);
        }

        public IQueryable<Category> Items
        {
            get { return Context.Categories; }
        }
    }
}
