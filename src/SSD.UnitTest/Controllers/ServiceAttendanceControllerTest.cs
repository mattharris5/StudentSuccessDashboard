using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.IO;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.ComponentModel.DataAnnotations;
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
    public class ServiceAttendanceControllerTest : BaseControllerTest
    {
        private static readonly string UploadTemplateFolderPath = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "Templates");
        
        private IServiceAttendanceManager MockLogicManager { get; set; }
        private IFileProcessor MockFileProcessor { get; set; }
        private ServiceAttendanceController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<IServiceAttendanceManager>();
            MockFileProcessor = MockRepository.GenerateMock<IFileProcessor>();
            Target = new ServiceAttendanceController(MockLogicManager, MockFileProcessor);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
            MockHttpContext.Expect(m => m.User).Return(User);
            MockHttpContext.Server.Expect(m => m.MapPath("../../App_Data/Uploads/")).Return(UploadTemplateFolderPath);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenIContruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceAttendanceController(null, MockFileProcessor));
        }

        [TestMethod]
        public void GivenNullFileProcessor_WhenIContruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceAttendanceController(MockLogicManager, null));
        }

        [TestMethod]
        public void WhenIndexIsCalled_ThenActionResultIsReturned()
        {
            var result = Target.Index(1) as ActionResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenImAuthorized_WhenIndexIsCalled_ThenActionResultIsReturned()
        {
            var result = Target.Index(1);

            Assert.IsInstanceOfType(result, typeof(ActionResult));
        }

        [TestMethod]
        public void GivenValidModelWithId_WhenDataTableAjaxHandler_ThenJSonResultReturned()
        {
            HttpRequestBase MockRequest = MockRepository.GenerateMock<HttpRequestBase>();
            MockRequest.Expect(m => m["id"]).Return("1");
            DataTableRequestModel model = new DataTableRequestModel();
            MockHttpContext.Expect(c => c.Request).Return(MockRequest).Repeat.Any();

            var result = Target.DataTableAjaxHandler(model) as JsonResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenCreateIsCalled_ThenAPartialViewIsReturned()
        {
            var result = Target.Create(1) as ActionResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenCreatePosts_ThenAJsonResultIsReturned()
        {
            var model = new ServiceAttendanceModel { StudentAssignedOfferingId = 1, DateAttended = DateTime.Now };

            var result = Target.Create(model) as JsonResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenCreatePosts_ThenJsonResultContainsTrue()
        {
            var model = new ServiceAttendanceModel { StudentAssignedOfferingId = 1, DateAttended = DateTime.Now };

            var result = Target.Create(model) as JsonResult;

            result.AssertGetData(true);
        }

        [TestMethod]
        public void WhenCreatePosts_ThenLogicManagerCreates()
        {
            var model = new ServiceAttendanceModel { StudentAssignedOfferingId = 1, DateAttended = DateTime.Now };

            Target.Create(model);

            MockLogicManager.AssertWasCalled(m => m.Create(model, User));
        }

        [TestMethod]
        public void GivenInvalidModelState_WhenCreatePosts_ThenAPartialViewIsReturned()
        {
            Target.ModelState.AddModelError("key", "error");
            var model = new ServiceAttendanceModel { StudentAssignedOfferingId = 1, DateAttended = DateTime.Now };

            var result = Target.Create(model) as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenInvalidModelState_WhenCreatePosts_ThenPartialViewHasExpectedViewModel()
        {
            Target.ModelState.AddModelError("key", "error");
            var expected = new ServiceAttendanceModel { StudentAssignedOfferingId = 1, DateAttended = DateTime.Now };

            var result = Target.Create(expected) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenEditIsCalled_ThenAPartialViewIsReturned()
        {
            ServiceAttendanceModel expected = new ServiceAttendanceModel();
            MockLogicManager.Expect(m => m.GenerateEditViewModel(User, 1)).Return(expected);

            var result = Target.Edit(1) as PartialViewResult;

            ServiceAttendanceModel actual = result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenEditPosts_ThenAJsonResultIsReturned()
        {
            var result = Target.Edit(new ServiceAttendanceModel { Id = 1, SelectedSubjectId = 1 }) as JsonResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenViewModelHasInvalidState_WhenEditPosts_ThenAPartialViewIsReturned()
        {
            Target.ModelState.AddModelError("test", "test");

            var result = Target.Edit(new ServiceAttendanceModel { }) as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenLogicManagerThrowsValidationException_WhenEditPosts_ThenAPartialViewResultIsReturned()
        {
            ServiceAttendanceModel viewModel = new ServiceAttendanceModel { Id = 1, SelectedSubjectId = 1 };
            MockLogicManager.Expect(m => m.Edit(viewModel, User)).Throw(new ValidationException());

            var result = Target.Edit(viewModel) as PartialViewResult;

            result.AssertGetViewModel(viewModel);
        }

        [TestMethod]
        public void WhenDelete_ThenAPartialViewResultIsCreated()
        {
            MockLogicManager.Expect(m => m.GenerateEditViewModel(User, 1)).Return(new ServiceAttendanceModel());

            var result = Target.Delete(1) as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenValidServiceAttendanceId_WhenDelete_ThenAPartialViewResultOfTheCorrectServiceAttendanceIsReturned()
        {
            var expected = new ServiceAttendanceModel();
            MockLogicManager.Expect(m => m.GenerateDeleteViewModel(User, 1)).Return(expected);

            var result = Target.Delete(1) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenConfirmDelete_ThenDeleteIsCalled()
        {
            Target.ConfirmDelete(1);

            MockLogicManager.AssertWasCalled(m => m.Delete(1, User));
        }

        [TestMethod]
        public void WhenConfirmDelete_JsonResultContainsTrue()
        {
            var result = Target.ConfirmDelete(1) as JsonResult;

            Assert.AreEqual(true, result.Data);
        }

        [TestMethod]
        public void GivenAServiceIdForDownloadServiceAttendance_AndDownloadFolderPathDoesNotExist_WhenIDownloadTemplate_ThenAFileStreamResultIsReturned()
        {
            string folderThatDoesNotExist = Path.Combine(UploadTemplateFolderPath, "DoesNotExist");
            if (Directory.Exists(folderThatDoesNotExist))
            {
                Directory.Delete(folderThatDoesNotExist);
            }
            string path = Path.Combine(UploadTemplateFolderPath, ServiceAttendanceController.TemplateFile);
            MockHttpContext.Server.Expect(m => m.MapPath("../../Content/Downloads")).Return(folderThatDoesNotExist);
            MockFileProcessor.Expect(m => m.CreateTemplateDownload(User, path, 1)).Return(new DownloadFileModel { FileContentStream = new MemoryStream() });

            var result = Target.DownloadTemplate(1) as FileStreamResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenAnInvalidServiceIdForDownloadServiceAttendance_WhenIDownloadTemplate_ThenAHttpNotFoundResultIsReturned()
        {

            var result = Target.DownloadTemplate(0) as HttpNotFoundResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenModelStateIsNotValid_WhenIUploadFile_ThenAViewStatingTheModelErrorsIsReturned()
        {
            var file = MockRepository.GenerateStub<UploadExcelFileModel>();
            Target.ModelState.AddModelError("blah", "blah message");

            var result = Target.FileUpload(file) as ViewResult;

            var model = result.AssertGetViewModel<ServiceUploadModel>();
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.IsTrue(model.RowErrors.Any());
        }

        [TestMethod]
        public void GivenImAuthorizedAndViewModelIsValid_WhenIUploadFile_ThenAViewResultIsReturned()
        {
            var path = Path.Combine(UploadTemplateFolderPath, ServiceAttendanceController.TemplateFile);
            var file = MockRepository.GenerateStub<UploadExcelFileModel>();
            file.File = MockRepository.GenerateStub<HttpPostedFileBase>();
            file.File.Expect(f => f.ContentLength).Return(1);

            var result = Target.FileUpload(file) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenUploadFile_ThenFileProcessorConsumesFile()
        {
            var path = Path.Combine(UploadTemplateFolderPath, ServiceAttendanceController.TemplateFile);
            var file = MockRepository.GenerateStub<UploadExcelFileModel>();
            file.File = MockRepository.GenerateStub<HttpPostedFileBase>();
            file.File.Expect(f => f.ContentLength).Return(1);

            Target.FileUpload(file);

            MockFileProcessor.AssertWasCalled(m => m.ConsumeFile(file));
        }

        [TestMethod]
        public void WhenUploadFile_ThenFileProcessorConsumesFile_AndResultingDataTableIsImported()
        {
            DataTable expectedTable = new DataTable();
            var path = Path.Combine(UploadTemplateFolderPath, ServiceAttendanceController.TemplateFile);
            var file = MockRepository.GenerateStub<UploadExcelFileModel>();
            file.File = MockRepository.GenerateStub<HttpPostedFileBase>();
            file.File.Expect(f => f.ContentLength).Return(1);
            MockFileProcessor.Expect(m => m.ConsumeFile(file)).Return(expectedTable);

            Target.FileUpload(file);

            MockFileProcessor.AssertWasCalled(m => m.Import(User, path, expectedTable));
        }

        [TestMethod]
        public void WhenUploadFile_ThenFileProcessorConsumesFile_AndResultingDataTableIsImported_AndResultingModelReturnedInViewResult()
        {
            ServiceUploadModel expected = new ServiceUploadModel();
            DataTable expectedTable = new DataTable();
            var path = Path.Combine(UploadTemplateFolderPath, ServiceAttendanceController.TemplateFile);
            var file = MockRepository.GenerateStub<UploadExcelFileModel>();
            file.File = MockRepository.GenerateStub<HttpPostedFileBase>();
            file.File.Expect(f => f.ContentLength).Return(1);
            MockFileProcessor.Expect(m => m.ConsumeFile(file)).Return(expectedTable);
            MockFileProcessor.Expect(m => m.Import(User, path, expectedTable)).Return(expected); ;

            ViewResult actual = Target.FileUpload(file) as ViewResult;

            actual.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenICallFileUpload_ThenPartialViewIsReturned()
        {
            var actual = Target.FileUpload() as PartialViewResult;

            Assert.IsNotNull(actual);
        }
    }
}
