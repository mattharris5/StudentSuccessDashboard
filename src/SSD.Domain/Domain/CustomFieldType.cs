using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class CustomFieldType
    {
        public int Id { get; internal set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }
    }
}
