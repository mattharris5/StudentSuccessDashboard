using SSD.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace SSD.ViewModels
{
    public class ServiceRequestModel : IStateCopier<ServiceRequest>
    {
        private int _SelectedPriorityId;
        private int _SelectedSubjectId;
        private int _SelectedStatusId;

        public ServiceRequestModel()
        {
            SelectedPriorityId = 1;
            SelectedStatusId = 1;
            SelectedSubjectId = 1;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        
        public IEnumerable<int> StudentIds { get; set; }

        [Required(ErrorMessage = "Service Type is required")]
        public int SelectedServiceTypeId { get; set; }
        public SelectList ServiceTypes { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public int SelectedPriorityId
        {
            get { return _SelectedPriorityId; }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentException("Value cannot be 0");
                }
                _SelectedPriorityId = value;
            } 
        }
        public SelectList Priorities { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        public int SelectedSubjectId
        {
            get { return _SelectedSubjectId; }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentException("Value cannot be 0");
                }
                _SelectedSubjectId = value;
            }
        }
        public SelectList Subjects { get; set; }

        public int? SelectedAssignedOfferingId { get; set; }
        public SelectList AssignedOfferings { get; set; }

        public int SelectedStatusId
        {
            get { return _SelectedStatusId; }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentException("Value cannot be 0");
                }
                _SelectedStatusId = value;
            }
        }
        public SelectList Statuses { get; set; }

        public int OriginalStatusId { get; set; }

        public string Notes { get; set; }

        public string FulfillmentNotes { get; set; }

        public AuditModel Audit { get; set; }

        public void CopyTo(ServiceRequest model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            model.PriorityId = SelectedPriorityId;
            model.SubjectId = SelectedSubjectId;
            model.ServiceTypeId = SelectedServiceTypeId;
            model.Notes = Notes;
        }

        public void CopyFrom(ServiceRequest model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            Id = model.Id;
            StudentIds = new List<int>() { model.StudentId };
            SelectedPriorityId = model.PriorityId;
            SelectedServiceTypeId = model.ServiceTypeId;
            SelectedSubjectId = model.SubjectId;
            Notes = model.Notes;
            var latestDetail = model.FulfillmentDetails.OrderByDescending(f => f.CreateTime).First();
            SelectedAssignedOfferingId = latestDetail.FulfilledById;
            SelectedStatusId = latestDetail.FulfillmentStatusId;
            OriginalStatusId = SelectedStatusId;
            FulfillmentNotes = latestDetail.Notes;
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
