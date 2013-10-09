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
    public class ServiceAttendanceFileProcessorTest
    {
        private static readonly string UploadFileTemplatePath = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"]);
        private static readonly string ServiceAttendanceTemplatePath = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "Templates/" + ServiceAttendanceController.TemplateFile);
        private static readonly string DownloadFilePath = Path.GetFullPath(ConfigurationManager.AppSettings["FileDownloadPath"]);

        private TestData TestData { get; set; }
        private EducationSecurityPrincipal User { get; set; }
        private HttpContextBase MockContext { get; set; }
        private ServiceAttendanceFileProcessor Target { get; set; }
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
            MockBlobClient.Expect(m => m.CreateContainer(BaseFileProcessor.ServiceFileContainerName)).IgnoreArguments().Return(MockBlobContainer);
            FileData = new DataTable();
            FileData.Columns.Add("blank", typeof(string));
            FileData.Columns.Add("Id", typeof(string));
            FileData.Columns.Add("DateAttended", typeof(string));
            FileData.Columns.Add("Subject", typeof(string));
            FileData.Columns.Add("Duration", typeof(string));
            FileData.Columns.Add("Notes", typeof(string));
            DataRow row = FileData.NewRow();
            FileData.Rows.Add(row);
            row = FileData.NewRow();
            row["Id"] = "2";
            FileData.Rows.Add(row);
            row = FileData.NewRow();
            row["Id"] = "STUDENT ID";
            row["DateAttended"] = "DATE";
            row["Subject"] = "SUBJECT";
            row["Duration"] = "DURATION";
            row["Notes"] = "NOTES";
            FileData.Rows.Add(row);
            Repositories = new TestRepositories(TestData);
            IPermissionFactory mockPermissionFactory = MockRepository.GenerateMock<IPermissionFactory>();
            PermissionFactory.SetCurrent(mockPermissionFactory);
            Target = new ServiceAttendanceFileProcessor(MockBlobClient, Repositories.MockRepositoryContainer);
        }

        [TestCleanup]
        public void CleanupTests()
        {
            ProcessDirectoryPath(DownloadFilePath);
        }

        [TestMethod]
        public void GivenNullBlobClient_WhenIConstruct_ThenArgumentNullExceptionIsThrown()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceAttendanceFileProcessor(null, Repositories.MockRepositoryContainer));
        }

        [TestMethod]
        public void GivenNullRepositories_WhenIConstruct_ThenArgumentNullExceptionIsThrown()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceAttendanceFileProcessor(MockBlobClient, null));
        }

        [TestMethod]
        public void GivenIConsumeAValidServiceAttendanceUploadFile_ThenADataTableIsReturned()
        {
            var filePath = UploadFileTemplatePath + "ValidServiceAttendanceUpload.xlsx";
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
        public void GivenIConsumeAValidServiceAttendanceUploadFile_ThenTheCorrectDataTableIsReturned()
        {
            var filePath = UploadFileTemplatePath + "ValidServiceAttendanceUpload.xlsx";
            var destinationPath = CopyTestFile(filePath);

            DataTable expected = new DataTable();
            expected.Columns.Add("blank", typeof(string));
            expected.Columns.Add("Id", typeof(string));
            expected.Columns.Add("DateAttended", typeof(string));
            expected.Columns.Add("Subject", typeof(string));
            expected.Columns.Add("Duration", typeof(string));
            expected.Columns.Add("Notes", typeof(string));
            expected.Columns.Add(new DataColumn());
            expected.Columns.Add(new DataColumn());
            expected.Columns.Add(new DataColumn());
            expected.Columns.Add(new DataColumn());
            expected.Columns.Add(new DataColumn());
            expected.Columns.Add(new DataColumn());
            DataRow row = expected.NewRow();
            row["Id"] = "Upload Attendance Records for a Service Offering";
            expected.Rows.Add(row);
            row = expected.NewRow();
            row["Id"] = "1";
            row["DateAttended"] = "YMCA";
            row["Subject"] = "After School";
            expected.Rows.Add(row);
            row = expected.NewRow();
            row["Id"] = "STUDENT ID";
            row["DateAttended"] = "DATE";
            row["Subject"] = "SUBJECT";
            row["Duration"] = "DURATION";
            row["Notes"] = "NOTES";
            expected.Rows.Add(row);
            row = expected.NewRow();
            row["Id"] = "10";
            row["DateAttended"] = "41390";
            row["Subject"] = "test subject 3";
            row["Duration"] = "30";
            row["Notes"] = "Further testing";
            expected.Rows.Add(row);
            row = expected.NewRow();
            row["Id"] = "20";
            row["DateAttended"] = "41391";
            row["Subject"] = "test subject 4";
            row["Duration"] = "40";
            row["Notes"] = "Even further testing";
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
        public void GivenTableIsValid_WhenImportServiceAttendances_ThenModelHasNoErrors()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            row["DateAttended"] = "41390";
            FileData.Rows.Add(row);
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(MockRepository.GenerateMock<IPermission>());
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is("ScheduleOffering"), Arg<object[]>.Matches(args => PermissionArgIsStudent(args, TestData.Students[0])))).Return(MockRepository.GenerateMock<IPermission>());

            ServiceUploadModel actual = Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            Assert.AreEqual(0, actual.RowErrors.Count);
            Assert.AreEqual(1, actual.SuccessfulRowsCount);
            Assert.AreEqual(1, actual.ProcessedRowCount);
        }

        [TestMethod]
        public void GivenTableHeaderIsInvalid_WhenImportServiceAttendances_ThenModelErrors()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            row["DateAttended"] = "41390";
            FileData.Rows.Add(row);
            FileData.Rows[1]["Id"] = "";

            ServiceUploadModel actual = Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            Assert.AreEqual(1, actual.RowErrors.Count);
            Assert.AreEqual(0, actual.SuccessfulRowsCount);
            Assert.AreEqual(0, actual.ProcessedRowCount);
        }

        [TestMethod]
        public void GivenServiceOfferingDoesNotExist_WhenImportServiceAttendances_ThenModelErrors()
        {
            ServiceUploadModel model = new ServiceUploadModel();
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            row["DateAttended"] = "41390";
            FileData.Rows.Add(row);
            FileData.Rows[1]["Id"] = "0";

            ServiceUploadModel actual = Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            Assert.AreEqual(1, actual.RowErrors.Count);
            Assert.AreEqual(0, actual.SuccessfulRowsCount);
            Assert.AreEqual(0, actual.ProcessedRowCount);
        }

        [TestMethod]
        public void WhenImportServiceAttendances_ThenPermissionAccessRequested()
        {
            IPermission mockPermission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(mockPermission);

            Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            mockPermission.AssertWasCalled(m => m.GrantAccess(User));
        }

        [TestMethod]
        public void GivenTableIsValid_WhenImportServiceAttendances_ThenModelRowCountReflectsChanges()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            row["DateAttended"] = "41390";
            FileData.Rows.Add(row);
            int expected = 1;
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(MockRepository.GenerateMock<IPermission>());
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is("ScheduleOffering"), Arg<object[]>.Matches(args => PermissionArgIsStudent(args, TestData.Students[0])))).Return(MockRepository.GenerateMock<IPermission>());

            ServiceUploadModel actual = Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            Assert.AreEqual(expected, actual.SuccessfulRowsCount);
            Assert.AreEqual(expected, actual.ProcessedRowCount);
        }

        [TestMethod]
        public void GivenTableIsInvalid_WhenImportServiceAttendances_ThenModelReflectsErrors()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10a";
            row["DateAttended"] = "41390";
            FileData.Rows.Add(row);
            int expected = 1;
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(MockRepository.GenerateMock<IPermission>());
            
            ServiceUploadModel actual = Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            Assert.AreEqual(expected, actual.RowErrors.Count);
        }

        [TestMethod]
        public void GivenTableIsInvalid_WhenImportServiceAttendances_ThenErrorFileNameHasServiceOfferingName()
        {
            ServiceOffering offering = TestData.ServiceOfferings[1];
            DataRow row = FileData.NewRow();
            row["Id"] = "10a";
            row["DateAttended"] = "41390";
            FileData.Rows.Add(row);
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", offering)).Return(MockRepository.GenerateMock<IPermission>());

            ServiceUploadModel actual = Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            StringAssert.Contains(actual.ErrorDownloadFile.FileName, offering.Name.GetSafeFileName());
        }

        [TestMethod]
        public void GivenValidTable_WhenICallImportServiceAttendances_ThenTheModelHasNoErrors()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            row["DateAttended"] = "41390";
            FileData.Rows.Add(row);
            int expected = 0;
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(MockRepository.GenerateMock<IPermission>());
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is("ScheduleOffering"), Arg<object[]>.Matches(args => PermissionArgIsStudent(args, TestData.Students[0])))).Return(MockRepository.GenerateMock<IPermission>());

            ServiceUploadModel actual = Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            Assert.AreEqual(expected, actual.RowErrors.Count);
        }

        [TestMethod]
        public void GivenNoStudentSISId_WhenImportServiceAttendances_ThenTheModelHasErrors()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "";
            row["DateAttended"] = "41390";
            FileData.Rows.Add(row);
            int expected = 1;
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(MockRepository.GenerateMock<IPermission>());
            
            ServiceUploadModel actual = Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            Assert.AreEqual(expected, actual.RowErrors.Count);
            Assert.AreEqual("Malformed Student Id on row 4", actual.RowErrors[0]);
        }

        [TestMethod]
        public void GivenDateIsInIncorrectFormat_WhenImportServiceAttendances_ThenTheModelHasErrors()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            row["DateAttended"] = "41390a";
            FileData.Rows.Add(row);
            int expected = 1;
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(MockRepository.GenerateMock<IPermission>());
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is("ScheduleOffering"), Arg<object[]>.Matches(args => PermissionArgIsStudent(args, TestData.Students[0])))).Return(MockRepository.GenerateMock<IPermission>());

            ServiceUploadModel actual = Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            Assert.AreEqual(expected, actual.RowErrors.Count);
            Assert.AreEqual("Malformed Date Attended on row 4", actual.RowErrors[0]);
        }

        [TestMethod]
        public void GivenUserEntersInvalidSubject_WhenImport_ThenTheModelHasErrors()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            row["DateAttended"] = "41390";
            row["Subject"] = "blah";
            FileData.Rows.Add(row);
            int expected = 1;
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(MockRepository.GenerateMock<IPermission>());
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is("ScheduleOffering"), Arg<object[]>.Matches(args => PermissionArgIsStudent(args, TestData.Students[0])))).Return(MockRepository.GenerateMock<IPermission>());

            ServiceUploadModel actual = Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            Assert.AreEqual(expected, actual.RowErrors.Count);
            Assert.AreEqual("Malformed Subject on row 4", actual.RowErrors[0]);
        }

        [TestMethod]
        public void GivenUserEntersNoSubject_WhenImport_ThenUseDefaultSubject()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            row["DateAttended"] = "41390";
            row["Subject"] = string.Empty;
            FileData.Rows.Add(row);
            int expectedErrorCount = 0;
            StudentAssignedOffering expectedStudentAssignedOffering = TestData.StudentAssignedOfferings.Single(a => a.Student.StudentSISId == "10" && a.Id == 3);
            DateTime expectedDateAttended = DateTime.FromOADate(41390);
            Subject expectedSubject = TestData.Subjects[0];
            decimal expectedDuration = 0;
            string expectedNotes = string.Empty;
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(MockRepository.GenerateMock<IPermission>());
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is("ScheduleOffering"), Arg<object[]>.Matches(args => PermissionArgIsStudent(args, TestData.Students[0])))).Return(MockRepository.GenerateMock<IPermission>());

            ServiceUploadModel actual = Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            Assert.AreEqual(expectedErrorCount, actual.RowErrors.Count);
            Repositories.MockServiceAttendanceRepository.AssertWasCalled(m => m.Add(Arg<ServiceAttendance>.Matches(a => MatchProperties(a, expectedStudentAssignedOffering, expectedDateAttended, expectedSubject, expectedDuration, expectedNotes))));
        }

        [TestMethod]
        public void GivenValidTable_WhenICallImportServiceAttendances_ThenAddedServiceAttendanceHasCreateAudited()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            row["DateAttended"] = "41390";
            FileData.Rows.Add(row);
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(MockRepository.GenerateMock<IPermission>());
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is("ScheduleOffering"), Arg<object[]>.Matches(args => PermissionArgIsStudent(args, TestData.Students[0])))).Return(MockRepository.GenerateMock<IPermission>());

            Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            Repositories.MockServiceAttendanceRepository.AssertWasCalled(m => m.Add(Arg<ServiceAttendance>.Matches(a => a.CreateTime.WithinTimeSpanOf(TimeSpan.FromSeconds(1), DateTime.Now) && a.CreatingUser == User.Identity.User)));
        }

        [TestMethod]
        public void GivenDurationIsInIncorrectFormat_WhenICallIsValidServiceOffering_ThenFalseIsReturned()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            row["DateAttended"] = "41390";
            row["Duration"] = "30a";
            FileData.Rows.Add(row);
            int expected = 1;
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(MockRepository.GenerateMock<IPermission>());
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is("ScheduleOffering"), Arg<object[]>.Matches(args => PermissionArgIsStudent(args, TestData.Students[0])))).Return(MockRepository.GenerateMock<IPermission>());

            ServiceUploadModel actual = Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            Assert.AreEqual(expected, actual.RowErrors.Count);
            Assert.AreEqual("Malformed Duration on row 4", actual.RowErrors[0]);
        }

        [TestMethod]
        public void GivenUserEntersValidSubject_WhenImport_ThenTheModelHasNoErrors()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "10";
            row["DateAttended"] = "41390";
            row["Subject"] = "Math";
            FileData.Rows.Add(row);
            int expected = 0;
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[1])).Return(MockRepository.GenerateMock<IPermission>());
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is("ScheduleOffering"), Arg<object[]>.Matches(args => PermissionArgIsStudent(args, TestData.Students[0])))).Return(MockRepository.GenerateMock<IPermission>());

            ServiceUploadModel actual = Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            Assert.AreEqual(expected, actual.RowErrors.Count);
        }

        [TestMethod]
        public void WhenImportServiceAttendances_ThenPermissionGrantAccessCalledForStudent()
        {
            DataRow row = FileData.NewRow();
            row["Id"] = "20";
            row["DateAttended"] = "41390";
            FileData.Rows.Add(row);
            FileData.Rows[1]["Id"] = "1";
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[0])).Return(MockRepository.GenerateMock<IPermission>());
            IPermission mockPermission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is("ScheduleOffering"), Arg<object[]>.Matches(args => PermissionArgIsStudent(args, TestData.Students[1])))).Return(mockPermission);

            Target.Import(User, ServiceAttendanceTemplatePath, FileData);

            mockPermission.AssertWasCalled(m => m.GrantAccess(User));
        }

        [TestMethod]
        public void GivenValidServiceOffering_WhenCreateAssignedOfferingTemplateDownload_ThenADownloadFileViewModelIsReturned()
        {
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", TestData.ServiceOfferings[0])).Return(MockRepository.GenerateMock<IPermission>());

            var model = Target.CreateTemplateDownload(User, ServiceAttendanceTemplatePath, 1) as DownloadFileModel;

            Assert.IsNotNull(model);
            DestroyTestFile(model.BlobAddress);
        }

        [TestMethod]
        public void GivenValidServiceOffering_WhenCreateAssignedOfferingTemplateDownload_ThenFileNameContainsServiceOfferingName()
        {
            ServiceOffering offering = TestData.ServiceOfferings[0];
            PermissionFactory.Current.Expect(m => m.Create("ImportOfferingData", offering)).Return(MockRepository.GenerateMock<IPermission>());

            var model = Target.CreateTemplateDownload(User, ServiceAttendanceTemplatePath, offering.Id) as DownloadFileModel;

            StringAssert.Contains(model.FileName, offering.Name.GetSafeFileName());
            DestroyTestFile(model.BlobAddress);
        }

        [TestMethod]
        public void GivenAnInvalidServiceOffering_WhenCreateAssignedOfferingTemplateDownload_ThenAModelIsNotReturned()
        {
            var model = Target.CreateTemplateDownload(User, ServiceAttendanceTemplatePath, 137836) as DownloadFileModel;

            Assert.IsNull(model);
        }

        [TestMethod]
        public void WhenIRetrieveUploadErrorFiles_ThenADownloadFileViewModelIsReturned()
        {
            var model = Target.RetrieveUploadErrorsFile(ServiceAttendanceTemplatePath) as DownloadFileModel;

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

        private static bool MatchProperties(ServiceAttendance actualState, StudentAssignedOffering expectedStudentAssignedOffering, DateTime expectedDateAttended, Subject expectedSubject, decimal expectedDuration, string expectedNotes)
        {
            Assert.AreEqual(expectedStudentAssignedOffering, actualState.StudentAssignedOffering);
            Assert.AreEqual(expectedDateAttended, actualState.DateAttended);
            Assert.AreEqual(expectedSubject.Id, actualState.Subject.Id);
            Assert.AreEqual(expectedDuration, actualState.Duration);
            Assert.AreEqual(expectedNotes, actualState.Notes);
            return true;
        }
    }
}