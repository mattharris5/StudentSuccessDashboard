using SSD.Security;
using SSD.ViewModels;
using System.Data;

namespace SSD.IO
{
    public interface IFileProcessor
    {
        DataTable ConsumeFile(UploadExcelFileModel uploadFile);
        ServiceUploadModel Import(EducationSecurityPrincipal user, string templatePath, DataTable dataTable);
        void ProcessError(DataRow row, string rowError, ServiceUploadModel model);
        DownloadFileModel CreateTemplateDownload(EducationSecurityPrincipal user, string templatePath, int serviceOfferingId);
        DownloadFileModel RetrieveUploadErrorsFile(string blobAddress);
    }
}
