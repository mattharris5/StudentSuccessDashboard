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
    public class SchoolControllerTest : BaseControllerTest
    {
        private ISchoolDistrictManager MockLogicManager { get; set; }
        private SchoolController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<ISchoolDistrictManager>();
            Target = new SchoolController(MockLogicManager);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
            MockHttpContext.Expect(m => m.User).Return(User);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new SchoolController(null));
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
            SchoolSelectorModel expected = new SchoolSelectorModel();
            MockLogicManager.Expect(m => m.GenerateSchoolSelectorViewModel()).Return(expected);

            PartialViewResult result = Target.Selector();

            result.AssertGetViewModel(expected);
        }
    }
}
