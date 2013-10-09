using System;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class ServiceAttendance : IAuditCreate, IAuditModify
    {
        public ServiceAttendance()
        {
            CreateTime = DateTime.Now;
        }

        public int Id { get; internal set; }

        [Required]
        public int StudentAssignedOfferingId { get; set; }
        public StudentAssignedOffering StudentAssignedOffering { get; set; }

        [Required]
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        [Required]
        public DateTime DateAttended { get; set; }

        public decimal Duration { get; set; }

        public string Notes { get; set; }

        public DateTime CreateTime { get; internal set; }

        [Required]
        public int CreatingUserId { get; set; }
        public User CreatingUser { get; set; }

        public DateTime? LastModifyTime { get; set; }

        public int? LastModifyingUserId { get; set; }
        public User LastModifyingUser { get; set; }
    }
}
