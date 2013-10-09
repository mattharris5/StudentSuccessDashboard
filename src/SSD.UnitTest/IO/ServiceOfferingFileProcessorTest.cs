using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Controllers;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.Security.Permissions;
using SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace SSD.IO
{
    [TestClass]
    public class ServiceOfferingFileProcessorTest
    {
        private static readonly string UploadFileTemplatePath = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"]);
        private static readonly string ServiceOfferingTemplatePath = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "Templates/" + ServiceOfferingController.TemplateFile);
        private static readonly string DownloadFilePath = Path.GetFullPath(ConfigurationManager.AppSettings["FileDownloadPath"]);

        private TestData TestData { get; set; }
        private EducationSecurityPrincipal User { get; set; }
        private HttpContextBase MockContext { get; set; }
        private ServiceOfferingFileProcessor Target { get; set; }
        private DataTable FileData { get; set; }
        private IBlobClient MockBlobClient { get; set; }
        private IBlobContainer MockBlobContainer { get; set; }
        private TestRepositories Repositories { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            TestData = new TestData();
            User = new EducationSecurityPrincipal(new User { UserKey = "TestKey" });
            MockContext = MockHttpContextFactory.Create();
            MockContext.Expect(m => m.User).Return(User);
            MockBlobClient = MockRepository.GenerateMock<IBlobClient>();
            MockBlobContainer = MockRepository.GenerateMock<IBlobContainer>();
            MockBlobContainer.Expect(m => m.DownloadToStream(null, null)).IgnoreArguments().Do(new Action<string, Stream>((address, target) =>
            {
                byte[] byteArray = File.ReadAllBytes(address);
                target.Write(byteArray, 0, (int)byteArray.Length);
            }));
            MockBlobContainer.Expect(m => m.UploadFromStream(null, null)).IgnoreArguments().Do(new Action<string, Stream>((address, stream) =>
            {
                File.WriteAllBytes(address, ((MemoryStream)stream).ToArray());
            }));
            MockBlobClient.Expect(m => m.CreateContainer(BaseFileProcessor.ServiceFileContainerName)).Return(MockBlobContainer);
            FileData = new DataTable();
            FileData.Columns.Add("blank", typeof(string));
            FileData.Columns.Add("Id", typeof(string));
            FileData.Columns.Add("StartDate", typeof(string));
            FileData.Columns.Add("EndDate", typeof(string));
            FileData.Columns.Add("Notes", typeof(string));
            DataRow row = FileData.NewRow();
            FileData.Rows.Add(row);
            row = FileData.NewRow();
            row["Id"] = "2";
            FileData.Rows.Add(row);
            row = FileData.NewRow();
            row["Id"] = "STUDENT ID";
            row["StartDate"] = "START DATE";
            row["EndDate"] = "END DATE";
            row["Notes"] = "NOTES";
            FileData.Rows.Add(row);
            Repositories = new TestRepositories(TestData);
            IPermissionFactory mockPermissionFactory = MockRepository.GenerateMock<IPermissionFactory>();
            PermissionFactory.SetCurrent(mockPermissionFactory);
            Target = new ServiceOfferingFileProcessor(MockBlobClient, Repositories.MockRepositoryContainer);
        }

        [TestCleanup]
        public void CleanupTests()
        {
            ProcessDirectoryPath(DownloadFilePath);
        }

        [TestMethod]
        public void GivenNullBlobClient_WhenIConstruct_ThenArgumentNullExceptionIsThrown()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceOfferingFileProcessor(null, Repositories.MockRepositoryContainer));
        }

        [TestMethod]
        public void GivenNullRepositories_WhenIConstruct_ThenArgumentNullExceptionIsThrown()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceOfferingFileProcessor(MockBlobClient, null));
        }

        [TestMethod]
        public void GivenIConsumeAValidServiceUploadFile_ThenADataTableIsReturned()
        {
            var filePath = UploadFileTemplatePath + "HappyPath.xlsx";
            var destinationPath = CopyTestFile(filePath);

            using (FileStream fs = File.Open(destinationPath, FileMode.Open))
            {
                var uploadFile = new UploadExcelFileModel();
                uploadFile.File = MockRepository.GenerateMock<HttpPostedFileBase>();
                uploadFile.File.Expect(u => u.InputStream).Return(fs);

                var table = Target.ConsumeFile(uploadFile);

                Assert.IsNotNull(table);
            }

            DestroyTestFile(destinationPath);
        }

        [TestMethod]
        public void GivenIConsumeAValidServiceOfferingUploadFile_ThenTheCorrectDataTableIsReturned()
        {
            var filePath = UploadFileTemplatePath + "HappyPath.xlsx";
            var destinationPath = CopyTestFile(filePath);
            var model = new ServiceUploadModel();

            DataTable expected = new DataTable();
            expected.Columns.Add("blank", typeof(string));
            expected.Columns.Add("Id", typeof(string));
            expected.Columns.Add("StartDate", typeof(string));
            expected.Columns.Add("EndDate", typeof(string));
            expected.Columns.Add("Notes", typeof(string));
            expected.Columns.Add(new DataColumn());
            expected.Columns.Add(new DataColumn());
            expected.Columns.Add(new DataColumn());
            expected.Columns.Add(new DataColumn());
            expected.Columns.Add(new DataColumn());
            expected.Columns.Add(new DataColumn());
            expected.Columns.Add(new DataColumn());
            DataRow row = expected.NewRow();
            row["Id"] = "Upload and Assign Students to a Service Offering";
            expected.Rows.Add(row);
            row = expected.NewRow();
            row["Id"] = "1";
            row["StartDate"] = "YMCA";
            row["EndDate"] = "After School";
            expected.Rows.Add(row);
            row = expected.NewRow();
            row["Id"] = "STUDENT ID";
            row["StartDate"] = "START DATE";
            row["EndDate"] = "END DATE";
            row["Notes"] = "NOTES";
            expected.Rows.Add(row);
            row = expected.NewRow();
            row["Id"] = "10";
            row["StartDate"] = "40920";
            row["EndDate"] = "40920";
            row["Notes"] = "testing";
            expected.Rows.Add(row);
            row = expected.NewRow();
            row["Id"] = "20";
            row["StartDate"] = "40962";
            row["EndDate"] = "40963";
            row["Notes"] = "testing 2";
            expected.Rows.Add(row);

            using (FileStream fs = File.Open(destinationPath, FileMode.Open))
            {
                var uploadFile = new UploadExcelFileModel();
                uploadFile.File = MockRepository.GenerateMock<HttpPostedFileBase>();
                uploadFile.File.Expect(u => u.InputStream).Return(fs);

                var actual = Target.ConsumeFile(uploadFile);

                CollectionAssert.AreEqual(expected.Rows[0].ItemArray, actual.Rows[0].ItemArray);
                CollectionAssert.AreEqual(expected.Rows[1].ItemArray, actual.Rows[1].ItemArray);
                CollectionAssert.AreEqual(expected.Rows[2].ItemArray, actual.Rows[2].ItemArray);
                CollectionAssert.AreEqual(expected.Rows[3].ItemArray, actual.Rows[3].ItemArray);
                CollectionAssert.AreEqual(expected.Rows[4].ItemArray, actual.Rows[4].ItemArray);
            }

            DestroyTestFile(destinationPath);
        }

        [TestMethod]
        public void GivenIConsumeAValidServiceUploadFile_ThenTheModelHasNoErrors()
        {
            var filePath = UploadFileTemplatePath + "HappyPath.xlsx";
            var destinationPath = CopyTestFile(filePath);
            var model = new ServiceUploadModel();

            using (FileStream fs = File.Open(destinationPath, FileMode.Open))
            {
                var uploadFile = new UploadExcelFileModel();
                uploadFile.File = MockRepository.GenerateMock<HttpPostedFileBase>();
                uploadFile.File.Expect(u => u.InputStream).Return(fs);

                var table = Target.ConsumeFile(uploadFile);

                Assert.IsNotNull(model);
                Assert.IsTrue(model.RowErrors.Count == 0);
            }

            DestroyTestFile(destinationPath);
        }

        [TestMethod]
        public void GivenTableIsValid_WhenImportServiceOfferings_ThenModelHasNoErrors()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            FileData.Rows.Add(row);
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(MockRepository.GenerateMock<IPermission>());
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is("ScheduleOffering"), Arg<object[]>.Matches(args => PermissionArgIsStudent(args, TestData.Students[0])))).Return(MockRepository.GenerateMock<IPermission>());

            ServiceUploadModel actual = Target.Import(User, ServiceOfferingTemplatePath, FileData);

            Assert.AreEqual(0, actual.RowErrors.Count);
            Assert.AreEqual(1, actual.SuccessfulRowsCount);
            Assert.AreEqual(1, actual.ProcessedRowCount);
        }

        [TestMethod]
        public void GivenTableHeaderIsInvalid_WhenImportServiceOfferings_ThenModelErrors()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            FileData.Rows.Add(row);
            FileData.Rows[1]["Id"] = "";

            ServiceUploadModel actual = Target.Import(User, ServiceOfferingTemplatePath, FileData);

            Assert.AreEqual(1, actual.RowErrors.Count);
            Assert.AreEqual(0, actual.SuccessfulRowsCount);
            Assert.AreEqual(0, actual.ProcessedRowCount);
        }

        [TestMethod]
        public void GivenServiceOfferingDoesNotExist_WhenImportServiceOfferings_ThenModelErrors()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            FileData.Rows.Add(row);
            FileData.Rows[1]["Id"] = "0";

            ServiceUploadModel actual = Target.Import(User, ServiceOfferingTemplatePath, FileData);

            Assert.AreEqual(1, actual.RowErrors.Count);
            Assert.AreEqual(0, actual.SuccessfulRowsCount);
            Assert.AreEqual(0, actual.ProcessedRowCount);
        }

        [TestMethod]
        public void GivenServiceOfferingInactive_WhenImport_ThenModelErrors()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            FileData.Rows.Add(row);
            FileData.Rows[1]["Id"] = TestData.ServiceOfferings.First(s => !s.IsActive).Id;

            ServiceUploadModel actual = Target.Import(User, ServiceOfferingTemplatePath, FileData);

            Assert.AreEqual(1, actual.RowErrors.Count);
            Assert.AreEqual(0, actual.SuccessfulRowsCount);
            Assert.AreEqual(0, actual.ProcessedRowCount);
        }

        [TestMethod]
        public void WhenImportServiceOfferings_ThenPermissionAccessRequested()
        {
            IPermission mockPermission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(mockPermission);

            Target.Import(User, ServiceOfferingTemplatePath, FileData);

            mockPermission.AssertWasCalled(m => m.GrantAccess(User));
        }

        [TestMethod]
        public void GivenNoStudentSISId_WhenImportServices_ThenTheModelHasErrors()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "";
            FileData.Rows.Add(row);
            int expected = 1;
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(MockRepository.GenerateMock<IPermission>());

            ServiceUploadModel actual = Target.Import(User, ServiceOfferingTemplatePath, FileData);

            Assert.AreEqual(expected, actual.RowErrors.Count);
            Assert.AreEqual("Malformed Student Id on row 4", actual.RowErrors[0]);
        }

        [TestMethod]
        public void GivenStartDateIsInIncorrectFormat_WhenImportServices_ThenTheModelHasErrors()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            row["StartDate"] = "41390a";
            FileData.Rows.Add(row);
            int expected = 1;
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(MockRepository.GenerateMock<IPermission>());
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is("ScheduleOffering"), Arg<object[]>.Matches(args => PermissionArgIsStudent(args, TestData.Students[0])))).Return(MockRepository.GenerateMock<IPermission>());

            ServiceUploadModel actual = Target.Import(User, ServiceOfferingTemplatePath, FileData);

            Assert.AreEqual(expected, actual.RowErrors.Count);
            Assert.AreEqual("Malformed Start Date on row 4", actual.RowErrors[0]);
        }

        [TestMethod]
        public void GivenEndDateIsInIncorrectFormat_WhenImportServices_ThenTheModelHasErrors()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            row["StartDate"] = "41390";
            row["EndDate"] = "41390a";
            FileData.Rows.Add(row);
            int expected = 1;
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(MockRepository.GenerateMock<IPermission>());
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is("ScheduleOffering"), Arg<object[]>.Matches(args => PermissionArgIsStudent(args, TestData.Students[0])))).Return(MockRepository.GenerateMock<IPermission>());

            ServiceUploadModel actual = Target.Import(User, ServiceOfferingTemplatePath, FileData);

            Assert.AreEqual(expected, actual.RowErrors.Count);
            Assert.AreEqual("Malformed End Date on row 4", actual.RowErrors[0]);
        }

        [TestMethod]
        public void WhenImportServiceOfferings_ThenPermissionGrantAccessCalledForStudent()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "20";
            FileData.Rows.Add(row);
            FileData.Rows[1]["Id"] = "1";
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[0])).Return(MockRepository.GenerateMock<IPermission>());
            IPermission mockPermission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is("ScheduleOffering"), Arg<object[]>.Matches(args => PermissionArgIsStudent(args, TestData.Students[1])))).Return(mockPermission);

            Target.Import(User, ServiceOfferingTemplatePath, FileData);

            mockPermission.AssertWasCalled(m => m.GrantAccess(User));
        }

        [TestMethod]
        public void GivenValidServiceOffering_WhenCreateTemplateDownload_ThenADownloadFileViewModelIsReturned()
        {
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[0])).Return(MockRepository.GenerateMock<IPermission>());

            var model = Target.CreateTemplateDownload(User, ServiceOfferingTemplatePath, 1) as DownloadFileModel;

            Assert.IsNotNull(model);
            DestroyTestFile(model.BlobAddress);
        }

        [TestMethod]
        public void GivenAnInvalidServiceOffering_WhenCreateTemplateDownload_ThenAModelIsNotReturned()
        {
            var model = Target.CreateTemplateDownload(User, ServiceOfferingTemplatePath, 15675) as DownloadFileModel;

            Assert.IsNull(model);
        }

        [TestMethod]
        public void WhenIRetrieveUploadErrorFiles_ThenADownloadFileViewModelIsReturned()
        {
            var model = Target.RetrieveUploadErrorsFile(ServiceOfferingTemplatePath) as DownloadFileModel;

            Assert.IsNotNull(model);
            Assert.IsTrue(model.FileContentStream.Length > 0);
        }

        private string CopyTestFile(string currentPath)
        {
            var destinationPath = currentPath.Replace(".xlsx", "-test.xlsx");
            File.Copy(currentPath, destinationPath, true);
            return destinationPath;
        }

        private void DestroyTestFile(string testPath)
        {
            if (testPath != null && testPath.Length > 0)
            {
                File.Delete(testPath);
            }
        }

        private void ProcessDirectoryPath(string directory)
        {
            var directoryAbsolute = Path.GetFullPath(directory);
            var directoryInfo = new DirectoryInfo(directoryAbsolute);
            if (directoryInfo.Exists)
            {
                CleanupDirectory(directoryInfo);
            }
        }

        private void CleanupDirectory(DirectoryInfo directoryInfo)
        {
            var directoryFiles = directoryInfo.GetFiles("*.xlsx");
            foreach (var file in directoryFiles)
            {
                File.Delete(file.FullName);
            }
        }

        private static bool PermissionArgIsStudent(object[] args, Student student)
        {
            var list = args[0] as IEnumerable<Student>;
            return list.Single() == student;
        }
    }
}