using System.Linq;

namespace SSD.ViewModels.DataTables
{
    public interface IDataTableBinder
    {
        DataTableResultModel Bind<TEntity>(IQueryable<TEntity> items, IClientDataTable<TEntity> dataTable, DataTableRequestModel requestModel);
    }
}
