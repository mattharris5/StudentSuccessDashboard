using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class StudentAssignedOfferingRepository : IStudentAssignedOfferingRepository
    {
        public StudentAssignedOfferingRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(StudentAssignedOffering item)
        {
            Context.StudentAssignedOfferings.Add(item);
        }

        public void Remove(StudentAssignedOffering item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            throw new NotSupportedException(string.Format("{0} entities cannot be removed.  Only \"soft\" deletes are supported via IsActive.", item.GetType().Name));
        }

        public void Update(StudentAssignedOffering item)
        {
            Context.SetModified(item);
        }

        public IQueryable<StudentAssignedOffering> Items
        {
            get { return Context.StudentAssignedOfferings; }
        }
    }
}
