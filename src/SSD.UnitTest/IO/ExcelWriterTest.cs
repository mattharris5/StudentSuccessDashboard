using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Controllers;
using SSD.Domain;
using System;
using System.Configuration;
using System.IO;

namespace SSD.IO
{
    [TestClass]
    public class ExcelWriterTest
    {
        private static string UploadFileTemplatePath = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"]);

        private static string ServiceOfferingSheetName = "Assign Service Offering";
        private TestData TestData { get; set; }
        private string AssignedServiceOfferingTemplatePath { get; set; }
        private ExcelWriter Target { get; set; }
        
        [TestInitialize]
        public void InitializeTest()
        {
            TestData = new TestData();
            AssignedServiceOfferingTemplatePath = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "Templates/" + ServiceOfferingController.TemplateFile);
            Target = new ExcelWriter();
        }

        [TestMethod]
        public void GivenNullWorksheetWriter_WhenInitializeFrom_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.InitializeFrom("", null));
        }

        [TestMethod]
        public void GivenNullBlobContainer_WhenWrite_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => Target.Write(null, string.Empty));
        }
        
        [TestMethod]
        public void GivenAServiceOffering_AndInitializeFromTemplate_WhenWrite_ThenItIsSavedForTheUserToDownload()
        {
            var offering = TestData.ServiceOfferings[0];
            var fileName = string.Format("{0}-{1}-{2}{3}", offering.Provider.Name, offering.ServiceType.Name, DateTime.Now.Ticks, ".xlsx");
            var downloadfilePath = string.Format("{0}{1}",
                AssignedServiceOfferingTemplatePath.Replace("\\App_Data\\Uploads\\Templates\\" + ServiceOfferingController.TemplateFile, "\\Content\\Downloads\\"), fileName);
            var writer = new WorksheetWriter(offering, ServiceOfferingSheetName);
            IBlobContainer mockBlobContainer = CreateMockBlobContainer();
            Target.InitializeFrom(AssignedServiceOfferingTemplatePath, writer);

            Target.Write(mockBlobContainer, downloadfilePath);

            Assert.IsTrue(File.Exists(downloadfilePath));
            File.Delete(downloadfilePath);
        }

        [TestMethod]
        public void GivenAServiceOffering_AndInitializeFromTemplate_WhenWrite_ThenFileSavedWithExpectedContentType()
        {
            string expectedContentType = ExcelWriter.ContentType;
            var offering = TestData.ServiceOfferings[0];
            var fileName = string.Format("{0}-{1}-{2}{3}", offering.Provider.Name, offering.ServiceType.Name, DateTime.Now.Ticks, ".xlsx");
            var downloadfilePath = string.Format("{0}{1}", AssignedServiceOfferingTemplatePath.Replace("\\App_Data\\Uploads\\Templates\\" + ServiceOfferingController.TemplateFile, "\\Content\\Downloads\\"), fileName);
            WorksheetWriter writer = new WorksheetWriter(offering, ServiceOfferingSheetName);
            IBlobContainer mockBlobContainer = MockRepository.GenerateMock<IBlobContainer>();
            Target.InitializeFrom(AssignedServiceOfferingTemplatePath, writer);

            Target.Write(mockBlobContainer, downloadfilePath);

            mockBlobContainer.AssertWasCalled(m => m.UploadFromStream(downloadfilePath, Target.FileContentStream, expectedContentType));
        }

        [TestMethod]
        public void GivenNullWorksheetWriter_WhenAppendErrorRows_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.AppendErrorRows("doesn't matter", null));
        }

        [TestMethod]
        public void GivenNotInitialized_WhenAppendErrorRows_ThenThrowException()
        {
            var offering = TestData.ServiceOfferings[0];
            var writer = new WorksheetWriter(offering, null);

            Target.ExpectException<InvalidOperationException>(() => Target.AppendErrorRows(null, writer));
        }

        [TestMethod]
        public void GivenANullSheetName_AndInitializeFromFile_WhenAppendErrorRows_AndWrite_ThenTheFirstSheetIsUsed()
        {
            var filePath = UploadFileTemplatePath + "NullStartEndDateAndNotesCol.xlsx";
            var destinationPath = CopyTestFile(filePath);
            var offering = TestData.ServiceOfferings[0];
            var writer = new WorksheetWriter(offering, null);
            IBlobContainer mockBlobContainer = CreateMockBlobContainer();

            Target.InitializeFrom(destinationPath);
            Target.AppendErrorRows(null, writer);
            Target.Write(mockBlobContainer, destinationPath);

            Assert.IsNotNull(File.Exists(destinationPath));
            DestroyTestFile(destinationPath);
        }

        [TestMethod]
        public void GivenNotInitialized_WhenWrite_ThenThrowException()
        {
            IBlobContainer mockBlobContainer = CreateMockBlobContainer();

            Target.ExpectException<InvalidOperationException>(() => Target.Write(mockBlobContainer, "doesn't matter"));
        }

        private string CopyTestFile(string currentPath)
        {
            var destinationPath = currentPath.Replace(".xlsx", "-test.xlsx");
            File.Copy(currentPath, destinationPath, true);
            return destinationPath;
        }

        private void DestroyTestFile(string testPath)
        {
            File.Delete(testPath);
        }

        private static IBlobContainer CreateMockBlobContainer()
        {
            IBlobContainer mockBlobContainer = MockRepository.GenerateMock<IBlobContainer>();
            mockBlobContainer.Expect(m => m.DownloadToStream(null, null)).IgnoreArguments().Do(new Action<string, Stream>((address, target) =>
            {
                byte[] byteArray = File.ReadAllBytes(address);
                target.Write(byteArray, 0, (int)byteArray.Length);
            }));
            mockBlobContainer.Expect(m => m.UploadFromStream(null, null, null)).IgnoreArguments().Do(new Action<string, Stream, string>((address, source, contentType) =>
            {
                Directory.CreateDirectory(Path.GetDirectoryName(address));
                File.WriteAllBytes(address, ((MemoryStream)source).ToArray());
            }));
            return mockBlobContainer;
        }
    }
}
