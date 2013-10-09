using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.Domain;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class UserControllerTest : BaseControllerTest
    {
        private IAccountManager MockAccountManager { get; set; }
        private ISecurityConfiguration MockSecurityConfiguration { get; set; }
        private UserController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockAccountManager = MockRepository.GenerateMock<IAccountManager>();
            MockSecurityConfiguration = MockRepository.GenerateMock<ISecurityConfiguration>();
            Target = new UserController(MockAccountManager, MockSecurityConfiguration);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
            MockHttpContext.Expect(m => m.User).Return(User);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new UserController(null, MockSecurityConfiguration));
        }

        [TestMethod]
        public void GivenNullSecurityConfiguration_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new UserController(MockAccountManager, null));
        }

        [TestMethod]
        public void WhenUserManageActionIsCalled_ThenAViewResultIsCreated()
        {
            ViewResult result = Target.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenAutocompleteFirstNameIsCalled_ThenJsonResultIsReturned()
        {
            JsonResult result = Target.AutocompleteFirstName(string.Empty);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenAutocompleteFirstNameIsCalled_ThenJsonResultHasData()
        {
            IEnumerable<string> expected = new string[] { "bob", "bob smith" };
            string expectedTerm = "bob";
            MockAccountManager.Expect(m => m.SearchFirstNames(expectedTerm)).Return(expected);

            JsonResult actual = Target.AutocompleteFirstName(expectedTerm);

            actual.AssertGetData(expected);
        }

        [TestMethod]
        public void WhenAutocompleteFirstNameIsCalled_ThenLogicManagerGeneratesViewModel()
        {
            string expectedTerm = "bob";

            JsonResult actual = Target.AutocompleteFirstName(expectedTerm);

            MockAccountManager.AssertWasCalled(m => m.SearchFirstNames(expectedTerm));
        }

        [TestMethod]
        public void WhenAutocompleteLastNameIsCalled_ThenJsonResultIsReturned()
        {
            JsonResult result = Target.AutocompleteLastName(string.Empty);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenAutocompleteLastNameIsCalled_ThenJsonResultHasData()
        {
            IEnumerable<string> expected = new string[] { "bob", "bob smith" };
            string expectedTerm = "bob";
            MockAccountManager.Expect(m => m.SearchLastNames(expectedTerm)).Return(expected);

            JsonResult actual = Target.AutocompleteLastName(expectedTerm);

            actual.AssertGetData(expected);
        }

        [TestMethod]
        public void WhenAutocompleteLastNameIsCalled_ThenLogicManagerGeneratesViewModel()
        {
            string expectedTerm = "bob";

            JsonResult actual = Target.AutocompleteLastName(expectedTerm);

            MockAccountManager.AssertWasCalled(m => m.SearchLastNames(expectedTerm));
        }

        [TestMethod]
        public void WhenAutocompleteEmailIsCalled_ThenJsonResultIsReturned()
        {
            JsonResult result = Target.AutocompleteEmail(string.Empty);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenAutocompleteEmailIsCalled_ThenJsonResultHasData()
        {
            IEnumerable<string> expected = new string[] { "bob", "bob smith" };
            string expectedTerm = "bob";
            MockAccountManager.Expect(m => m.SearchEmails(expectedTerm)).Return(expected);

            JsonResult actual = Target.AutocompleteEmail(expectedTerm);
            
            actual.AssertGetData(expected);
        }

        [TestMethod]
        public void WhenAutocompleteEmailIsCalled_ThenLogicManagerGeneratesViewModel()
        {
            string expectedTerm = "bob";

            JsonResult actual = Target.AutocompleteEmail(expectedTerm);

            MockAccountManager.AssertWasCalled(m => m.SearchEmails(expectedTerm));
        }

        [TestMethod]
        public void WhenRequestDataTableAjaxHandler_ThenJsonResultContainsDataTableResultModel()
        {
            DataTableRequestModel requestModel = new DataTableRequestModel();
            MockAccountManager.Expect(m => m.GenerateDataTableResultViewModel(Arg.Is(requestModel), Arg<IClientDataTable<User>>.Is.NotNull)).Return(new DataTableResultModel());

            JsonResult result = Target.DataTableAjaxHandler(requestModel);

            result.AssertGetData<DataTableResultModel>();
        }

        [TestMethod]
        public void WhenRequestDataTableAjaxHandler_ThenGenerateListViewModelReceivesCorrectDataTableRequest_AndResultPassedToJson()
        {
            DataTableResultModel expected = new DataTableResultModel();
            DataTableRequestModel requestModel = new DataTableRequestModel();
            MockAccountManager.Expect(m => m.GenerateDataTableResultViewModel(Arg.Is(requestModel), Arg<IClientDataTable<User>>.Is.NotNull)).Return(expected);

            JsonResult result = Target.DataTableAjaxHandler(requestModel);

            result.AssertGetData(expected);
        }

        [TestMethod]
        public void WhenICreateAUserRole_ThenPartiViewReturned()
        {
            MockAccountManager.Expect(m => m.GenerateCreateViewModel(1)).Return(new UserRoleModel());

            PartialViewResult result = (PartialViewResult)Target.CreateRole(1);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenLogicManagerReturnsViewModel_WhenICreateRole_ThenPartialViewResultContainsViewModel()
        {
            int expectedId = 878797;
            UserRoleModel expected = new UserRoleModel();
            MockAccountManager.Expect(m => m.GenerateCreateViewModel(expectedId)).Return(expected);

            var result = Target.CreateRole(expectedId) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenAValidViewModel_WhenCreateRole_ThenJsonResultReturned_AndDataIsTrue()
        {
            JsonResult result = Target.CreateRole(new UserRoleModel { UserId = 1 }) as JsonResult;

            result.AssertGetData(true);
        }

        [TestMethod]
        public void GivenGenerateViewModelThrowsValidationExceptionOnPostedRoles_WhenCreateRole_ThenPartialViewResultIsReturned_AndModelStateContainsPostedRolesErrors()
        {
            UserRoleModel viewModel = new UserRoleModel { UserId = 1 };
            MockAccountManager.Expect(m => m.Create(viewModel, User)).Throw(new ValidationException(new ValidationResult("fake error happened!", new string[] { "PostedRoles" }), null, null));
            
            PartialViewResult result = Target.CreateRole(viewModel) as PartialViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(Target.ModelState["PostedRoles"].Errors.Count > 0);
        }

        [TestMethod]
        public void WhenIEditAUserRole_ThenPartiViewReturned()
        {
            MockAccountManager.Expect(m => m.GenerateEditViewModel(1)).Return(new UserRoleModel());

            PartialViewResult result = (PartialViewResult)Target.EditRole(1);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenLogicManagerReturnsViewModel_WhenIEditRole_ThenPartialViewResultContainsViewModel()
        {
            int expectedId = 878797;
            UserRoleModel expected = new UserRoleModel();
            MockAccountManager.Expect(m => m.GenerateEditViewModel(expectedId)).Return(expected);

            var result = Target.EditRole(expectedId) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenAValidViewModel_WhenEditRole_ThenJsonResultReturned_AndDataIsTrue()
        {
            JsonResult result = Target.EditRole(new UserRoleModel { UserId = 1 }) as JsonResult;

            result.AssertGetData(true);
        }

        [TestMethod]
        public void WhenToggleActivationIsCalled_ThenJsonResultReturned_AndDataIsTrue()
        {
            JsonResult result = Target.ToggleActivation(1, true) as JsonResult;

            result.AssertGetData(true);
        }

        [TestMethod]
        public void WhenMultiToggleActivationIsCalled_ThenJsonResultReturned_AndDataIsTrue()
        {
            JsonResult result = Target.MultiToggleActivation(new int[] { 1 }, true) as JsonResult;

            result.AssertGetData(true);
        }

        [TestMethod]
        public void GivenNoSearchFilter_WhenIGetUserIds_ThenAllIdsAreReturned()
        {
            var expected = new[] { 18, 382 };
            MockAccountManager.Expect(m => m.GetFilteredUserIds(Arg<IClientDataTable<User>>.Is.NotNull)).Return(expected);

            var result = Target.AllFilteredUserIds() as JsonResult;

            result.AssertGetData(expected);
        }

        [TestMethod]
        public void GivenAValidUserId_WhenIGenerateUserAssociations_ThenAPartialViewIsReturned()
        {
            int id = 1;
            MockAccountManager.Expect(m => m.GenerateUserAssociationsViewModel(id)).Return(new UserAssociationsModel());

            var result = Target.UserAssociations(id) as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenICallMultiUserActivation_ThenAPartialViewResultIsReturned()
        {
            var result = Target.MultiUserActivation(true, "Test") as PartialViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenId_WhenUserAccessChangeEvents_ThenViewResultReturned()
        {
            MockAccountManager.Expect(m => m.GenerateUserAccessChangeEventModel(1)).Return(new UserModel());

            var result = Target.AccessAudit(1) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenValidModelWithId_WhenAccessAuditDataTableAjaxHandler_TheJSonResultReturned()
        {
            HttpRequestBase MockRequest = MockRepository.GenerateMock<HttpRequestBase>();
            MockRequest.Expect(m => m["id"]).Return("1");
            DataTableRequestModel model = new DataTableRequestModel();
            MockHttpContext.Expect(c => c.Request).Return(MockRequest).Repeat.Any();

            var result = Target.AccessAuditDataTableAjaxHandler(model) as JsonResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenId_WhenLoginAudit_ThenViewResultReturned()
        {
            MockAccountManager.Expect(m => m.GenerateUserLoginEventModel(1)).Return(new UserModel());

            var result = Target.LoginAudit(1) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenValidModelWithId_WhenLoginAuditDataTableAjaxHandler_TheJSonResultReturned()
        {
            HttpRequestBase MockRequest = MockRepository.GenerateMock<HttpRequestBase>();
            MockRequest.Expect(m => m["id"]).Return("1");
            DataTableRequestModel model = new DataTableRequestModel();
            MockHttpContext.Expect(c => c.Request).Return(MockRequest).Repeat.Any();

            var result = Target.LoginAuditDataTableAjaxHandler(model) as JsonResult;

            Assert.IsNotNull(result);
        }
    }
}
