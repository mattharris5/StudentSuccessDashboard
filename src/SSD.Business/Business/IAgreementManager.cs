using SSD.Security;
using SSD.ViewModels;

namespace SSD.Business
{
    public interface IAgreementManager
    {
        EulaModel GenerateEulaAdminModel();
        EulaModel GeneratePromptViewModel();
        EulaModel GenerateEulaModelByUser(int userId);
        void Create(EulaModel viewModel, EducationSecurityPrincipal user);
        void Log(EulaModel viewModel, EducationSecurityPrincipal user);
    }
}
