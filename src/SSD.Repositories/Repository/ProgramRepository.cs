using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class ProgramRepository : IProgramRepository
    {
        private IEducationContext Context { get; set; }

        public ProgramRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public void Add(Program item)
        {
            Context.Programs.Add(item);
        }

        public void Remove(Program item)
        {
            Context.Programs.Remove(item);
        }

        public void Update(Program item)
        {
            Context.SetModified(item);
        }

        public IQueryable<Program> Items
        {
            get { return Context.Programs; }
        }
    }
}
