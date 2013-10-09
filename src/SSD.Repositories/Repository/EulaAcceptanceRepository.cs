using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class EulaAcceptanceRepository : IEulaAcceptanceRepository
    {
        private IEducationContext Context { get; set; }

        public EulaAcceptanceRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public void Add(EulaAcceptance item)
        {
            Context.EulaAcceptances.Add(item);
        }

        public void Remove(EulaAcceptance item)
        {
            throw new NotSupportedException("Remove is not supported in this repository");
        }

        public void Update(EulaAcceptance item)
        {
            throw new NotSupportedException("Update is not supported in this repository");
        }

        public IQueryable<EulaAcceptance> Items
        {
            get { return Context.EulaAcceptances; }
        }
    }
}
