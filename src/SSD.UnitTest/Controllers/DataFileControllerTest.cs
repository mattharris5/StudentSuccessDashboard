using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.IO;
using SSD.ViewModels;
using System;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class DataFileControllerTest : BaseControllerTest
    {
        private ICustomFieldManager MockLogicManager { get; set; }
        private DataFileController Target { get; set; }
        private static readonly string UploadTemplateFolderPath = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "Templates");

        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<ICustomFieldManager>();
            Target = new DataFileController(MockLogicManager);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
            MockHttpContext.Expect(m => m.User).Return(User);
            MockHttpContext.Server.Expect(m => m.MapPath("../../App_Data/Uploads/")).Return(UploadTemplateFolderPath);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new DataFileController(null));
        }

        [TestMethod]
        public void WhenStudentProfileExport_ThenViewResultReturned()
        {
            ViewResult actual = Target.StudentProfileExport();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenModel_WhenGenerateStudentProfileExport_ThenFileResultReturned()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();
            string templatePath = Path.Combine(UploadTemplateFolderPath, DataFileController.TemplateFile);
            MockLogicManager.Expect(m => m.GenerateStudentProfileExport(User, model, templatePath)).Return(new MemoryStream());

            var result = Target.StudentProfileExport(model) as FileStreamResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenModel_WhenGenerateStudentProfileExport_ThenFileResultContainsExportStream()
        {
            Stream expected = new MemoryStream();
            StudentProfileExportModel model = new StudentProfileExportModel();
            string templatePath = Path.Combine(UploadTemplateFolderPath, DataFileController.TemplateFile);
            MockLogicManager.Expect(m => m.GenerateStudentProfileExport(User, model, templatePath)).Return(expected);

            var result = Target.StudentProfileExport(model) as FileStreamResult;

            Assert.AreEqual(expected, result.FileStream);
        }

        [TestMethod]
        public void GivenModel_WhenGenerateStudentProfileExport_ThenFileResultUsesValidContentType()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();
            string templatePath = Path.Combine(UploadTemplateFolderPath, DataFileController.TemplateFile);
            Stream expected = new MemoryStream();
            MockLogicManager.Expect(m => m.GenerateStudentProfileExport(User, model, templatePath)).Return(expected);

            var result = Target.StudentProfileExport(model) as FileStreamResult;

            Assert.AreEqual(ExcelWriter.ContentType, result.ContentType);
        }

        [TestMethod]
        public void GivenTooManyStudents_WhenGenerateStudentProfileExport_ThenPartialViewResultReturned()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();
            MockLogicManager.Expect(m => m.CheckStudentCount(model, 65000)).Throw(new ArgumentOutOfRangeException("blah"));

            var result = Target.StudentProfileExport(model) as PartialViewResult;

            Assert.IsNotNull(result);
        }
    }
}
