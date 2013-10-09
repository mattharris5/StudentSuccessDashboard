using System.ComponentModel.DataAnnotations;

namespace SSD.ViewModels
{
    public class RemoveApprovedProviderModel
    {
        [Required]
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        [Required]
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
    }
}
