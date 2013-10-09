using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        public ServiceRequestRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(ServiceRequest item)
        {
            Context.ServiceRequests.Add(item);
        }

        public void Remove(ServiceRequest item)
        {
            Context.ServiceRequests.Remove(item);
        }

        public void Update(ServiceRequest item)
        {
            Context.SetModified(item);
        }

        public IQueryable<ServiceRequest> Items
        {
            get { return Context.ServiceRequests; }
        }
    }
}
