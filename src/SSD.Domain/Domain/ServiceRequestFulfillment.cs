using System;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class ServiceRequestFulfillment : IAuditCreate
    {
        public ServiceRequestFulfillment()
        {
            CreateTime = DateTime.Now;
        }
        public int Id { get; internal set; }

        [Required]
        public int ServiceRequestId { get; set; }
        public ServiceRequest ServiceRequest { get; set; }

        [Required]
        public int FulfillmentStatusId { get; set; }
        public FulfillmentStatus FulfillmentStatus { get; set; }

        public int? FulfilledById { get; set; }
        public StudentAssignedOffering FulfilledBy { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        public DateTime CreateTime { get; internal set; }
        
        [Required]
        public int CreatingUserId { get; set; }
        public User CreatingUser { get; set; }
    }
}
