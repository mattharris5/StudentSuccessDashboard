using SSD.Domain;
using System;
using System.Linq.Expressions;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    public class ServiceAttendanceClientDataTable : BaseClientDataTable<ServiceAttendance>
    {
        public ServiceAttendanceClientDataTable(HttpRequestBase request)
            : base(request)
        {
            Id = int.Parse(request["id"]);
        }

        public int Id { get; private set; }

        public override Expression<Func<ServiceAttendance, string>> SortSelector
        {
            get
            {
                if (SortColumnIndex == 0)
                {
                    return s => RepositoryFunctions.StringConvert(RepositoryFunctions.DateDiff("mi", DateTime.MinValue, s.DateAttended));
                }
                if (SortColumnIndex == 1)
                {
                    return s => s.Subject.Name;
                }
                return s => RepositoryFunctions.StringConvert((double)s.Duration);
            }
        }

        public override Expression<Func<ServiceAttendance, bool>> FilterPredicate
        {
            get
            {
                return s => s.StudentAssignedOfferingId == Id;
            }
        }

        public override Expression<Func<ServiceAttendance, object>> DataSelector
        {
            get 
            {
                return t => new
                {
                    DateAttended = t.DateAttended,
                    Subject = t.Subject.Name,
                    Duration = t.Duration,
                    Notes = t.Notes,
                    Id = t.Id
                };
            }
        }
    }
}
