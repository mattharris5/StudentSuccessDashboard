using SSD.Domain;
using System;

namespace SSD.ViewModels
{
    public class StudentServiceOfferingScheduleModel : IStateCopier<StudentAssignedOffering>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Notes { get; set; }
        public AuditModel Audit { get; set; }

        public void CopyTo(StudentAssignedOffering model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            model.StartDate = StartDate;
            model.EndDate = EndDate;
            model.Notes = Notes;
        }

        public void CopyFrom(StudentAssignedOffering model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            Id = model.Id;
            Name = model.ServiceOffering.Name;
            StartDate = model.StartDate;
            EndDate = model.EndDate;
            Notes = model.Notes;
            if (Audit == null)
            {
                Audit = new AuditModel();
            }
            Audit.CreatedBy = model.CreatingUser.DisplayName;
            Audit.CreateTime = model.CreateTime;
            if (model.LastModifyingUser != null)
            {
                Audit.LastModifiedBy = model.LastModifyingUser.DisplayName;
                Audit.LastModifyTime = model.LastModifyTime;
            }
            else
            {
                Audit.LastModifiedBy = null;
                Audit.LastModifyTime = null;
            }
        }
    }
}
