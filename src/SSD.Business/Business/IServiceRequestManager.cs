using SSD.Security;
using SSD.ViewModels;

namespace SSD.Business
{
    public interface IServiceRequestManager
    {
        void PopulateViewModel(ServiceRequestModel viewModel);
        ServiceRequestModel GenerateEditViewModel(EducationSecurityPrincipal user, int requestId);
        ServiceRequestModel GenerateCreateViewModel();
        ServiceRequestModel GenerateDeleteViewModel(EducationSecurityPrincipal user, int requestId);
        void Edit(EducationSecurityPrincipal user, ServiceRequestModel viewModel);
        void Create(EducationSecurityPrincipal user, ServiceRequestModel viewModel);
        void Delete(EducationSecurityPrincipal user, int requestId);
    }
}
