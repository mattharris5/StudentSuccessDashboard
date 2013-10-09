using System;
using System.Linq;

namespace SSD.ViewModels.DataTables
{
    public class DataTableBinder : IDataTableBinder
    {
        public DataTableResultModel Bind<TEntity>(IQueryable<TEntity> items, IClientDataTable<TEntity> dataTable, DataTableRequestModel requestModel)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            if (dataTable == null)
            {
                throw new ArgumentNullException("dataTable");
            }
            if (requestModel == null)
            {
                throw new ArgumentNullException("requestModel");
            }
            return BindCore(items, dataTable, requestModel);
        }

        private static DataTableResultModel BindCore<TEntity>(IQueryable<TEntity> items, IClientDataTable<TEntity> dataTable, DataTableRequestModel requestModel)
        {
            IQueryable<TEntity> query = dataTable.ApplyFilters(items);
            query = dataTable.ApplySort(query);
            var resultSet = dataTable.CreateResultSet(query, requestModel);
            return new DataTableResultModel
            {
                sEcho = requestModel.sEcho,
                iTotalRecords = items.Count(),
                iTotalDisplayRecords = query.Count(),
                aaData = resultSet
            };
        }
    }
}
