using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.Domain;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class StudentApprovalControllerTest : BaseControllerTest
    {
        private ISchoolDistrictManager MockLogicManager { get; set; }
        private StudentApprovalController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<ISchoolDistrictManager>();
            Target = new StudentApprovalController(MockLogicManager);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new StudentApprovalController(null));
        }

        [TestMethod]
        public void GivenLogicManagerGeneratesViewModel_WhenGetManageView_ThenViewContainsViewModel()
        {
            StudentApprovalListOptionsModel expected = new StudentApprovalListOptionsModel();
            MockLogicManager.Expect(m => m.GenerateApprovalListOptionsViewModel()).Return(expected);

            ViewResult actual = Target.Index();

            actual.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenGetDataTableAjaxHandler_ThenReturnViewModelFromLogicManager()
        {
            DataTableRequestModel model = new DataTableRequestModel();

            Target.DataTableAjaxHandler(model);

            MockLogicManager.AssertWasCalled(m => m.GenerateApprovalDataTableResultViewModel(Arg.Is(model), Arg<IClientDataTable<Student>>.Is.NotNull));
        }

        [TestMethod]
        public void WhenGetDataTableAjaxHandler_ThenReturnGeneratedViewModel()
        {
            DataTableRequestModel model = new DataTableRequestModel();
            DataTableResultModel expected = new DataTableResultModel();
            MockLogicManager.Expect(m => m.GenerateApprovalDataTableResultViewModel(Arg.Is(model), Arg<IClientDataTable<Student>>.Is.NotNull)).Return(expected);

            JsonResult result = Target.DataTableAjaxHandler(model);

            result.AssertGetData(expected);
        }

        [TestMethod]
        public void GivenLogicManagerGeneratesViewModel_WhenGetAddProviders_ThenViewResultReturned()
        {
            MockLogicManager.Expect(m => m.GenerateAddStudentApprovalViewModel(1)).Return(new AddStudentApprovalModel());

            ActionResult actual = Target.AddProviders(1);

            Assert.IsInstanceOfType(actual, typeof(PartialViewResult));
        }

        [TestMethod]
        public void GivenLogicManagerGeneratesViewModel_WhenGetAddProviders_ThenViewResultContainsViewModel()
        {
            AddStudentApprovalModel expected = new AddStudentApprovalModel();
            MockLogicManager.Expect(m => m.GenerateAddStudentApprovalViewModel(1)).Return(expected);

            PartialViewResult actual = Target.AddProviders(1) as PartialViewResult;

            actual.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenValidViewModel_WhenPostAddProviders_ThenReturnJsonTrueResult()
        {
            JsonResult result = Target.AddProviders(new AddStudentApprovalModel()) as JsonResult;

            result.AssertGetData(true);
        }

        [TestMethod]
        public void GivenModelErrors_WhenPostAddProviders_ThenViewResultReturnedWithViewModel()
        {
            AddStudentApprovalModel expected = new AddStudentApprovalModel();
            Target.ModelState.AddModelError("whatever", "this is an error");

            PartialViewResult result = Target.AddProviders(expected) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenModelErrors_WhenPostAddProviders_ThenViewModelGetsPopulated()
        {
            AddStudentApprovalModel expected = new AddStudentApprovalModel();
            Target.ModelState.AddModelError("whatever", "this is an error");

            PartialViewResult result = Target.AddProviders(expected) as PartialViewResult;

            MockLogicManager.AssertWasCalled(m => m.PopulateViewModelLists(expected));
        }

        [TestMethod]
        public void GivenViewModelGenerated_WhenRemoveProvider_ThenViewContainsViewModel()
        {
            RemoveApprovedProviderModel expected = new RemoveApprovedProviderModel();
            MockLogicManager.Expect(m => m.GenerateRemoveProviderViewModel(1, 1)).Return(expected);

            PartialViewResult result = Target.RemoveProvider(1, 1) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenValidViewModel_WhenRemoveProviderPost_ThenLogicManagerRemovesProvider_AndJsonSuccessResultReturned()
        {
            RemoveApprovedProviderModel viewModel = new RemoveApprovedProviderModel();

            JsonResult result = Target.RemoveProvider(viewModel) as JsonResult;

            MockLogicManager.AssertWasCalled(m => m.RemoveProvider(viewModel));
            result.AssertGetData(true);
        }

        [TestMethod]
        public void GivenViewModelGenerated_WhenRemoveAllProvidersBySchool_ThenViewModelContains()
        {
            RemoveApprovedProvidersBySchoolModel expected = new RemoveApprovedProvidersBySchoolModel();
            MockLogicManager.Expect(m => m.GenerateRemoveProvidersBySchoolViewModel()).Return(expected);

            PartialViewResult result = Target.RemoveAllProvidersBySchool();

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenSelectedSchoolsInViewModel_WhenRemoveAllProvidersBySchool_ThenLogicManagerInvokedWithSchoolIds()
        {
            IEnumerable<int> expected = new[] { 48, 438 };
            RemoveApprovedProvidersBySchoolModel viewModel = new RemoveApprovedProvidersBySchoolModel { SelectedSchools = expected };

            Target.RemoveAllProvidersBySchool(viewModel);

            MockLogicManager.AssertWasCalled(m => m.RemoveAllProviders(expected));
        }

        [TestMethod]
        public void WhenRemoveAllProviders_ThenLogicManagerInvokedWithoutSchoolId()
        {
            Target.RemoveAllProviders();

            MockLogicManager.AssertWasCalled(m => m.RemoveAllProviders());
        }

        [TestMethod]
        public void GivenStudentHasOptOut_WhenSetOptOut_ThenLogicManagerUpdatesStateForStudent()
        {
            Target.SetOptOut(1, true);

            MockLogicManager.AssertWasCalled(m => m.SetStudentOptOutState(1, true));
        }

        [TestMethod]
        public void GivenStudentDoesNotHaveOptOut_WhenSetOptOut_ThenLogicManagerUpdatesStateForStudent()
        {
            Target.SetOptOut(2, false);

            MockLogicManager.AssertWasCalled(m => m.SetStudentOptOutState(2, false));
        }

        [TestMethod]
        public void GivenStudentIdValid_WhenSetOptOut_ThenReturnTrueJsonResultValue()
        {
            JsonResult result = Target.SetOptOut(2, false) as JsonResult;

            result.AssertGetData(true);
        }

        [TestMethod]
        public void WhenCountStudentsWithApprovedProviders_ThenReturnJsonResult()
        {
            JsonResult actual = Target.CountStudentsWithApprovedProviders();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenCountFromLogicManager_WhenCountStudentsWithApprovedProviders_ThenJsonResultIsCountFromLogicManager()
        {
            int expected = 83498;
            MockLogicManager.Expect(m => m.CountStudentsWithApprovedProviders()).Return(expected);

            JsonResult result = Target.CountStudentsWithApprovedProviders();

            result.AssertGetData(expected);
        }
    }
}
