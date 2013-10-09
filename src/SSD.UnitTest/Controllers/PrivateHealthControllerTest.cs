using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.Domain;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class PrivateHealthControllerTest : BaseControllerTest
    {
        private ICustomFieldManager MockLogicManager { get; set; }
        private PrivateHealthController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<ICustomFieldManager>();
            Target = new PrivateHealthController(MockLogicManager);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
            MockHttpContext.Expect(m => m.User).Return(User);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new PrivateHealthController(null));
        }

        [TestMethod]
        public void WhenIndex_ThenGetViewResult()
        {
            ViewResult actual = Target.Index();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void WhenDataTableAjaxHandler_ThenReturnJsonResult()
        {
            DataTableRequestModel requestModel = new DataTableRequestModel();

            JsonResult actual = Target.DataTableAjaxHandler(requestModel);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenLogicManagerGeneratesViewModel_WhenDataTableAjaxHandler_ThenJsonResultContainsViewModel()
        {
            DataTableResultModel expected = new DataTableResultModel();
            DataTableRequestModel requestModel = new DataTableRequestModel();
            MockLogicManager.Expect(m => m.GenerateDataTableResultViewModel(Arg.Is(requestModel), Arg<IClientDataTable<CustomField>>.Is.NotNull)).Return(expected);

            JsonResult result = Target.DataTableAjaxHandler(requestModel);

            result.AssertGetData(expected);
        }

        [TestMethod]
        public void WhenUploadWizardIsCalled_ThenAViewResultIsReturned()
        {
            var result = Target.UploadWizard() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenNoModelStateErrors_WhenUploadWizardConfirmed_ThenAViewResultIsReturned()
        {
            UploadWizardFileViewModel model = new UploadWizardFileViewModel();
            MockLogicManager.Expect(m => m.GenerateMapFieldsViewModel(model, typeof(PrivateHealthField), User)).Return(new UploadWizardModel());

            var result = Target.UploadWizardConfirmed(model) as ViewResult;

            Assert.IsNotNull(result);
            result.AssertGetViewModel<UploadWizardModel>();
        }

        [TestMethod]
        public void GivenModelStateErrors_WhenUploadWizardConfirmed_ThenViewResultWithTheFirstModelReturned()
        {
            UploadWizardFileViewModel model = new UploadWizardFileViewModel();
            Target.ModelState.AddModelError("blah", "blerg");

            var result = Target.UploadWizardConfirmed(new UploadWizardFileViewModel()) as ViewResult;

            result.AssertGetViewModel<UploadWizardFileViewModel>();
        }

        [TestMethod]
        public void WhenUploadWizard2IsCalled_ThenAViewResultIsCreated()
        {
            var result = Target.UploadWizard2(new UploadWizardModel(), string.Empty) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenPreviousButtonWasClicked_WhenUploadWizard2_ThenTheAppropriateViewIsReturned()
        {
            var result = Target.UploadWizard2(new UploadWizardModel(), "previous") as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName.Equals("UploadWizard"));
        }

        [TestMethod]
        public void GivenSubmitButtonWasClickedAndNoStudentIdWasSelected_WhenUploadWizard2_ThenTheAppropriateModelStateErrorsAreReturned()
        {
            var result = Target.UploadWizard2(new UploadWizardModel(), "submit") as ViewResult;
            ModelState errorState = Target.ModelState.Where(e => e.Key == "StudentId").Select(e => e.Value).SingleOrDefault();

            Assert.IsNotNull(result);
            Assert.IsNotNull(errorState);
            Assert.IsTrue(errorState.Errors.Any());
        }

        [TestMethod]
        public void GivenSubmitButtonWasClicked_WhenUploadWizard2_ThenTheAppropriateViewIsReturned()
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
            MockLogicManager.Expect(m => m.GenerateUploadWizardCompleteViewModel(User, model)).Return(new UploadWizardCompleteModel());
            var result = Target.UploadWizard2(model, "submit") as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName.Equals("UploadWizard3"));
            result.AssertGetViewModel<UploadWizardCompleteModel>();
        }
    }
}
