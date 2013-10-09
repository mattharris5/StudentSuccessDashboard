using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class FulfillmentStatusRepository : IFulfillmentStatusRepository
    {
        private IEducationContext Context { get; set; }

        public FulfillmentStatusRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public void Add(FulfillmentStatus item)
        {
            throw new NotSupportedException("Add is not supported in this repository");
        }

        public void Remove(FulfillmentStatus item)
        {
            throw new NotSupportedException("Remove is not supported in this repository");
        }

        public void Update(FulfillmentStatus item)
        {
            throw new NotSupportedException("Update is not supported in this repository");
        }

        public IQueryable<FulfillmentStatus> Items
        {
            get { return Context.FulfillmentStatuses; }
        }
    }
}
