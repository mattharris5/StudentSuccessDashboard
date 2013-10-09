using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class ServiceRequestFulfillmentRepository : IServiceRequestFulfillmentRepository
    {
        private IEducationContext Context { get; set; }

        public ServiceRequestFulfillmentRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public void Add(ServiceRequestFulfillment item)
        {
            Context.ServiceRequestFulfillments.Add(item);
        }

        public void Remove(ServiceRequestFulfillment item)
        {
            Context.ServiceRequestFulfillments.Remove(item);
        }

        public void Update(ServiceRequestFulfillment item)
        {
            Context.SetModified(item);
        }

        public IQueryable<ServiceRequestFulfillment> Items
        {
            get { return Context.ServiceRequestFulfillments; }
        }
    }
}
