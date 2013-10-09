using SSD.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public abstract class CustomField : IAuditCreate, IAuditModify
    {
        public CustomField()
        {
            Categories = new List<CustomFieldCategory>();
            CreateTime = DateTime.Now;
        }

        public int Id { get; internal set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public DateTime CreateTime { get; internal set; }

        [Required]
        public int CreatingUserId { get; set; }
        public User CreatingUser { get; set; }

        public DateTime? LastModifyTime { get; set; }

        public int? LastModifyingUserId { get; set; }
        public User LastModifyingUser { get; set; }

        [Required]
        public int CustomFieldTypeId { get; set; }
        public CustomFieldType CustomFieldType { get; set; }

        [RequiredElements]
        public ICollection<CustomFieldCategory> Categories { get; set; }
    }
}
