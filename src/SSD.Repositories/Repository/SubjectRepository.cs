using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class SubjectRepository : ISubjectRepository
    {
        public SubjectRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(Subject item)
        {
            throw new NotSupportedException("Add is not supported in this repository");
        }

        public void Remove(Subject item)
        {
            throw new NotSupportedException("Remove is not supported in this repository");
        }

        public void Update(Subject item)
        {
            throw new NotSupportedException("Update is not supported in this repository");
        }

        public IQueryable<Subject> Items
        {
            get { return Context.Subjects; }
        }
    }
}
