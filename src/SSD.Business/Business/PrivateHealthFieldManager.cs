using SSD.Domain;
using SSD.IO;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SSD.Business
{
    public class PrivateHealthFieldManager : CustomFieldManager
    {
        private IProviderRepository ProviderRepository { get; set; }

        public PrivateHealthFieldManager(IRepositoryContainer repositories, IBlobClient blobClient, IDataTableBinder dataTableBinder, IUserAuditor auditor)
            : base(repositories, blobClient, dataTableBinder, auditor)
        {
            ProviderRepository = repositories.Obtain<IProviderRepository>();
        }

        public override DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<CustomField> dataTable)
        {
            IQueryable<CustomField> items = CustomFieldRepository.Items.OfType<PrivateHealthField>();
            return DataTableBinder.Bind<CustomField>(items, dataTable, requestModel);
        }

        public override void PopulateViewModel(CustomFieldModel viewModel)
        {
            base.PopulateViewModel(viewModel);
            PrivateHealthFieldModel validViewModel = viewModel as PrivateHealthFieldModel;
            if (validViewModel == null)
            {
                throw new ArgumentException(string.Format("Specified {0} must be of type {1}.", typeof(CustomFieldModel).Name, typeof(PrivateHealthFieldModel).Name), "viewModel");
            }
            validViewModel.Providers = new SelectList(ProviderRepository.Items, "Id", "Name");
        }

        protected override CustomFieldModel CreateFieldModel()
        {
            return new PrivateHealthFieldModel();
        }

        protected override CustomField CreateFieldEntity()
        {
            return new PrivateHealthField();
        }
    }
}
