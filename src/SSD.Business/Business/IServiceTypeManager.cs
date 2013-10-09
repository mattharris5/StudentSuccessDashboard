using SSD.Domain;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.Collections.Generic;

namespace SSD.Business
{
    public interface IServiceTypeManager
    {
        void SetPrivacy(EducationSecurityPrincipal user, int typeId, bool isPrivate);
        void PopulateViewModel(EducationSecurityPrincipal user, ServiceTypeModel viewModel);
        ServiceTypeListOptionsModel GenerateListOptionsViewModel(EducationSecurityPrincipal user);
        DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<ServiceType> dataTable);
        ServiceTypeModel GenerateCreateViewModel(EducationSecurityPrincipal user);
        ServiceTypeModel GenerateEditViewModel(EducationSecurityPrincipal user, int id);
        ServiceTypeModel GenerateDeleteViewModel(int id);
        IEnumerable<string> SearchNames(string term);
        void Create(ServiceTypeModel viewModel);
        void Edit(ServiceTypeModel viewModel);
        void Delete(int typeId);
        void ValidateForDuplicate(ServiceTypeModel viewModel);
        ServiceTypeSelectorModel GenerateSelectorViewModel();
    }
}
