using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SSD.ViewModels.DataTables
{
    public interface IClientDataTable<T>
    {
        Expression<Func<T, object>> DataSelector { get; }
        Expression<Func<T, bool>> FilterPredicate { get; }
        Expression<Func<T, string>> SortSelector { get; }
        bool HasSort { get; }
        bool SortAscending { get; }
        int? SortColumnIndex { get; }
        IQueryable<T> ApplyFilters(IQueryable<T> items);
        IQueryable<T> ApplySort(IQueryable<T> items);
        IList<object> CreateResultSet(IQueryable<T> items, DataTableRequestModel requestModel);
    }
}
