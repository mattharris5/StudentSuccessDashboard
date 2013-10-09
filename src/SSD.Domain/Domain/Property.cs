using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class Property
    {
        public int Id { get; internal set; }

        [Required]
        [StringLength(255)]
        public string EntityName { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        public bool IsProtected { get; set; }
    }
}
