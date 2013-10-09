using SSD.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class ServiceRequest : IAuditCreate, IAuditModify
    {
        public ServiceRequest()
        {
            CreateTime = DateTime.Now;
        }

        public int Id { get; internal set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        [Required]
        public int ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [Required]
        public int PriorityId { get; set; }
        public Priority Priority { get; set; }

        [Required]
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public DateTime CreateTime { get; internal set; }

        [Required]
        public int CreatingUserId { get; set; }
        public User CreatingUser { get; set; }

        public DateTime? LastModifyTime { get; set; }

        public int? LastModifyingUserId { get; set; }
        public User LastModifyingUser { get; set; }

        [RequiredElements]
        public ICollection<ServiceRequestFulfillment> FulfillmentDetails { get; set; }
    }
}
