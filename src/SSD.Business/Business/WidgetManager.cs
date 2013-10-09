using SSD.Repository;
using SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.Business
{
    public class WidgetManager : IWidgetManager
    {
        public WidgetManager(IRepositoryContainer repositories)
        {
            if (repositories == null)
            {
                throw new ArgumentNullException("repositories");
            }
            SchoolRepository = repositories.Obtain<ISchoolRepository>();
            ServiceTypeRepository = repositories.Obtain<IServiceTypeRepository>();
            StudentRepository = repositories.Obtain<IStudentRepository>();
            ServiceRequestRepository = repositories.Obtain<IServiceRequestRepository>();
        }

        private ISchoolRepository SchoolRepository { get; set; }
        private IServiceTypeRepository ServiceTypeRepository { get; set; }
        private IStudentRepository StudentRepository { get; set; }
        private IServiceRequestRepository ServiceRequestRepository { get; set; }

        public IEnumerable<ServiceRequestsBySchoolModel> GenerateServiceRequestsBySchoolModel()
        {
            var serviceRequests = ServiceRequestRepository.Items;
            var viewModels = SchoolRepository.Items.Select(t => new ServiceRequestsBySchoolModel
            {
                SchoolName = t.Name,
                Total = serviceRequests.Where(s => s.Student.SchoolId == t.Id && s.FulfillmentDetails.OrderByDescending(f => f.CreateTime).FirstOrDefault().FulfillmentStatus.Name != Statuses.Rejected).Count(),
                Open = serviceRequests.Where(s => s.Student.SchoolId == t.Id && s.FulfillmentDetails.OrderByDescending(f => f.CreateTime).FirstOrDefault().FulfillmentStatus.Name == Statuses.Open).Count(),
                Fulfilled = serviceRequests.Where(s => s.Student.SchoolId == t.Id && s.FulfillmentDetails.OrderByDescending(f => f.CreateTime).FirstOrDefault().FulfillmentStatus.Name == Statuses.Fulfilled).Count()
            }).OrderBy(s => s.SchoolName);
            return viewModels;
        }

        public IEnumerable<ServiceTypeMetricModel> GenerateServiceTypeMetricModels()
        {
            double totalStudents = StudentRepository.Items.Count();
            var viewModels = ServiceTypeRepository.Items.Where(s => s.IsActive).Select(t => new ServiceTypeMetricModel
            {
                ServiceTypeName = t.Name,
                ProviderCount = t.ServiceOfferings.SelectMany(o => o.StudentAssignedOfferings).Where(a => a.IsActive).Select(a => a.ServiceOffering.Provider).Distinct().Count(),
                PercentOfStudentsBeingServed = (int)((t.ServiceOfferings.SelectMany(s => s.StudentAssignedOfferings).Where(s => s.IsActive).Select(s => s.Student).Distinct().Count() / totalStudents) * 100.0)
            }).OrderBy(v => v.ServiceTypeName);
            return viewModels;
        }
    }
}
