using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.ViewModels;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class AgreementControllerTest : BaseControllerTest
    {
        private IAgreementManager MockLogicManager { get; set; }
        private AgreementController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<IAgreementManager>();
            Target = new AgreementController(MockLogicManager);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
            MockHttpContext.Expect(m => m.User).Return(User);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new AgreementController(null));
        }

        [TestMethod]
        public void WhenIndex_ThenViewResultReturned()
        {
            ViewResult actual = Target.Index();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenLogicManagerReturnsViewModel_WhenIndex_ThenViewResultContainsModel()
        {
            EulaModel expected = new EulaModel();
            MockLogicManager.Expect(m => m.GeneratePromptViewModel()).Return(expected);

            ViewResult result = Target.Index();

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenViewModel_WhenPostIndex_ThenLogicManagerLogsViewModelForUser()
        {
            EulaModel expected = new EulaModel();

            Target.Index(expected, null);

            MockLogicManager.AssertWasCalled(m => m.Log(expected, User));
        }

        [TestMethod]
        public void GivenNullRedirect_WhenPostIndex_ThenRedirectToHome()
        {
            EulaModel viewModel = new EulaModel();

            var actual = Target.Index(viewModel, null) as RedirectToRouteResult;

            actual.AssertActionRedirection("Index", "Home");
        }

        [TestMethod]
        public void GivenRedirectUrl_WhenPostIndex_ThenRedirectToHome()
        {
            string expected = "whatever";
            EulaModel viewModel = new EulaModel();

            var actual = Target.Index(viewModel, expected) as RedirectResult;

            Assert.AreEqual(expected, actual.Url);
        }

        [TestMethod]
        public void GivenLogicManagerReturnsViewModel_WhenAdmin_ThenViewResultReturned()
        {
            MockLogicManager.Expect(m => m.GenerateEulaAdminModel()).Return(new EulaModel());

            var result = Target.Admin() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenValidModel_WhenAdminPosts_ThenLogicManagerCreates()
        {
            var model = new EulaModel { Id = 1, EulaText = "blah blah blah" };

            Target.Admin(model);

            MockLogicManager.AssertWasCalled(m => m.Create(model, User));
        }

        [TestMethod]
        public void GivenValidModel_WhenAdminPosts_ThenPageWithReturnedWithNewViewModel()
        {
            var model = new EulaModel { Id = 1, EulaText = "blah blah blah" };
            var expected = new EulaModel { Id = 2, EulaText = "blah blah blah" };
            MockLogicManager.Expect(m => m.GenerateEulaAdminModel()).Return(expected);

            var result = Target.Admin(model) as ViewResult;

            Assert.IsNotNull(result);
            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenInvalidModelState_WhenAdminPosts_ThenPartivalViewIsReturned()
        {
            Target.ModelState.AddModelError("key", "error");
            var model = new EulaModel { Id = 1, EulaText = "blah" };

            var result = Target.Admin(model) as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenInvalidModelState_WhenAdminPosts_ThenPartialViewHasExpectedViewModel()
        {
            Target.ModelState.AddModelError("key", "error");
            var expected = new EulaModel { Id = 1, EulaText = "blah" };

            var result = Target.Admin(expected) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenAValidEulaId_WhenUserEula_ThenAPartialViewReturned()
        {
            int id = 1;
            MockLogicManager.Expect(m => m.GenerateEulaModelByUser(id)).Return(new EulaModel());

            var result = Target.UserEula(id) as PartialViewResult;

            Assert.IsNotNull(result);
        }
    }
}
