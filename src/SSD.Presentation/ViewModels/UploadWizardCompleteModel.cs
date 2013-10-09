using System.Collections.Generic;

namespace SSD.ViewModels
{
    public class UploadWizardCompleteModel
    {
        public UploadWizardCompleteModel()
        {
            RowErrors = new List<string>();
            RowErrorValues = new List<string>();
        }

        public int ProcessedRowCount { get; set; }

        public int SuccessfulRowsCount { get; set; }

        public List<string> RowErrors { get; set; }

        public List<string> RowErrorValues { get; set; }

        public DownloadFileModel ErrorDownloadFile { get; set; }
    }
}
