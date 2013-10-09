using SSD.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD.ViewModels
{
    public class UploadWizardModel
    {
        public UploadWizardModel()
        {
            CustomFields = new List<CustomFieldSelectModel>();
        }

        public string BlobAddress { get; set; }

        [Required(ErrorMessage = "Source is required")]
        public string Source { get; set; }

        [Required(ErrorMessage = "Field name row is required.")]
        public int FieldNameRow { get; set; }
        
        [Required(ErrorMessage = "Number of fields is required.")]
        public int NumberOfFields { get; set; }

        [Required(ErrorMessage = "First data row is required.")]
        [NumericLessThan("LastDataRow", AllowEquality = true)]
        public int FirstDataRow { get; set; }

        [Required(ErrorMessage = "Last data row is required")]
        [NumericGreaterThan("FirstDataRow", AllowEquality = true)]
        public int LastDataRow { get; set; }

        public List<CustomFieldSelectModel> CustomFields { get; set; }
    }
}
