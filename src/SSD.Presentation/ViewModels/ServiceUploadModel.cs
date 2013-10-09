using System.Collections.Generic;

namespace SSD.ViewModels
{
    public class ServiceUploadModel
    {
        public ServiceUploadModel()
        {
            RowErrors = new List<string>();
            RowErrorValues = new List<FileRowModel>();
        }

        public int ServiceOfferingId { get; set; }

        public int ProcessedRowCount { get; set; }

        public int SuccessfulRowsCount { get; set; }

        public List<string> RowErrors { get; set; }

        public List<FileRowModel> RowErrorValues { get; set; }

        public DownloadFileModel ErrorDownloadFile { get; set; }
    }
}
