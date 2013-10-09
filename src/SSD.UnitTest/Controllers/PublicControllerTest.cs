using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.Domain;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class PublicControllerTest : BaseControllerTest
    {
        private ICustomFieldManager MockLogicManager { get; set; }
        private PublicController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<ICustomFieldManager>();
            Target = new PublicController(MockLogicManager);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
            MockHttpContext.Expect(m => m.User).Return(User);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new PublicController(null));
        }

        [TestMethod]
        public void WhenIndexIsCalled_ThenAViewResultIsCreated()
        {
            var result = Target.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenLogicManagerCreatesViewModel_WhenCreate_ThenPartialViewContainsViewModel()
        {
            var expected = new PublicFieldModel();
            MockLogicManager.Expect(m => m.GenerateCreateViewModel()).Return(expected);

            PartialViewResult result = Target.Create();

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenEditPublicFieldButtonIsClicked_ThenAViewResultIsCreated()
        {
            MockLogicManager.Expect(m => m.GenerateEditViewModel(1, User)).Return(new PublicFieldModel());

            var result = Target.Edit(1) as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenModelStateIsValid_WhenIPostEdit_ThenAJsonResultIsReturned()
        {
            var result = Target.Edit(new PublicFieldModel()) as JsonResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenValidateThrowValidationException_WhenIPostEdit_ThenReturnCreateViewWithViewModel()
        {
            var viewModel = new PublicFieldModel();
            MockLogicManager.Expect(m => m.Validate(viewModel)).Throw(new ValidationException(new ValidationResult("blah", new[] { "FieldNameWithError" }), null, viewModel));

            var result = Target.Edit(viewModel) as PartialViewResult;

            result.AssertGetViewModel(viewModel);
        }

        [TestMethod]
        public void GivenValidateThrowValidationException_WhenIPostEdit_ThenModelStateErrorIncludesMemberNameAndMessage()
        {
            string expectedErrorMessage = "blah";
            string expectedFieldName = "FieldNameWithError";
            var viewModel = new PublicFieldModel();
            MockLogicManager.Expect(m => m.Validate(viewModel)).Throw(new ValidationException(new ValidationResult(expectedErrorMessage, new[] { expectedFieldName }), null, viewModel));

            Target.Edit(viewModel);

            Assert.IsTrue(Target.ModelState[expectedFieldName].Errors.Any(e => e.ErrorMessage == expectedErrorMessage));
        }

        [TestMethod]
        public void GivenModelIsInvalid_WhenIPostEdit_ThenItIsRepopulatedAndAPartialViewResultIsReturned()
        {
            Target.ModelState.AddModelError("blah", "blah");

            var result = Target.Edit(new PublicFieldModel()) as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenModelIsForPublicFieldWithPublicData_AndDataTypeIsChanged_WhenIPostEdit_ThenReturnEditView()
        {
            var expected = new PublicFieldModel();
            MockLogicManager.Expect(m => m.Edit(expected, User)).Throw(new ValidationException(new ValidationResult("Error!"), null, expected));

            PartialViewResult result = Target.Edit(expected) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenModelIsForPublicFieldWithPublicData_AndDataTypeIsChanged_WhenIPostEdit_ThenModelStateHasError()
        {
            string expectedErrorMessage = "blah";
            string expectedFieldName = "FieldNameWithError";
            var viewModel = new PublicFieldModel();
            MockLogicManager.Expect(m => m.Edit(viewModel, User)).Throw(new ValidationException(new ValidationResult(expectedErrorMessage, new[] { expectedFieldName }), null, viewModel));

            Target.Edit(viewModel);

            Assert.IsTrue(Target.ModelState[expectedFieldName].Errors.Any(e => e.ErrorMessage == expectedErrorMessage));
        }

        [TestMethod]
        public void GivenModelIsForPublicFieldWithPublicData_AndDataTypeIsChanged_WhenIPostEdit_ThenViewModelPopulated()
        {
            var expected = new PublicFieldModel();
            MockLogicManager.Expect(m => m.Edit(expected, User)).Throw(new ValidationException(new ValidationResult("Error!"), null, expected));

            Target.Edit(expected);

            MockLogicManager.AssertWasCalled(m => m.PopulateViewModel(expected));
        }

        [TestMethod]
        public void GivenAPublicField_WhenGettingTableData_ThenActionColumnDataContainsIdAndName()
        {
            var request = new DataTableRequestModel();
            var expected = new DataTableResultModel();
            MockLogicManager.Expect(m => m.GenerateDataTableResultViewModel(Arg.Is(request), Arg<IClientDataTable<CustomField>>.Is.NotNull)).Return(expected);

            var result = Target.DataTableAjaxHandler(request) as JsonResult;

            result.AssertGetData(expected);
        }

        [TestMethod]
        public void GivenAnInvalidViewModel_WhenIPostCreate_ThenAPartialViewResultIsReturned()
        {
            Target.ModelState.AddModelError("blah", "blah");

            var result = Target.Create(new PublicFieldModel()) as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenIPostCreate_ThenAJsonResultIsReturned()
        {
            var result = Target.Create(new PublicFieldModel()) as JsonResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenValidateThrowValidationException_WhenIPostCreate_ThenReturnCreateViewWithViewModel()
        {
            var viewModel = new PublicFieldModel();
            MockLogicManager.Expect(m => m.Validate(viewModel)).Throw(new ValidationException(new ValidationResult("blah", new[] { "FieldNameWithError" }), null, viewModel));

            var result = Target.Create(viewModel) as PartialViewResult;

            result.AssertGetViewModel(viewModel);
        }

        [TestMethod]
        public void GivenValidateThrowValidationException_WhenIPostCreate_ThenModelStateErrorIncludesMemberNameAndMessage()
        {
            string expectedErrorMessage = "blah";
            string expectedFieldName = "FieldNameWithError";
            var viewModel = new PublicFieldModel();
            MockLogicManager.Expect(m => m.Validate(viewModel)).Throw(new ValidationException(new ValidationResult(expectedErrorMessage, new[] { expectedFieldName }), null, viewModel));

            Target.Create(viewModel);

            Assert.IsTrue(Target.ModelState[expectedFieldName].Errors.Any(e => e.ErrorMessage == expectedErrorMessage));
        }

        [TestMethod]
        public void WhenICallDelete_ThenAPartialViewResultIsReturned()
        {
            MockLogicManager.Expect(m => m.GenerateDeleteViewModel(1)).Return(new PublicFieldModel());

            var result = Target.Delete(1) as PartialViewResult;

            result.AssertGetViewModel<CustomFieldModel>();
        }

        [TestMethod]
        public void WhenIPostDelete_ThenAJsonResultIsReturned()
        {
            var result = Target.DeleteConfirmed(1) as JsonResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenValidationExceptionThrown_WhenIPostDelete_ThenReturnDeleteView()
        {
            CustomFieldModel expected = new PublicFieldModel();
            MockLogicManager.Expect(m => m.Delete(1)).Throw(new ValidationException(new ValidationResult("blah"), null, 1));
            MockLogicManager.Expect(m => m.GenerateDeleteViewModel(1)).Return(expected);

            var result = Target.DeleteConfirmed(1) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenValidationExceptionThrown_WhenIPostDelete_ThenModelStateContainsError()
        {
            CustomFieldModel expected = new PublicFieldModel();
            MockLogicManager.Expect(m => m.Delete(1)).Throw(new ValidationException(new ValidationResult("blah"), null, 1));
            MockLogicManager.Expect(m => m.GenerateDeleteViewModel(1)).Return(expected);

            Target.DeleteConfirmed(1);

            Assert.AreEqual("blah", Target.ModelState[string.Empty].Errors.Single().ErrorMessage);
        }

        [TestMethod]
        public void WhenUploadWizardIsCalled_ThenAViewResultIsCreated()
        {
            var result = Target.UploadWizard() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenUploadWizardConfirmed_ThenAViewResultIsCreated()
        {
            var result = Target.UploadWizardConfirmed(new UploadWizardFileViewModel()) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenUploadWizard2IsCalled_ThenAViewResultIsCreated()
        {
            var result = Target.UploadWizard2(new UploadWizardModel(), string.Empty) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenPreviousButtonWasClicked_WhenIPost_ThenTheAppropriateViewIsReturned()
        {
            var result = Target.UploadWizard2(new UploadWizardModel(), "previous") as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName.Equals("UploadWizard"));
        }

        [TestMethod]
        public void GivenSubmitButtonWasClickedAndNoStudentIdWasSelected_WhenIPost_ThenTheAppropriateModelStateErrorsAreReturned()
        {
            var result = Target.UploadWizard2(new UploadWizardModel(), "submit") as ViewResult;
            ModelState errorState = Target.ModelState.Where(e => e.Key == "StudentId").Select(e => e.Value).SingleOrDefault();

            Assert.IsNotNull(result);
            Assert.IsNotNull(errorState);
            Assert.IsTrue(errorState.Errors.Any());
        }

        [TestMethod]
        public void GivenSubmitButtonWasClicked_WhenIPost_ThenTheAppropriateViewIsReturned()
        {
            var model = new UploadWizardModel
            {
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    }
                }
            };
            var result = Target.UploadWizard2(model, "submit") as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName.Equals("UploadWizard3"));
        }

        [TestMethod]
        public void GivenInvalidModelState_WhenIPostUploadWizard_ThenAViewIsReturnedContainingTheInvalidModelState()
        {
            Target.ModelState.AddModelError("blah", "blah");
            var result = Target.UploadWizardConfirmed(new UploadWizardFileViewModel()) as ViewResult;

            Assert.IsTrue(result.ViewName.Equals(string.Empty));
            Assert.IsFalse(Target.ModelState.IsValid);
        }

        [TestMethod]
        public void GivenAnInvalidBlobAddress_WhenIDownloadUploadErrors_ThenAnHttpNotFoundResultIsReturned()
        {
            MockLogicManager.Expect(m => m.RetrieveUploadErrorsFile("blah")).Return(null);

            var result = Target.DownloadUploadErrors("blah") as HttpNotFoundResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenIDownloadUploadErrors_ThenFileStreamResultIsReturned()
        {
            MockLogicManager.Expect(m => m.RetrieveUploadErrorsFile("blah")).Return(new DownloadFileModel
            {
                BlobAddress = "blah",
                FileName = "blah.txt",
                FileContentStream = new MemoryStream()
            });

            var result = Target.DownloadUploadErrors("blah") as FileStreamResult;

            Assert.IsNotNull(result);
        }
    }
}
