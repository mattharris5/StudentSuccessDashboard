using Microsoft.Linq.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    public abstract class BaseClientDataTable<T> : IClientDataTable<T>
    {
        protected BaseClientDataTable(HttpRequestBase request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            Request = request;
            InitializeSortSettings();
        }

        protected HttpRequestBase Request { get; private set; }

        public bool HasSort { get; private set; }

        public int? SortColumnIndex { get; private set; }

        public bool SortAscending { get; private set; }

        public abstract Expression<Func<T, string>> SortSelector { get; }

        public abstract Expression<Func<T, object>> DataSelector { get; }

        public abstract Expression<Func<T, bool>> FilterPredicate { get; }

        public virtual IQueryable<T> ApplyFilters(IQueryable<T> items)
        {
            return items.Where(FilterPredicate).WithTranslations();
        }

        public virtual IQueryable<T> ApplySort(IQueryable<T> items)
        {
            if (HasSort)
            {
                return SortAscending ? items.OrderBy(SortSelector).WithTranslations() :
                                       items.OrderByDescending(SortSelector).WithTranslations();
            }
            return items.OrderBy(item => 1).WithTranslations(); // OrderBy on something is required in order to support paging
        }

        public virtual IList<object> CreateResultSet(IQueryable<T> items, DataTableRequestModel requestModel)
        {
            if (requestModel == null)
            {
                throw new ArgumentNullException("requestModel");
            }
            if (GetType() == typeof(StudentClientDataTable) || GetType() == typeof(UserClientDataTable))
            {
                var displayedItems = GetPagedData(items, requestModel).WithTranslations();
                var materializedResults = displayedItems.ToList().AsQueryable().Select(DataSelector);
                return materializedResults.ToList();
            }
            var selectedItems = GetPagedData(items, requestModel).Select(DataSelector).WithTranslations();
            var results = selectedItems.ToList();
            return results;
        }

        protected IQueryable<T> GetPagedData(IQueryable<T> items, DataTableRequestModel requestModel)
        {
            if (requestModel.iDisplayLength != -1)
            {
                return items.Skip(requestModel.iDisplayStart).Take(requestModel.iDisplayLength);
            }
            else
            {
                return items;
            }
        }

        protected IEnumerable<string> ExtractFilterList(string filterKey)
        {
            if (Request[filterKey] != null)
            {
                string filterValues = Request[filterKey];
                if (filterValues.Length > 0)
                {
                    string[] splitValues = filterValues.Split('|');
                    return splitValues.Where(s => !string.IsNullOrWhiteSpace(s));
                }
            }
            return Enumerable.Empty<string>();
        }

        protected string ExtractFilterValue(string filterKey)
        {
            return string.IsNullOrWhiteSpace(Request[filterKey]) ? null : Request[filterKey];
        }

        private void InitializeSortSettings()
        {
            string sortColumn = ExtractFilterValue("iSortCol_0");
            string sortDirection = ExtractFilterValue("sSortDir_0");
            int sortColumnIndex;
            if (int.TryParse(sortColumn, out sortColumnIndex))
            {
                SortColumnIndex = sortColumnIndex;
            }
            else
            {
                SortColumnIndex = null;
            }
            HasSort = SortColumnIndex.HasValue && !string.IsNullOrWhiteSpace(sortDirection);
            SortAscending = sortDirection == "asc";
        }
    }
}
