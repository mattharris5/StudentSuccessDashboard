using System.Collections.Generic;

namespace SSD.ViewModels
{
    public class ServiceOfferingUploadModel
    {
        public ServiceOfferingUploadModel()
        {
            RowErrors = new List<string>();
            RowErrorValues = new List<ServiceOfferingFileRowViewModel>();
        }

        public int CreatingUserId { get; set; }

        public int ServiceOfferingId { get; set; }

        public int ProcessedRowCount { get; set; }
        
        public int SuccessfulRowsCount { get; set; }

        public List<string> RowErrors { get; set; }

        public List<ServiceOfferingFileRowViewModel> RowErrorValues { get; set; }

        public DownloadFileModel ErrorDownloadFile { get; set; }
    }
}
