using System;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class EulaAcceptance : IAuditCreate
    {
        public int Id { get; internal set; }

        public int EulaAgreementId { get; set; }
        public EulaAgreement EulaAgreement { get; set; }

        public DateTime CreateTime { get; set; }

        [Required]
        public int CreatingUserId { get; set; }
        public User CreatingUser { get; set; }
    }
}
