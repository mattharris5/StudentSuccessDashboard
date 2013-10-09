using SSD.Domain;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    public class PublicFieldClientDataTable : BaseClientDataTable<CustomField>
    {
        public PublicFieldClientDataTable(HttpRequestBase request)
            : base(request)
        { }

        public override Expression<Func<CustomField, string>> SortSelector
        {
            get
            {
                if (SortColumnIndex == 1)
                {
                    return c => c.CustomFieldType.Name;
                }
                return c => c.Name;
            }
        }

        public override Expression<Func<CustomField, object>> DataSelector
        {
            get
            {
                return c => new
                {
                    Name = c.Name,
                    Type = c.CustomFieldType.Name,
                    Categories = c.Categories.Select(s => s.Name),
                    Id = c.Id
                };
            }
        }

        public override Expression<Func<CustomField, bool>> FilterPredicate
        {
            get
            {
                return c => c is PublicField;
            }
        }
    }
}
