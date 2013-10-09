using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class ServiceTypeRepository : IServiceTypeRepository
    {
        public ServiceTypeRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(ServiceType item)
        {
            Context.ServiceTypes.Add(item);
        }

        public void Remove(ServiceType item)
        {
            Context.ServiceTypes.Remove(item);
        }

        public void Update(ServiceType item)
        {
            Context.SetModified(item);
        }

        public IQueryable<ServiceType> Items
        {
            get { return Context.ServiceTypes; }
        }

        public void AddLink(ServiceType serviceType, Category category)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }
            if (category == null)
            {
                throw new ArgumentNullException("category");
            }
            serviceType.Categories.Add(category);
            category.ServiceTypes.Add(serviceType);
        }

        public void DeleteLink(ServiceType serviceType, Category category)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }
            if (category == null)
            {
                throw new ArgumentNullException("category");
            }
            serviceType.Categories.Remove(category);
            category.ServiceTypes.Remove(serviceType);
        }
    }
}
