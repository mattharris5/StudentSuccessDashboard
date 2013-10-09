using SSD.Domain;
using System;
using System.Linq.Expressions;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    public class AuditAccessClientDataTable : BaseClientDataTable<UserAccessChangeEvent>
    {
        public AuditAccessClientDataTable(HttpRequestBase request)
            : base(request)
        {
            Id = int.Parse(request["id"]);
        }

        public int Id { get; private set; }

        public override Expression<Func<UserAccessChangeEvent, string>> SortSelector
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

        public override Expression<Func<UserAccessChangeEvent, bool>> FilterPredicate
        {
            get 
            {
                return s => s.UserId == Id;
            }
        }

        public override Expression<Func<UserAccessChangeEvent, object>> DataSelector
        {
            get 
            {
                return t => new
                {
                    CreatingUser = t.CreatingUser.DisplayName,
                    CreateTime = t.CreateTime,
                    UserActive = t.UserActive,
                    AccessData = t.AccessData,
                    Id = t.Id
                };
            }
        }
    }
}
