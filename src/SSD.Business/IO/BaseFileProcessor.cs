using Microsoft.WindowsAzure;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.Security.Permissions;
using SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SSD.IO
{
    public abstract class BaseFileProcessor : IFileProcessor
    {
        public static readonly string ServiceFileContainerName = CloudConfigurationManager.GetSetting("ServiceOfferingFileContainerName");
        protected const string ServiceOfferingSheetName = "Assign Service Offering";
        protected static readonly int StartRow = 3;

        protected BaseFileProcessor(IBlobClient blobClient, IRepositoryContainer repositories)
        {
            if (blobClient == null)
            {
                throw new ArgumentNullException("blobClient");
            }
            if (repositories == null)
            {
                throw new ArgumentNullException("repositories");
            }
            BlobClient = blobClient;
            RepositoryContainer = repositories;
            ServiceOfferingRepository = repositories.Obtain<IServiceOfferingRepository>();
            StudentRepository = repositories.Obtain<IStudentRepository>();
            StudentAssignedOfferingRepository = repositories.Obtain<IStudentAssignedOfferingRepository>();
            SubjectRepository = repositories.Obtain<ISubjectRepository>();
            ServiceAttendanceRepository = repositories.Obtain<IServiceAttendanceRepository>();
        }

        private IBlobClient BlobClient { get; set; }
        private IRepositoryContainer RepositoryContainer { get; set; }
        protected IServiceOfferingRepository ServiceOfferingRepository { get; private set; }
        protected IStudentRepository StudentRepository { get; private set; }
        protected IStudentAssignedOfferingRepository StudentAssignedOfferingRepository { get; private set; }
        protected ISubjectRepository SubjectRepository { get; private set; }
        protected IServiceAttendanceRepository ServiceAttendanceRepository { get; private set; }

        public DataTable ConsumeFile(UploadExcelFileModel uploadFile)
        {
            var memoryStream = new MemoryStream();
            uploadFile.File.InputStream.CopyTo(memoryStream);
            return ExcelParser.ExtractExcelSheetValues(memoryStream, ServiceOfferingSheetName);
        }

        public ServiceUploadModel Import(EducationSecurityPrincipal user, string templatePath, DataTable dataTable)
        {
            var headerRow = dataTable.Rows[1];
            int serviceOfferingId;
            ServiceOffering offering = null;
            if (int.TryParse(headerRow[1].ToString(), out serviceOfferingId))
            {
                offering = ServiceOfferingRepository.Items.Include(s => s.Provider).
                                                           Include(s => s.ServiceType).
                                                           Include(s => s.Program).
                                                           SingleOrDefault(s => s.Id == serviceOfferingId);
            }
            ServiceUploadModel model = new ServiceUploadModel();
            if (offering != null && offering.IsActive)
            {
                serviceOfferingId = int.Parse(headerRow[1].ToString());
                if (!GrantUserAccessToUploadOffering(user, offering))
                {
                    throw new EntityAccessUnauthorizedException("Not authorized to schedule service offerings with this provider.");
                }
                model.ServiceOfferingId = serviceOfferingId;
                ProcessDataTable(user, dataTable, model);
                RepositoryContainer.Save();
                if (model.RowErrors.Any())
                {
                    ProcessErrorFile(user, offering, model, templatePath);
                }
            }
            else
            {
                model.ProcessedRowCount = model.SuccessfulRowsCount = 0;
                model.RowErrors.Add("Invalid Service Offering ID");
            }
            return model;
        }

        public void ProcessError(DataRow row, string rowError, ServiceUploadModel model)
        {
            model.RowErrors.Add(rowError);
            CreateFileUploadErrorRows(row, model);
        }

        public DownloadFileModel CreateTemplateDownload(EducationSecurityPrincipal user, string templatePath, int serviceOfferingId)
        {
            var offering = ServiceOfferingRepository.Items.Include(s => s.Provider).
                                                           Include(s => s.ServiceType).
                                                           Include(s => s.Program).
                                                           SingleOrDefault(s => s.Id == serviceOfferingId);
            if (offering == null)
            {
                return null;
            }
            if (!GrantUserAccessToUploadOffering(user, offering))
            {
                throw new EntityAccessUnauthorizedException("You are not have the appropriate rights to download this service offering.");
            }
            var processor = new WorksheetWriter(offering, ServiceOfferingSheetName);
            var fileName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", offering.Name.GetSafeFileName(), ".xlsx");
            var downloadFile = new DownloadFileModel
            {
                FileName = fileName,
                BlobAddress = string.Empty
            };
            ExcelWriter writer = new ExcelWriter();
            writer.InitializeFrom(templatePath, processor);
            downloadFile.FileContentStream = writer.FileContentStream;
            downloadFile.FileContentStream.Position = 0;
            return downloadFile;
        }

        public DownloadFileModel RetrieveUploadErrorsFile(string blobAddress)
        {
            var memoryStream = new MemoryStream();
            var container = BlobClient.CreateContainer(ServiceFileContainerName);
            container.DownloadToStream(blobAddress, memoryStream);
            var downloadFile = new DownloadFileModel
            {
                FileName = blobAddress,
                FileContentStream = memoryStream
            };
            downloadFile.FileContentStream.Position = 0;
            return downloadFile;
        }

        protected abstract void ProcessDataTable(EducationSecurityPrincipal user, DataTable dataTable, ServiceUploadModel model);

        protected abstract void CreateFileUploadErrorRows(DataRow row, ServiceUploadModel model);

        private IWorksheetWriter CreateWorksheetWriter(ServiceOffering offering)
        {
            return new WorksheetWriter(offering, ServiceOfferingSheetName);
        }

        private void ProcessErrorFile(EducationSecurityPrincipal user, ServiceOffering offering, ServiceUploadModel model, string templatePath)
        {
            var fileName = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", offering.Name.GetSafeFileName(), DateTime.Now.Ticks, ".xlsx");
            var blobAddress = string.Format("{0}-{1}-{2}", user.Identity.User.DisplayName, user.Identity.User.Id, fileName);
            model.ErrorDownloadFile = new DownloadFileModel
            {
                FileName = fileName,
                BlobAddress = blobAddress
            };
            var worksheetWriter = CreateWorksheetWriter(offering);
            ExcelWriter writer = new ExcelWriter();
            writer.InitializeFrom(templatePath, worksheetWriter);
            foreach (var error in model.RowErrorValues)
            {
                worksheetWriter.ErrorRows.Add(error);
            }
            writer.AppendErrorRows(ServiceOfferingSheetName, worksheetWriter);
            writer.Write(BlobClient.CreateContainer(ServiceFileContainerName), model.ErrorDownloadFile.BlobAddress);
        }

        private bool GrantUserAccessToUploadOffering(EducationSecurityPrincipal user, ServiceOffering offering)
        {
            IPermission permission = PermissionFactory.Current.Create("ImportOfferingData", offering);
            return permission.TryGrantAccess(user);
        }

        protected bool GrantUserAccessToSchedulingAnOffering(EducationSecurityPrincipal user, Student student)
        {
            IPermission permission = PermissionFactory.Current.Create("ScheduleOffering", new List<Student> { student });
            return permission.TryGrantAccess(user);
        }
    }
}
