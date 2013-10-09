using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class StudentAssignedOffering : IAuditCreate, IAuditModify
    {
        public StudentAssignedOffering()
        {
            Attendances = new List<ServiceAttendance>();
            CreateTime = DateTime.Now;
            Fulfillments = new List<ServiceRequestFulfillment>();
        }

        public int Id { get; internal set; }

        public bool IsActive { get; set; }

        [Required]
        public int ServiceOfferingId { get; set; }
        public ServiceOffering ServiceOffering { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Notes { get; set; }

        [Required]
        public int CreatingUserId { get; set; }
        public User CreatingUser { get; set; }

        public DateTime? LastModifyTime { get; set; }

        public int? LastModifyingUserId { get; set; }
        public User LastModifyingUser { get; set; }

        public ICollection<ServiceAttendance> Attendances { get; set; }

        public ICollection<ServiceRequestFulfillment> Fulfillments { get; set; }
    }
}
