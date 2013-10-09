using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class Role
    {
        public Role()
        {
            UserRoles = new List<UserRole>();
        }

        public int Id { get; internal set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
