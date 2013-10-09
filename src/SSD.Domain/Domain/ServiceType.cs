using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class ServiceType
    {
        public ServiceType()
        {
            Categories = new List<Category>();
            ServiceOfferings = new List<ServiceOffering>();
        }

        public int Id { get; internal set; }

        public bool IsActive { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Category> Categories { get; set; }

        public bool IsPrivate { get; set; }

        public ICollection<ServiceOffering> ServiceOfferings { get; set; }
    }
}
