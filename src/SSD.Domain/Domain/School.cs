using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class School
    {
        public School()
        {
            Programs = new List<Program>();
            UserRoles = new List<UserRole>();
            Students = new List<Student>();
        }

        public int Id { get; internal set; }

        [StringLength(68)]
        public string SchoolKey { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<Program> Programs { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}
