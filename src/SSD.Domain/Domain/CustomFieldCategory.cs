using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class CustomFieldCategory
    {
        public int Id { get; internal set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        public ICollection<CustomField> Fields { get; set; }
    }
}
