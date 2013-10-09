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
    public class ProgramControllerTest : BaseControllerTest
    {
        private IProgramManager MockLogicManager { get; set; }
        private ProgramController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<IProgramManager>();
            Target = new ProgramController(MockLogicManager);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new ProgramController(null));
        }

        [TestMethod]
        public void GivenLogicManagerGeneratesViewModel_WhenDataTableAjaxHandler_ThenJsonResultContainsViewModel()
        {
            var request = new DataTableRequestModel();
            var expected = new DataTableResultModel();
            MockLogicManager.Expect(m => m.GenerateDataTableResultViewModel(Arg.Is(request), Arg<IClientDataTable<Program>>.Is.NotNull)).Return(expected);

            JsonResult result = Target.DataTableAjaxHandler(request);

            result.AssertGetData(expected);
        }

        [TestMethod]
        public void GivenLogicManagerGeneratesViewModel_WhenCreate_ThenViewResultContainsViewModel()
        {
            ProgramModel expected = new ProgramModel();
            MockLogicManager.Expect(m => m.GenerateCreateViewModel()).Return(expected);

            PartialViewResult actual = Target.Create();

            actual.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenLogicManagerGeneratesViewModel_WhenCreate_ThenViewModelGetsPopulatedLists()
        {
            ProgramModel expected = new ProgramModel();
            MockLogicManager.Expect(m => m.GenerateCreateViewModel()).Return(expected);

            Target.Create();

            MockLogicManager.AssertWasCalled(m => m.PopulateViewModelLists(expected));
        }

        [TestMethod]
        public void GivenValidViewModel_WhenCreate_ThenReturnJsonTrue()
        {
            ProgramModel viewModel = new ProgramModel();

            JsonResult actual = Target.Create(viewModel) as JsonResult;

            actual.AssertGetData(true);
        }

        [TestMethod]
        public void GivenValidViewModel_WhenCreate_ThenLogicManagerCreatesEntity()
        {
            ProgramModel viewModel = new ProgramModel();

            Target.Create(viewModel);

            MockLogicManager.AssertWasCalled(m => m.Create(viewModel));
        }

        [TestMethod]
        public void GivenInvalidViewModel_WhenCreate_ThenReturnViewWithViewModel()
        {
            ProgramModel viewModel = new ProgramModel();
            Target.ModelState.AddModelError("whatever", "this is an invalid view model");

            PartialViewResult actual = Target.Create(viewModel) as PartialViewResult;

            actual.AssertGetViewModel(viewModel);
        }

        [TestMethod]
        public void GivenInvalidViewModel_WhenCreate_ThenLogicManagerDoesNotCreateEntity()
        {
            ProgramModel viewModel = new ProgramModel();
            Target.ModelState.AddModelError("whatever", "this is an invalid view model");

            PartialViewResult actual = Target.Create(viewModel) as PartialViewResult;

            MockLogicManager.AssertWasNotCalled(m => m.Create(viewModel));
        }

        [TestMethod]
        public void GivenInvalidViewModel_WhenCreate_ThenPopulateViewModelListsCalled()
        {
            ProgramModel viewModel = new ProgramModel();
            Target.ModelState.AddModelError("whatever", "this is an invalid view model");

            PartialViewResult actual = Target.Create(viewModel) as PartialViewResult;

            MockLogicManager.AssertWasCalled(m => m.PopulateViewModelLists(viewModel));
        }

        [TestMethod]
        public void GivenLogicManagerGeneratesViewModel_WhenEdit_ThenViewResultContainsViewModel()
        {
            int id = 2384;
            ProgramModel expected = new ProgramModel();
            MockLogicManager.Expect(m => m.GenerateEditViewModel(id)).Return(expected);

            PartialViewResult actual = Target.Edit(id);

            actual.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenLogicManagerGeneratesViewModel_WhenEdit_ThenViewModelGetsPopulatedLists()
        {
            int id = 2384;
            ProgramModel expected = new ProgramModel();
            MockLogicManager.Expect(m => m.GenerateEditViewModel(id)).Return(expected);

            Target.Edit(id);

            MockLogicManager.AssertWasCalled(m => m.PopulateViewModelLists(expected));
        }

        [TestMethod]
        public void GivenValidViewModel_WhenEdit_ThenReturnJsonTrue()
        {
            ProgramModel viewModel = new ProgramModel();

            JsonResult actual = Target.Edit(viewModel) as JsonResult;

            actual.AssertGetData(true);
        }

        [TestMethod]
        public void GivenValidViewModel_WhenEdit_ThenLogicManagerEditsEntity()
        {
            ProgramModel viewModel = new ProgramModel();

            Target.Edit(viewModel);

            MockLogicManager.AssertWasCalled(m => m.Edit(viewModel));
        }

        [TestMethod]
        public void GivenInvalidViewModel_WhenEdit_ThenReturnViewWithViewModel()
        {
            ProgramModel viewModel = new ProgramModel();
            Target.ModelState.AddModelError("whatever", "this is an invalid view model");

            PartialViewResult actual = Target.Edit(viewModel) as PartialViewResult;

            actual.AssertGetViewModel(viewModel);
        }

        [TestMethod]
        public void GivenInvalidViewModel_WhenEdit_ThenLogicManagerDoesNotEditEntity()
        {
            ProgramModel viewModel = new ProgramModel();
            Target.ModelState.AddModelError("whatever", "this is an invalid view model");

            PartialViewResult actual = Target.Edit(viewModel) as PartialViewResult;

            MockLogicManager.AssertWasNotCalled(m => m.Create(viewModel));
        }

        [TestMethod]
        public void GivenInvalidViewModel_WhenEdit_ThenPopulateViewModelListsCalled()
        {
            ProgramModel viewModel = new ProgramModel();
            Target.ModelState.AddModelError("whatever", "this is an invalid view model");

            PartialViewResult actual = Target.Edit(viewModel) as PartialViewResult;

            MockLogicManager.AssertWasCalled(m => m.PopulateViewModelLists(viewModel));
        }

        [TestMethod]
        public void GivenLogicManagerGeneratesViewModel_WhenDelete_ThenViewResultContainsViewModel()
        {
            int id = 328;
            ProgramModel expected = new ProgramModel();
            MockLogicManager.Expect(m => m.GenerateDeleteViewModel(id)).Return(expected);

            PartialViewResult actual = Target.Delete(id);

            actual.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenDeleteConfirmed_ThenLogicManagerDeletesProgramById()
        {
            int id = 328;

            Target.DeleteConfirmed(id);

            MockLogicManager.AssertWasCalled(m => m.Delete(id));
        }

        [TestMethod]
        public void WhenDeleteConfirmed_ThenReturnJsonTrue()
        {
            var actual = Target.DeleteConfirmed(3) as JsonResult;

            actual.AssertGetData(true);
        }

        [TestMethod]
        public void GivenProgramAssociatedWithStudentAssignedOfferings_WhenDeleteConfirmed_ThenPartialViewReturned()
        {
            MockLogicManager.Expect(m => m.Delete(1)).Throw(new ValidationException());
            ProgramModel model = new ProgramModel();
            MockLogicManager.Expect(m => m.GenerateDeleteViewModel(1)).Return(model);

            var actual = Target.DeleteConfirmed(1) as PartialViewResult;

            Assert.IsNotNull(actual);
            actual.AssertGetViewModel(model);
        }
    }
}
