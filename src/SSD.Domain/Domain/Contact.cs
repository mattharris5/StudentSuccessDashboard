using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class Contact
    {
        [StringLength(200)]
        public string Name { get; set; }

        [Phone]
        [StringLength(15)]
        public string Phone { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }
    }
}
