using SSD.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SSD.ViewModels
{
    public class UploadWizardFileViewModel : UploadWizardModel
    {
        [Required]
        [FileSize(1, 2000240)]
        [FileTypes("csv,tsv,txt")]
        public HttpPostedFileBase File { get; set; }
    }
}
