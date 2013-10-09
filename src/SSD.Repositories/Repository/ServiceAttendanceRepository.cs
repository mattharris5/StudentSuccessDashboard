using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class ServiceAttendanceRepository : IServiceAttendanceRepository
    {
        public ServiceAttendanceRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(ServiceAttendance item)
        {
            Context.ServiceAttendances.Add(item);
        }

        public void Remove(ServiceAttendance item)
        {
            Context.ServiceAttendances.Remove(item);
        }

        public void Update(ServiceAttendance item)
        {
            Context.SetModified(item);
        }

        public IQueryable<ServiceAttendance> Items
        {
            get { return Context.ServiceAttendances; }
        }
    }
}
