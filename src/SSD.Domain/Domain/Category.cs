using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class Category
    {
        public Category()
        {
            ServiceTypes = new List<ServiceType>();
        }

        public int Id { get; internal set; }

        [Required, StringLength(255)]
        public string Name { get; set; }

        public ICollection<ServiceType> ServiceTypes { get; set; }
    }
}
