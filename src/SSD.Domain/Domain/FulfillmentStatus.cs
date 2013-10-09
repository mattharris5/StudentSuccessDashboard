using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class FulfillmentStatus
    {
        public int Id { get; internal set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
    }
}
