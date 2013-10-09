using SSD.Domain;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;

namespace SSD.Business
{
    public interface IServiceAttendanceManager
    {
        DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<ServiceAttendance> dataTable);
        ServiceAttendanceModel GenerateCreateViewModel(EducationSecurityPrincipal user, int id);
        ServiceAttendanceModel GenerateEditViewModel(EducationSecurityPrincipal user, int id);
        ServiceAttendanceModel GenerateDeleteViewModel(EducationSecurityPrincipal user, int id);
        void Create(ServiceAttendanceModel viewModel, EducationSecurityPrincipal user);
        void Edit(ServiceAttendanceModel viewModel, EducationSecurityPrincipal user);
        void Delete(int id, EducationSecurityPrincipal user);
    }
}
