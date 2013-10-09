using SSD.Domain;
using SSD.Security;
using SSD.ViewModels;
using System.Collections.Generic;

namespace SSD.Business
{
    public interface IScheduledServiceManager
    {
        ScheduleServiceOfferingListOptionsModel GenerateScheduleOfferingViewModel(EducationSecurityPrincipal user, IEnumerable<int> students);
        ServiceOfferingScheduleModel GenerateCreateViewModel(int offeringId);
        StudentServiceOfferingScheduleModel GenerateEditViewModel(EducationSecurityPrincipal user, int scheduledOfferingId);
        DeleteServiceOfferingScheduleModel GenerateDeleteViewModel(EducationSecurityPrincipal user, int scheduledOfferingId);
        void Create(EducationSecurityPrincipal user, ServiceOfferingScheduleModel viewModel);
        void Edit(EducationSecurityPrincipal user, StudentServiceOfferingScheduleModel viewModel);
        void Delete(EducationSecurityPrincipal user, int scheduledOfferingId);
    }
}
