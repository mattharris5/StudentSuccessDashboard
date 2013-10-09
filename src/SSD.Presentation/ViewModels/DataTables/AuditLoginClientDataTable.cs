using SSD.Domain;
using System;
using System.Linq.Expressions;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    public class AuditLoginClientDataTable : BaseClientDataTable<LoginEvent>
    {
        public AuditLoginClientDataTable(HttpRequestBase request)
            : base(request)
        {
            Id = int.Parse(request["id"]);
        }

        public int Id { get; private set; }

        public override Expression<Func<LoginEvent, string>> SortSelector
        {
            get 
            {
                if (SortColumnIndex == 0)
                {
                    return s => s.CreatingUser.DisplayName;
                }
                return s => RepositoryFunctions.StringConvert(RepositoryFunctions.DateDiff("mi", DateTime.MinValue, s.CreateTime));
            }
        }

        public override Expression<Func<LoginEvent, bool>> FilterPredicate
        {
            get
            {
                return s => s.CreatingUserId == Id;
            }
        }

        public override Expression<Func<LoginEvent, object>> DataSelector
        {
            get
            {
                return t => new
                {
                    CreatingUser = t.CreatingUser.DisplayName,
                    CreateTime = t.CreateTime,
                    Id = t.Id
                };
            }
        }
    }
}
