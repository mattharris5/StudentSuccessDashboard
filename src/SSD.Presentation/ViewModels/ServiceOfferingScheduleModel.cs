using SSD.Domain;
using System;
using System.Collections.Generic;

namespace SSD.ViewModels
{
    public class ServiceOfferingScheduleModel : IStateCopier<StudentAssignedOffering>
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public int ServiceOfferingId { get; set; }
        
        public IEnumerable<int> SelectedStudents { get; set; }
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public string Notes { get; set; }

        public void CopyTo(StudentAssignedOffering model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            model.EndDate = EndDate;
            model.Notes = Notes;
            model.ServiceOfferingId = ServiceOfferingId;
            model.StartDate = StartDate;
        }

        public void CopyFrom(StudentAssignedOffering model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            Id = model.Id;
            EndDate = model.EndDate;
            Notes = model.Notes;
            ServiceOfferingId = model.ServiceOfferingId;
            StartDate = model.StartDate;
            Name = (model.ServiceOffering != null) ? model.ServiceOffering.Name : string.Empty;
        }
    }
}
