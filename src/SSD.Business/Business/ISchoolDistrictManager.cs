using SSD.Domain;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.Collections.Generic;

namespace SSD.Business
{
    public interface ISchoolDistrictManager
    {
        StudentListOptionsModel GenerateListOptionsViewModel(EducationSecurityPrincipal user);
        StudentApprovalListOptionsModel GenerateApprovalListOptionsViewModel();
        AddStudentApprovalModel GenerateAddStudentApprovalViewModel(int id);
        IEnumerable<int> GetFilteredFinderStudentIds(EducationSecurityPrincipal user, IClientDataTable<Student> dataTable);
        IEnumerable<string> SearchFirstNames(EducationSecurityPrincipal user, string term);
        IEnumerable<string> SearchLastNames(EducationSecurityPrincipal user, string term);
        IEnumerable<string> SearchIdentifiers(EducationSecurityPrincipal user, string term);
        StudentDetailModel GenerateStudentDetailViewModel(EducationSecurityPrincipal user, int id);
        void AddProviders(AddStudentApprovalModel viewModel);
        RemoveApprovedProviderModel GenerateRemoveProviderViewModel(int id, int providerId);
        void RemoveProvider(RemoveApprovedProviderModel viewModel);
        RemoveApprovedProvidersBySchoolModel GenerateRemoveProvidersBySchoolViewModel();
        void RemoveAllProviders();
        void RemoveAllProviders(IEnumerable<int> schoolIds);
        void PopulateViewModelLists(AddStudentApprovalModel viewModel);
        IEnumerable<Property> FindStudentProperties();
        DataTableResultModel GenerateApprovalDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<Student> dataTable);
        DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<Student> dataTable);
        void SetStudentOptOutState(int id, bool hasParentalOptOut);
        int CountStudentsWithApprovedProviders();
        SchoolSelectorModel GenerateSchoolSelectorViewModel();
        GradeSelectorModel GenerateGradeSelectorViewModel();
    }
}
    