using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class SchoolRepository : ISchoolRepository
    {
        public SchoolRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(School item)
        {
            Context.Schools.Add(item);
        }

        public void Remove(School item)
        {
            Context.Schools.Remove(item);
        }

        public void Update(School item)
        {
            Context.SetModified(item);
        }

        public IQueryable<School> Items
        {
            get { return Context.Schools; }
        }
    }
}
