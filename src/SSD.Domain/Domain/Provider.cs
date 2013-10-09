using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class Provider
    {
        public Provider()
        {
            Address = new Address();
            Contact = new Contact();
            ApprovingStudents = new List<Student>();
            UserRoles = new List<UserRole>();
            ServiceOfferings = new List<ServiceOffering>();
        }

        public int Id { get; internal set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public Address Address { get; set; }

        [Url]
        public string Website { get; set; }

        public Contact Contact { get; set; }

        public ICollection<Student> ApprovingStudents { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        public ICollection<ServiceOffering> ServiceOfferings { get; set; }

        public bool IsActive { get; set; }
    }
}
