using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.Domain;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class ServiceTypeControllerTest : BaseControllerTest
    {
        private IServiceTypeManager MockLogicManager { get; set; }
        private ServiceTypeController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<IServiceTypeManager>();
            Target = new ServiceTypeController(MockLogicManager);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
            MockHttpContext.Expect(m => m.User).Return(User);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenIContruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceTypeController(null));
        }

        [TestMethod]
        public void WhenAutoCompleteServiceTypeNameActionIsCalled_ThenJsonArrayOfServiceTypeNamesIsReturned()
        {
            var searchTerm = "p";
            var expected = new List<string> { "Provide College Access" };
            MockLogicManager.Expect(m => m.SearchNames(searchTerm)).Return(expected);

            var result = Target.AutocompleteServiceTypeName(searchTerm) as JsonResult;

            result.AssertGetData(expected);
        }

        [TestMethod]
        public void WhenManageIsCalled_ThenAViewIsReturned()
        {
            ViewResult result = Target.Index();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenManageIsCalled_ThenAViewWithAListOfCategoriesIsReturned()
        {
            ServiceTypeListOptionsModel expected = new ServiceTypeListOptionsModel();
            MockLogicManager.Expect(m => m.GenerateListOptionsViewModel(User)).Return(expected);

            ViewResult result = Target.Index();

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenLogicManagerGeneratesViewModel_WhenCreateActionIsCalled_ThenReturnedPartialViewContainsViewModel()
        {
            ServiceTypeModel expected = new ServiceTypeModel();
            MockLogicManager.Expect(m => m.GenerateCreateViewModel(User)).Return(expected);

            PartialViewResult result = Target.Create();

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenACreateActionIsPosted_ThenAActionResultIsReturned()
        {
            ServiceTypeModel model = new ServiceTypeModel
            {
                Id = 0,
                Name = "Bob",
                Description = "Bob's Description"
            };
            ActionResult result = Target.Create(model);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenViewModelHasInvalidState_WhenCreatePosted_ThenViewResultContainsValidationErrorMessage()
        {
            Target.ModelState.AddModelError("Name", new ValidationException());
            var model = new ServiceTypeModel();

            PartialViewResult result = Target.Create(model) as PartialViewResult;

            result.AssertGetViewModel(model);
        }

        [TestMethod]
        public void GivenViewModelHasInvalidState_WhenCreatePosted_ThenViewResultWithViewModelReturned_AndViewModelPopulatedLists()
        {
            var expected = new ServiceTypeModel();
            Target.ModelState.AddModelError("Name", new ValidationException());
            
            PartialViewResult result = Target.Create(expected) as PartialViewResult;

            MockLogicManager.AssertWasCalled(m => m.PopulateViewModel(User, expected));
            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenViewModelHasDuplicateName_WhenCreatePosted_ThenModelStateHasErrors()
        {
            var model = new ServiceTypeModel { Name = "Mentoring" };
            MockLogicManager.Expect(m => m.ValidateForDuplicate(model)).Throw(new ValidationException(new ValidationResult("Error!", new[] { "Name" }), null, model));
            
            PartialViewResult result = Target.Create(model) as PartialViewResult;

            Assert.IsTrue(Target.ModelState["Name"].Errors.Count > 0);
        }

        [TestMethod]
        public void WhenANewlyCreatedIsSubmitted_ThenReturnAJsonResult()
        {
            JsonResult result = Target.Create(new ServiceTypeModel { Id = 3 }) as JsonResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenANewlyCreatedIsSubmitted_WhenCreatePosted_ThenTrueJsonResultReturned()
        {
            ServiceTypeModel viewModel = new ServiceTypeModel();

            JsonResult result = Target.Create(viewModel) as JsonResult;

            Assert.AreEqual(true, result.Data);
        }

        [TestMethod]
        public void GivenNewServiceTypeState_WhenCreatePosted_ThenLogicManagerCreatesServiceType()
        {
            ServiceTypeModel expected = new ServiceTypeModel();

            Target.Create(expected);

            MockLogicManager.AssertWasCalled(m => m.Create(expected));
        }

        [TestMethod]
        public void GivenViewModelIsGenerated_WhenEditButtonIsClicked_ThenAViewResultIsCreated()
        {
            ServiceTypeModel expected = new ServiceTypeModel();
            MockLogicManager.Expect(m => m.GenerateEditViewModel(User, 1)).Return(expected);

            var result = Target.Edit(1) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenEditIsSubmitted_ThenReturnSuccessJsonResult()
        {
            JsonResult result = Target.Edit(new ServiceTypeModel { Id = 1, Name = "Bob" }) as JsonResult;

            Assert.AreEqual(true, result.Data);
        }

        [TestMethod]
        public void GivenAnEditedServiceTypeIsSubmittedWithoutAUniqueName_WhenEditPosted_ThenReturnViewResultWithViewModel_AndNameHasModelStateErrors()
        {
            var expected = new ServiceTypeModel { Id = 1, Name = "Mentoring" };
            MockLogicManager.Expect(m => m.ValidateForDuplicate(expected)).Throw(new ValidationException(new ValidationResult("Error!", new[] { "Name" }), null, expected));
            
            PartialViewResult result = Target.Edit(expected) as PartialViewResult;

            result.AssertGetViewModel(expected);
            Assert.IsTrue(Target.ModelState["Name"].Errors.Count > 0);
        }

        [TestMethod]
        public void GivenAnInvalidModelState_WhenIEditType_ThenTheCorrectCategoriesAreAddedToTheViewModel()
        {
            var expected = new ServiceTypeModel { Id = 1, Name = "Mentoring" };
            MockLogicManager.Expect(m => m.ValidateForDuplicate(expected)).Throw(new ValidationException(new ValidationResult("Error!", new[] { "Name" }), null, expected));

            Target.Edit(expected);

            MockLogicManager.AssertWasCalled(m => m.PopulateViewModel(User, expected));
        }

        [TestMethod]
        public void GivenValidServiceTypeId_WhenICallDelete_ThenAPartialViewResultIsReturned()
        {
            ServiceTypeModel expected = new ServiceTypeModel();
            MockLogicManager.Expect(m => m.GenerateDeleteViewModel(1)).Return(expected);

            var result = Target.Delete(1) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenValidationExceptionThrown_WhenDeleteIsClicked_ThenAPartialViewResultIsCreated_AndExpectedViewModelGenerated()
        {
            ServiceTypeModel expected = new ServiceTypeModel();
            MockLogicManager.Expect(m => m.Delete(1)).Throw(new ValidationException());
            MockLogicManager.Expect(m => m.GenerateDeleteViewModel(1)).Return(expected);

            PartialViewResult result = Target.DeleteConfirmed(1) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenValidationExceptionThrown_WhenDeleteIsClicked_ThenModelStateContainsExpectedError()
        {
            MockLogicManager.Expect(m => m.Delete(1)).Throw(new ValidationException(new ValidationResult("Error!", new[] { "Name" }), null, 1));
            
            Target.DeleteConfirmed(1);

            ModelState errorState = Target.ModelState.Where(e => e.Key == "Name").Select(e => e.Value).SingleOrDefault();
            Assert.IsNotNull(errorState);
            Assert.IsTrue(errorState.Errors.Any());
        }

        [TestMethod]
        public void GivenValidServiceTypeId_WhenIsDeleted_JsonResultContainsTrue()
        {
            JsonResult result = Target.DeleteConfirmed(2) as JsonResult;

            Assert.AreEqual(true, result.Data);
        }

        [TestMethod]
        public void GivenUserIsNotDataAdmin_WhenGettingTableData_ThenViewModelGenerated()
        {
            AssertDataTableAjaxHandler(false);
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenGettingTableData_ThenViewModelGenerated()
        {
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } });

            AssertDataTableAjaxHandler(true);
        }

        [TestMethod]
        public void WhenISetPrivateServiceType_ThenServiceTypeIsUpdated()
        {
            var result = Target.SetPrivate(1, true) as JsonResult;

            result.AssertGetData(true);
        }

        [TestMethod]
        public void WhenIAttemptToDeleteAServiceTypeWithAssociatedServiceOfferings_ThenAValidationErrorIsThrown()
        {
            var result = Target.DeleteConfirmed(1) as ActionResult;
            
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenSelector_ThenPartialViewResultReturned()
        {
            PartialViewResult actual = Target.Selector();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenLogicManagerGeneratesViewModel_WhenSelector_ThenViewModelInResult()
        {
            ServiceTypeSelectorModel expected = new ServiceTypeSelectorModel();
            MockLogicManager.Expect(m => m.GenerateSelectorViewModel()).Return(expected);

            PartialViewResult result = Target.Selector();

            result.AssertGetViewModel(expected);
        }

        private void AssertDataTableAjaxHandler(bool expectedIsAdministrator)
        {
            var expected = new DataTableResultModel();
            var requestModel = new DataTableRequestModel();
            MockLogicManager.Expect(m => m.GenerateDataTableResultViewModel(requestModel, null)).IgnoreArguments().Do(new Func<DataTableRequestModel, IClientDataTable<ServiceType>, DataTableResultModel>((p, r) =>
            {
                ServiceTypeClientDataTable castR = r as ServiceTypeClientDataTable;
                Assert.IsNotNull(castR);
                Assert.AreEqual(expectedIsAdministrator, castR.IsAdministrator);
                return expected;
            }));

            JsonResult result = Target.DataTableAjaxHandler(requestModel);

            result.AssertGetData(expected);
        }
    }
}
