using System;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class EulaAgreement : IAuditCreate
    {
        public int Id { get; internal set; }

        [Required]
        public string EulaText { get; set; }

        public DateTime CreateTime { get; set; }

        [Required]
        public int CreatingUserId { get; set; }
        public User CreatingUser { get; set; }
    }
}
