using SSD.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SSD.ViewModels
{
    public class UploadExcelFileModel
    {
        [Required]
        [FileSize(1, 2000240)]
        [FileTypes("xlsx")]
        public HttpPostedFileBase File { get; set; }
    }
}
