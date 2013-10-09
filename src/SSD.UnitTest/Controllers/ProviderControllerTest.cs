using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.Domain;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class ProviderControllerTest : BaseControllerTest
    {
        private IProviderManager MockLogicManager { get; set; }
        private ProviderController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<IProviderManager>();
            Target = new ProviderController(MockLogicManager);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
            MockHttpContext.Expect(m => m.User).Return(User);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ProviderController(null));
        }

        [TestMethod]
        public void WhenCreateProviderButtonIsClicked_ThenAViewResultIsCreated()
        {
            var result = Target.Create() as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenViewModelHasInvalidState_WhenCreatePosted_ThenViewResultContainsValidationErrorMessage()
        {
            Target.ModelState.AddModelError("Name", new ValidationException());
            var model = new ProviderModel();

            var result = Target.Create(model) as PartialViewResult;

            result.AssertGetViewModel(model);
        }

        [TestMethod]
        public void GivenViewModelHasInvalidState_WhenCreatePosted_ThenViewModelContainsSchoolList()
        {
            Target.ModelState.AddModelError("Name", new ValidationException());
            var expected = new ProviderModel();

            var result = Target.Create(expected) as PartialViewResult;

            result.AssertGetViewModel<ProviderModel>(expected);
            MockLogicManager.AssertWasCalled(m => m.PopulateViewModel(expected));
        }

        [TestMethod]
        public void GivenLogicManagerThrowsValidationException_WhenCreatePosted_ThenModelStateHasErrors_AndViewModelIsPopulated()
        {
            var expected = new ProviderModel { Name = "YMCA" };
            MockLogicManager.Expect(m => m.Validate(expected)).Throw(new ValidationException(new ValidationResult("Error!", new[] { "Name" }), null, expected));

            var result = Target.Create(expected) as PartialViewResult;

            result.AssertGetViewModel<ProviderModel>(expected);
            MockLogicManager.AssertWasCalled(m => m.PopulateViewModel(expected));
            Assert.IsTrue(Target.ModelState["Name"].Errors.Count > 0);
        }

        [TestMethod]
        public void WhenGetCreateResult_ThenViewModelIsPopulated()
        {
            var result = Target.Create() as PartialViewResult;

            ProviderModel actual = result.AssertGetViewModel<ProviderModel>();
            MockLogicManager.AssertWasCalled(m => m.PopulateViewModel(actual));
        }

        [TestMethod]
        public void WhenANewlyCreatedProviderIsSubmitted_ReturnAJsonResult()
        {
            var result = Target.Create(new ProviderModel { Id = 2 }) as JsonResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenANewViewModel_WhenCreate_ThenReturnPassingJsonResult_AndLogicManagerCreates()
        {
            var viewModel = new ProviderModel();

            var result = Target.Create(viewModel) as JsonResult;

            result.AssertGetData(true);
            MockLogicManager.AssertWasCalled(m => m.Create(User, viewModel));
        }

        [TestMethod]
        public void WhenEditProviderButtonIsClicked_ThenAViewResultIsCreated()
        {
            MockLogicManager.Expect(m => m.GenerateEditViewModel(User, 1)).Return(new ProviderModel());

            var result = Target.Edit(1) as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenValidProviderId_WhenGetEditResult_ThenViewModelReturned_AndViewModelIsPopulated()
        {
            var expected = new ProviderModel();
            MockLogicManager.Expect(m => m.GenerateEditViewModel(User, 1)).Return(expected);

            var result = Target.Edit(1) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenProviderViewModel_WhenEditProviderIsSubmitted_ThenReturnPassingJsonResult_AndLogicManagerEditsViewModel()
        {
            var expected = new ProviderModel { Id = 1, Name = "YMCA" };

            var result = Target.Edit(expected) as JsonResult;

            result.AssertGetData(true);
            MockLogicManager.AssertWasCalled(m => m.Edit(User, expected));
        }

        [TestMethod]
        public void GivenLogicManagerThrowsValidationException_WhenEditPosted_ReturnViewModel_AndModelStateContainsErrors()
        {
            var expected = new ProviderModel { Id = 1, Name = "Jimbo's Math Shop" };
            MockLogicManager.Expect(m => m.Edit(User, expected)).Throw(new ValidationException(new ValidationResult("Error!", new[] { "Name" }), null, expected));

            var result = Target.Edit(expected) as PartialViewResult;
            
            result.AssertGetViewModel(expected);
            Assert.IsTrue(Target.ModelState["Name"].Errors.Count > 0);
        }

        [TestMethod]
        public void WhenDeleteProviderIsClicked_ThenAPartialViewResultIsCreated()
        {
            MockLogicManager.Expect(m => m.GenerateDeleteViewModel(1)).Return(new ProviderModel());

            var result = Target.Delete(1) as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenValidProviderId_WhenDeleteProviderIsClicked_ThenAPartialViewResultOfTheCorrectProviderIsReturned()
        {
            var expected = new ProviderModel();
            MockLogicManager.Expect(m => m.GenerateDeleteViewModel(1)).Return(expected);

            var result = Target.Delete(1) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenProviderToDeleteIsSubmitted_ThenItIsDeletedFromTheRepository()
        {
            Target.DeleteConfirmed(1);

            MockLogicManager.AssertWasCalled(m => m.Delete(1));
        }

        [TestMethod]
        public void WhenProviderIsDeleted_JsonResultContainsTrue()
        {
            var expected = new Provider { Id = 1, Name = "Bob" };

            var result = Target.DeleteConfirmed(expected.Id) as JsonResult;

            Assert.AreEqual(true, result.Data);
        }

        [TestMethod]
        public void GivenProviderAssociatedWithStudentAssignedOfferings_WhenDeleteConfirmed_ThenPartialViewReturned()
        {
            MockLogicManager.Expect(m => m.Delete(1)).Throw(new ValidationException());
            ProviderModel model = new ProviderModel();
            MockLogicManager.Expect(m => m.GenerateDeleteViewModel(1)).Return(model);

            var actual = Target.DeleteConfirmed(1) as PartialViewResult;

            Assert.IsNotNull(actual);
            actual.AssertGetViewModel(model);
        }

        [TestMethod]
        public void GivenAProvider_WhenGettingTableData_ThenJsonResultContainsViewModelFromLogicManager()
        {
            var request = new DataTableRequestModel();
            var expected = new DataTableResultModel();
            MockLogicManager.Expect(m => m.GenerateDataTableResultViewModel(Arg.Is(request), Arg<IClientDataTable<Provider>>.Is.NotNull)).Return(expected);

            var result = Target.DataTableAjaxHandler(request) as JsonResult;

            result.AssertGetData(expected);
        }

        [TestMethod]
        public void GivenModelStateIsInvalid_WhenIPostEdit_ThenAPartialViewIsReturned()
        {
            Target.ModelState.AddModelError("blah", "blahhhhh");
            var result = Target.Edit(new ProviderModel{ }) as PartialViewResult;

            Assert.IsNotNull(result);
        }
    }
}
