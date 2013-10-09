using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    public class StudentApprovalClientDataTable : BaseClientDataTable<Student>
    {
        public StudentApprovalClientDataTable(HttpRequestBase request)
            : base(request)
        {
            InitializeRequestValues();
        }

        public string Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public IEnumerable<string> Schools { get; private set; }
        public IEnumerable<string> Providers { get; private set; }

        public override Expression<Func<Student, string>> SortSelector
        {
            get
            {
                if (SortColumnIndex == 1)
                {
                    return s => s.FullName;
                }
                if (SortColumnIndex == 2)
                {
                    return s => RepositoryFunctions.StringConvert((double)s.Grade);
                }
                if (SortColumnIndex == 3)
                {
                    return s => s.School.Name;
                }
                return s => s.StudentSISId;
            }
        }

        public override Expression<Func<Student, object>> DataSelector
        {
            get
            {
                return s => new
                {
                    StudentSISId = s.StudentSISId,
                    Name = s.FullName,
                    Grade = s.Grade,
                    School = s.School.Name,
                    ApprovedProviders = s.ApprovedProviders.Where(p => p.IsActive).Select(p => new { Id = p.Id, Name = p.Name }),
                    Id = s.Id,
                    HasParentalOptOut = s.HasParentalOptOut
                };
            }
        }

        public override Expression<Func<Student, bool>> FilterPredicate
        {
            get
            {
                Expression<Func<Student, bool>> filterPredicate = s => true;
                if (Id != null)
                {
                    filterPredicate = filterPredicate.AndAlso(s => s.StudentSISId.ToLower().Contains(Id.ToLower()));
                }
                if (FirstName != null)
                {
                    filterPredicate = filterPredicate.AndAlso(s => s.FirstName.ToLower().Contains(FirstName.ToLower()));
                }
                if (LastName != null)
                {
                    filterPredicate = filterPredicate.AndAlso(s => s.LastName.ToLower().Contains(LastName.ToLower()));
                }
                if (Schools.Any())
                {
                    filterPredicate = filterPredicate.AndAlso(s => Schools.Contains(s.School.Name));
                }
                if (Providers.Any())
                {
                    filterPredicate = filterPredicate.AndAlso(s => Providers.Any(f => s.ApprovedProviders.Select(p => p.Name).Contains(f)));
                }
                return filterPredicate;
            }
        }

        private void InitializeRequestValues()
        {
            Id = ExtractFilterValue("ID");
            FirstName = ExtractFilterValue("firstName");
            LastName = ExtractFilterValue("lastName");
            Schools = ExtractFilterList("schools");
            Providers = ExtractFilterList("providers");
        }
    }
}
