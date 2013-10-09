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
    public class CustomFieldControllerTest : BaseControllerTest
    {
        private ICustomFieldManager MockLogicManager { get; set; }
        private CustomFieldController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<ICustomFieldManager>();
            Target = new CustomFieldController(MockLogicManager);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
            MockHttpContext.Expect(m => m.User).Return(User);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new CustomFieldController(null));
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
            CustomFieldSelectorModel expected = new CustomFieldSelectorModel();
            MockLogicManager.Expect(m => m.GenerateSelectorViewModel(User)).Return(expected);

            PartialViewResult result = Target.Selector();

            result.AssertGetViewModel(expected);
        }
    }
}
