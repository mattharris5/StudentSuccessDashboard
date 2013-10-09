using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class PriorityRepository : IPriorityRepository
    {
        public PriorityRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(Priority item)
        {
            throw new NotSupportedException("Add is not supported in this repository");
        }

        public void Remove(Priority item)
        {
            throw new NotSupportedException("Remove is not supported in this repository");
        }

        public void Update(Priority item)
        {
            throw new NotSupportedException("Update is not supported in this repository");
        }

        public IQueryable<Priority> Items
        {
            get { return Context.Priorities; }
        }
    }
}
