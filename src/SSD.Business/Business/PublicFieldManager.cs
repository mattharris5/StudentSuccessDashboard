using SSD.Domain;
using SSD.IO;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.Linq;

namespace SSD.Business
{
    public class PublicFieldManager : CustomFieldManager
    {
        public PublicFieldManager(IRepositoryContainer repositories, IBlobClient blobClient, IDataTableBinder dataTableBinder, IUserAuditor auditor)
            : base(repositories, blobClient, dataTableBinder, auditor)
        { }

        public override DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<CustomField> dataTable)
        {
            IQueryable<CustomField> items = CustomFieldRepository.Items.OfType<PublicField>();
            return DataTableBinder.Bind<CustomField>(items, dataTable, requestModel);
        }

        protected override CustomFieldModel CreateFieldModel()
        {
            return new PublicFieldModel();
        }

        protected override CustomField CreateFieldEntity()
        {
            return new PublicField();
        }
    }
}
