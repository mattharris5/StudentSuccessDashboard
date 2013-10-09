using SSD.ViewModels;
using System.Collections.Generic;

namespace SSD.Business
{
    public interface IWidgetManager
    {
        IEnumerable<ServiceRequestsBySchoolModel> GenerateServiceRequestsBySchoolModel();
        IEnumerable<ServiceTypeMetricModel> GenerateServiceTypeMetricModels();
    }
}
