using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.Domain;
using SSD.IO;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class ServiceOfferingControllerTest : BaseControllerTest
    {
        private static readonly string UploadTemplateFolderPath = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "Templates");
        private IServiceOfferingManager MockLogicManager { get; set; }
        private IServiceTypeManager MockServiceTypeManager { get; set; }
        private IProviderManager MockProviderManager { get; set; }
        private IProgramManager MockProgramManager { get; set; }

        private ServiceOfferingController Target { get; set; }
        private IFileProcessor MockFileProcessor { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<IServiceOfferingManager>();
            MockServiceTypeManager = MockRepository.GenerateMock<IServiceTypeManager>();
            MockProviderManager = MockRepository.GenerateMock<IProviderManager>();
            MockProgramManager = MockRepository.GenerateMock<IProgramManager>();
            MockFileProcessor = MockRepository.GenerateMock<IFileProcessor>();
            Target = new ServiceOfferingController(MockLogicManager, MockServiceTypeManager, MockProviderManager, MockProgramManager, MockFileProcessor);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
            MockHttpContext.Expect(m => m.User).Return(User);
            MockHttpContext.Server.Expect(m => m.MapPath("../../App_Data/Uploads/")).Return(UploadTemplateFolderPath);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenIContruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceOfferingController(null, MockServiceTypeManager, MockProviderManager, MockProgramManager, MockFileProcessor));
        }

        [TestMethod]
        public void GivenNullServiceTypeManager_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceOfferingController(MockLogicManager, null, MockProviderManager, MockProgramManager, MockFileProcessor));
        }

        [TestMethod]
        public void GivenNullProviderManager_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceOfferingController(MockLogicManager, MockServiceTypeManager, null, MockProgramManager, MockFileProcessor));
        }

        [TestMethod]
        public void GivenNullProgramManager_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceOfferingController(MockLogicManager, MockServiceTypeManager, MockProviderManager, null, MockFileProcessor));
        }

        [TestMethod]
        public void GivenNullFileProcessor_WhenIContruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceOfferingController(MockLogicManager, MockServiceTypeManager, MockProviderManager, MockProgramManager, null));
        }

        [TestMethod]
        public void WhenManageIsCalled_ThenAViewIsReturned()
        {
            ViewResult result = Target.Index();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenViewModelIsCreated_WhenManageIsCalled_ThenViewModelMatchesCreated()
        {
            ServiceOfferingListOptionsModel expected = new ServiceOfferingListOptionsModel();
            MockLogicManager.Expect(m => m.GenerateListOptionsViewModel(User)).Return(expected);

            ViewResult result = Target.Index();

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenDataTableAjaxHandler_ThenJsonContainsGeneratedViewModel()
        {
            DataTableRequestModel model = new DataTableRequestModel();
            DataTableResultModel expected = new DataTableResultModel();
            MockLogicManager.Expect(m => m.GenerateDataTableResultViewModel(Arg.Is(model), Arg<IClientDataTable<ServiceOffering>>.Is.NotNull)).Return(expected);

            JsonResult result = Target.DataTableAjaxHandler(model);

            result.AssertGetData(expected);
        }

        [TestMethod]
        public void WhenSetFavorite_ThenSuccessJsonReturned()
        {
            JsonResult result = Target.SetFavorite(2437582, false) as JsonResult;

            result.AssertGetData(true);
        }

        [TestMethod]
        public void GivenLogicManagerYieldsFavorites_WhenGetFavorites_ThenViewResultContainsFavorites()
        {
            IEnumerable<ServiceOffering> expected = new[] { new ServiceOffering(), new ServiceOffering() };
            MockLogicManager.Expect(m => m.LoadFavorites(User)).Return(expected);

            PartialViewResult result = Target.Favorites();

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenFileUpload_ThenAViewIsReturned()
        {
            var result = Target.FileUpload() as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenIPostAFile_ThenAViewIsReturned()
        {
            var file = MockRepository.GenerateStub<UploadExcelFileModel>();
            file.File = MockRepository.GenerateStub<HttpPostedFileBase>();
            file.File.Expect(f => f.ContentType).Return("Blah");

            var result = Target.FileUpload(file) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenIPostAValidFile_ThenItIsSavedToTheServer()
        {
            var actual = ExecuteFileUploadPosted(1);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenAServiceIdForDownload_WhenIDownloadTemplate_ThenAFileStreamResultIsReturned()
        {
            string path = Path.Combine(UploadTemplateFolderPath, ServiceOfferingController.TemplateFile);
            MockFileProcessor.Expect(m => m.CreateTemplateDownload(User, path, 1)).Return(new DownloadFileModel { FileContentStream = new MemoryStream() });

            var result = Target.DownloadTemplate(1) as FileStreamResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenIDownloadUploadErrors_ThenAFileStreamResultIsReturned()
        {
            MockFileProcessor.Expect(m => m.RetrieveUploadErrorsFile("blah")).Return(new DownloadFileModel { BlobAddress = "blah", FileName = "blah", FileContentStream = new MemoryStream() });

            var result = Target.DownloadUploadErrors("blah") as FileStreamResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenAServiceIdForDownload_AndDownloadFolderPathDoesNotExist_WhenIDownloadTemplate_ThenAFileStreamResultIsReturned()
        {
            string folderThatDoesNotExist = Path.Combine(UploadTemplateFolderPath, "DoesNotExist");
            if (Directory.Exists(folderThatDoesNotExist))
            {
                Directory.Delete(folderThatDoesNotExist);
            }
            string path = Path.Combine(UploadTemplateFolderPath, ServiceOfferingController.TemplateFile);
            MockHttpContext.Server.Expect(m => m.MapPath("../../Content/Downloads")).Return(folderThatDoesNotExist);
            MockFileProcessor.Expect(m => m.CreateTemplateDownload(User, path, 1)).Return(new DownloadFileModel { FileContentStream = new MemoryStream() });

            var result = Target.DownloadTemplate(1) as FileStreamResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenAnInvalidServiceIdForDownload_WhenIDownloadTemplate_ThenAHttpNotFoundResultIsReturned()
        {
            var result = Target.DownloadTemplate(1345) as HttpNotFoundResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenServiceTypeNamesMatchTerm_WhenAutocompleteServiceTypeProviderOrProgram_ThenJsonResultContainsMatchingServiceTypeNames()
        {
            var expected = new[] { "Yellow", "YMCA" };
            MockServiceTypeManager.Expect(m => m.SearchNames("Y")).Return(expected);
            MockProviderManager.Expect(m => m.SearchProviderNames("Y")).Return(Enumerable.Empty<string>());
            MockProgramManager.Expect(m => m.SearchProgramNames("Y")).Return(Enumerable.Empty<string>());

            var result = Target.AutocompleteServiceTypeProviderOrProgram("Y") as JsonResult;

            IEnumerable<string> actual = result.AssertGetData<IEnumerable<string>>();
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        [TestMethod]
        public void GivenProviderNamesMatchTerm_WhenAutocompleteServiceTypeProviderOrProgram_ThenJsonResultContainsMatchingProviderNames()
        {
            var expected = new[] { "Yellow", "YMCA" };
            MockServiceTypeManager.Expect(m => m.SearchNames("Y")).Return(Enumerable.Empty<string>());
            MockProviderManager.Expect(m => m.SearchProviderNames("Y")).Return(expected);
            MockProgramManager.Expect(m => m.SearchProgramNames("Y")).Return(Enumerable.Empty<string>());

            var result = Target.AutocompleteServiceTypeProviderOrProgram("Y") as JsonResult;

            IEnumerable<string> actual = result.AssertGetData<IEnumerable<string>>();
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        [TestMethod]
        public void GivenProgramNamesMatchTerm_WhenAutocompleteServiceTypeProviderOrProgram_ThenJsonResultContainsMatchingProgramNames()
        {
            var expected = new[] { "Yellow", "YMCA" };
            MockServiceTypeManager.Expect(m => m.SearchNames("Y")).Return(Enumerable.Empty<string>());
            MockProviderManager.Expect(m => m.SearchProviderNames("Y")).Return(Enumerable.Empty<string>());
            MockProgramManager.Expect(m => m.SearchProgramNames("Y")).Return(expected);

            var result = Target.AutocompleteServiceTypeProviderOrProgram("Y") as JsonResult;

            IEnumerable<string> actual = result.AssertGetData<IEnumerable<string>>();
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        [TestMethod]
        public void GivenServiceTypeMatchesTerm_AndProviderMatchesTerm_AndProgramMatchesTerm_WhenAutocompleteServiceTypeProviderOrProgram_ThenJsonResultContainsMatchingServiceTypeNames_AndMatchingProviderNames_AndMatchingProgramNames()
        {
            var expectedServiceTypeMatches = new[] { "Your Service" };
            var expectedProviderMatches = new[] { "Yellow", "YMCA" };
            var expectedProgramMatches = new[] { "Playground Activities" };
            var expected = new[] { "Playground Activities", "Yellow", "YMCA", "Your Service" };
            MockServiceTypeManager.Expect(m => m.SearchNames("Y")).Return(expectedServiceTypeMatches);
            MockProviderManager.Expect(m => m.SearchProviderNames("Y")).Return(expectedProviderMatches);
            MockProgramManager.Expect(m => m.SearchProgramNames("Y")).Return(expectedProgramMatches);

            var result = Target.AutocompleteServiceTypeProviderOrProgram("Y") as JsonResult;

            IEnumerable<string> actual = result.AssertGetData<IEnumerable<string>>();
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        private ServiceUploadModel ExecuteFileUploadPosted(int mockedContentLength)
        {
            DataTable expectedTable = new DataTable();
            ServiceUploadModel expectedModel = new ServiceUploadModel();
            IServiceOfferingManager tempLogicManager = MockRepository.GenerateMock<IServiceOfferingManager>();
            ServiceOfferingController tempTarget = new ServiceOfferingController(tempLogicManager, MockServiceTypeManager, MockProviderManager, MockProgramManager, MockFileProcessor);
            tempTarget.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), tempTarget);
            var file = MockRepository.GenerateStub<UploadExcelFileModel>();
            file.File = MockRepository.GenerateStub<HttpPostedFileBase>();
            file.File.Expect(f => f.ContentLength).Return(mockedContentLength);
            file.File.Expect(f => f.ContentType).Return(ExcelWriter.ContentType);
            file.File.Expect(f => f.FileName).Return("HappyPath.xlsx");
            MockFileProcessor.Expect(m => m.ConsumeFile(file)).Return(expectedTable);
            MockFileProcessor.Expect(m => m.Import(User, Path.Combine(UploadTemplateFolderPath, ServiceOfferingController.TemplateFile), expectedTable)).Return(expectedModel);

            var result = tempTarget.FileUpload(file) as ViewResult;

            return result.AssertGetViewModel(expectedModel);
        }

        private ServiceUploadModel ExecuteFileUploadPostedFailure(int mockedContentLength)
        {
            IServiceOfferingManager tempLogicManager = MockRepository.GenerateMock<IServiceOfferingManager>();
            ServiceOfferingController tempTarget = new ServiceOfferingController(tempLogicManager, MockServiceTypeManager, MockProviderManager, MockProgramManager, MockFileProcessor);
            tempTarget.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), tempTarget);
            var file = MockRepository.GenerateStub<UploadExcelFileModel>();
            file.File = MockRepository.GenerateStub<HttpPostedFileBase>();
            file.File.Expect(f => f.ContentLength).Return(mockedContentLength);
            file.File.Expect(f => f.ContentType).Return(ExcelWriter.ContentType);
            file.File.Expect(f => f.FileName).Return("HappyPath.xlsx");

            var result = tempTarget.FileUpload(file) as ViewResult;

            MockFileProcessor.AssertWasNotCalled(m => m.ConsumeFile(file));
            MockFileProcessor.AssertWasNotCalled(m => m.Import(User, Path.Combine(UploadTemplateFolderPath, ServiceOfferingController.TemplateFile), null));
            return result.AssertGetViewModel<ServiceUploadModel>();
        }
    }
}
