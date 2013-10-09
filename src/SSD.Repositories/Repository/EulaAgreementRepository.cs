using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class EulaAgreementRepository : IEulaAgreementRepository
    {
        private IEducationContext Context { get; set; }

        public EulaAgreementRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public void Add(EulaAgreement item)
        {
            Context.EulaAgreements.Add(item);
        }

        public void Remove(EulaAgreement item)
        {
            throw new NotSupportedException("Remove is not supported in this repository");
        }

        public void Update(EulaAgreement item)
        {
            throw new NotSupportedException("Update is not supported in this repository");
        }

        public IQueryable<EulaAgreement> Items
        {
            get { return Context.EulaAgreements; }
        }
    }
}
