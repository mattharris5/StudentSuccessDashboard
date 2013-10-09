using System.IO;

namespace SSD.ViewModels
{
    public class DownloadFileModel
    {
        public string BlobAddress { get; set; }
        public string FileName { get; set; }
        public Stream FileContentStream { get; set; }
    }
}
