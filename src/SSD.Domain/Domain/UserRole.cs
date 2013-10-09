using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class UserRole : IAuditCreate, IAuditModify
    {
        public UserRole()
        {
            Schools = new List<School>();
            Providers = new List<Provider>();
            CreateTime = DateTime.Now;
        }

        public int Id { get; internal set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public DateTime CreateTime { get; internal set; }

        [Required]
        public int CreatingUserId { get; set; }
        public User CreatingUser { get; set; }

        public DateTime? LastModifyTime { get; set; }

        public int? LastModifyingUserId { get; set; }
        public User LastModifyingUser { get; set; }

        public ICollection<School> Schools { get; set; }

        public ICollection<Provider> Providers { get; set; }
    }
}
