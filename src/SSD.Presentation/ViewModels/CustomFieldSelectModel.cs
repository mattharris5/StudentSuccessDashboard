using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SSD.ViewModels
{
    public class CustomFieldSelectModel
    {
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Please select a custom field type")]
        public int? SelectedCustomFieldId { get; set; }
        public SelectList CustomFields { get; set; }
    }
}
