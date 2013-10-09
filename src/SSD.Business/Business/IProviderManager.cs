using SSD.Domain;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.Collections.Generic;

namespace SSD.Business
{
    public interface IProviderManager
    {
        IEnumerable<string> SearchProviderNames(string term);
        void Delete(int id);
        void Create(EducationSecurityPrincipal user, ProviderModel viewModel);
        void Edit(EducationSecurityPrincipal user, ProviderModel viewModel);
        ProviderModel GenerateEditViewModel(EducationSecurityPrincipal user, int providerId);
        DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<Provider> dataTable);
        ProviderModel GenerateDeleteViewModel(int id);
        void PopulateViewModel(ProviderModel viewModel);
        void Validate(ProviderModel viewModel);
    }
}
