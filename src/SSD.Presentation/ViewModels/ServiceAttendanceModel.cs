using SSD.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SSD.ViewModels
{
    public class ServiceAttendanceModel : IStateCopier<ServiceAttendance>
    {
        public int Id { get; set; }

        [Required]
        public int StudentAssignedOfferingId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime DateAttended { get; set; }

        public int SelectedSubjectId { get; set; }
        public SelectList Subjects { get; set; }

        public Decimal Duration { get; set; }

        public string Notes { get; set; }

        public List<int> Durations
        {
            get
            {
                return new List<int> { 30, 45, 60, 90, 120 };
            }
        }

        public AuditModel Audit { get; set; }

        public void CopyTo(ServiceAttendance model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            model.StudentAssignedOfferingId = StudentAssignedOfferingId;
            model.DateAttended = DateAttended;
            model.SubjectId = SelectedSubjectId;
            model.Duration = Duration;
            model.Notes = Notes;
        }

        public void CopyFrom(ServiceAttendance model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            Id = model.Id;
            StudentAssignedOfferingId = model.StudentAssignedOfferingId;
            DateAttended = model.DateAttended;
            SelectedSubjectId = model.SubjectId;
            Duration = model.Duration;
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
