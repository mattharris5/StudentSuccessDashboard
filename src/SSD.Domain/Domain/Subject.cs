using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class Subject
    {
        public const string DefaultName = "None";

        public int Id { get; internal set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }
}
