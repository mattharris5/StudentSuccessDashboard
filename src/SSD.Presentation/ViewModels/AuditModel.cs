using System;

namespace SSD.ViewModels
{
    public class AuditModel
    {
        public DateTime CreateTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifyTime { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
