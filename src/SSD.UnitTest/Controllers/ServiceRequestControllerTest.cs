using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class ServiceRequestControllerTest : BaseControllerTest
    {
        private IServiceRequestManager MockLogicManager { get; set; }
        private ServiceRequestController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<IServiceRequestManager>();
            Target = new ServiceRequestController(MockLogicManager);
            MockHttpContext.Expect(m => m.User).Return(User);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceRequestController(null));
        }

        [TestMethod]
        public void GivenViewModelIsGenerated_WhenIClickDeleteServiceRequest_ThenAPartialViewIsReturned_AndViewModelIsGenerated()
        {
            ServiceRequestModel expected = new ServiceRequestModel();
            MockLogicManager.Expect(m => m.GenerateDeleteViewModel(User, 1)).Return(expected);

            var result = Target.Delete(1) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenIDeleteServiceRequest_ThenJsonResultIsReturned()
        {
            ActionResult result = Target.DeleteConfirmed(1);
            
            Assert.IsInstanceOfType(result, typeof(JsonResult));
        }

        [TestMethod]
        public void GivenViewModelIsGenerated_WhenIEdit_ThenAPartialViewIsReturned_AndViewModelIsGenerated()
        {
            ServiceRequestModel expected = new ServiceRequestModel();
            MockLogicManager.Expect(m => m.GenerateEditViewModel(User, 1)).Return(expected);

            var result = Target.Edit(1) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenAnInvalidModelState_WhenIPostEdit_ThenAPartialViewIsReturned()
        {
            ServiceRequestModel expected = new ServiceRequestModel { Id = 1 };
            Target.ModelState.AddModelError("blah", "blah message");

            var result = Target.Edit(expected) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenUserChangedStatusButDidNotFilloutFulfillmentNotes_WhenIPostEdit_ThenAPartialViewIsReturned()
        {
            ServiceRequestModel viewModel = new ServiceRequestModel { Id = 1, OriginalStatusId = 1, SelectedStatusId = 2 };

            var result = Target.Edit(viewModel) as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenUserChangedStatusButDidNotFilloutFulfillmentNotes_WhenIPostEdit_ThenGenerateEditViewModelCalled()
        {
            ServiceRequestModel viewModel = new ServiceRequestModel { Id = 1, OriginalStatusId = 1, SelectedStatusId = 2 };

            var result = Target.Edit(viewModel) as PartialViewResult;

            MockLogicManager.AssertWasCalled(m => m.GenerateEditViewModel(User, viewModel.Id));
        }

        [TestMethod]
        public void WhenICreateAServiceRequest_ThenItIsCreateInTheDatabase()
        {
            ActionResult result = Target.Create();

            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        }

        [TestMethod]
        public void GivenViewModelIsGenerated_WhenICreate_ThenAPartialViewIsReturned_AndViewModelIsGenerated()
        {
            ServiceRequestModel expected = new ServiceRequestModel();
            MockLogicManager.Expect(m => m.GenerateCreateViewModel()).Return(expected);

            var result = Target.Create() as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenACreateServiceRequestActionIsPosted_ThenActionResultIsReturned()
        {
            ActionResult result = Target.Create(new ServiceRequestModel { StudentIds = new List<int> { 1 } });
            
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenEditIsPosted_ThenJsonResultIsReturned()
        {
            var result = Target.Edit(new ServiceRequestModel { OriginalStatusId = 1 }) as JsonResult;

            Assert.IsNotNull(result);
        }
    }
}
