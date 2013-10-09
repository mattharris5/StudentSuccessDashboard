using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class PrivateHealthDataViewEventRepository : IPrivateHealthDataViewEventRepository
    {
        private IEducationContext Context { get; set; }

        public PrivateHealthDataViewEventRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public void Add(PrivateHealthDataViewEvent item)
        {
            Context.PrivateHealthDataViewEvents.Add(item);
        }

        public void Remove(PrivateHealthDataViewEvent item)
        {
            throw new NotSupportedException("Remove is not supported in this repository");
        }

        public void Update(PrivateHealthDataViewEvent item)
        {
            throw new NotSupportedException("Update is not supported in this repository");
        }

        public IQueryable<PrivateHealthDataViewEvent> Items
        {
            get { return Context.PrivateHealthDataViewEvents; }
        }
    }
}
