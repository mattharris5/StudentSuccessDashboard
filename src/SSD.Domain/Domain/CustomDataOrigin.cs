using System;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class CustomDataOrigin : IAuditCreate
    {
        public CustomDataOrigin()
        {
            CreateTime = DateTime.Now;
        }

        public int Id { get; internal set; }

        public string FileName { get; set; }

        public bool WasManualEntry { get; set; }

        [StringLength(50)]
        public string Source { get; set; }

        public DateTime CreateTime { get; internal set; }

        [Required]
        public int CreatingUserId { get; set; }
        public User CreatingUser { get; set; }

        [StringLength(255)]
        public string AzureBlobKey { get; set; }
    }
}
