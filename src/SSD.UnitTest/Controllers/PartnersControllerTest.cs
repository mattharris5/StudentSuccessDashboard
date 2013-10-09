using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.Domain;
using SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class PartnersControllerTest : BaseControllerTest
    {
        private IProviderManager MockProviderLogicManager { get; set; }
        private IProgramManager MockProgramLogicManager { get; set; }
        private IServiceTypeManager MockServiceTypeManager { get; set; }
        private PartnersController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockProviderLogicManager = MockRepository.GenerateMock<IProviderManager>();
            MockProgramLogicManager = MockRepository.GenerateMock<IProgramManager>();
            MockServiceTypeManager = MockRepository.GenerateMock<IServiceTypeManager>();
            Target = new PartnersController(MockProviderLogicManager, MockProgramLogicManager, MockServiceTypeManager);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
            MockHttpContext.Expect(m => m.User).Return(User);
        }

        [TestMethod]
        public void GivenNullProviderManager_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new PartnersController(null, MockProgramLogicManager, MockServiceTypeManager));
        }

        [TestMethod]
        public void GivenNullProgramManager_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new PartnersController(MockProviderLogicManager, null, MockServiceTypeManager));
        }

        [TestMethod]
        public void GivenNullServiceTypeManager_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new PartnersController(MockProviderLogicManager, MockProgramLogicManager, null));
        }

        [TestMethod]
        public void WhenManageActionIsCalled_ThenAviewResultIsCreated()
        {
            ViewResult result = Target.Index();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenIndex_ThenListOptionsViewModelInResult()
        {
            ViewResult result = Target.Index();

            result.AssertGetViewModel<PartnerListOptionsModel>();
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenIndex_ThenShowAddProviderTrue()
        {
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } });

            ViewResult result = Target.Index();

            PartnerListOptionsModel actual = result.AssertGetViewModel<PartnerListOptionsModel>();
            Assert.IsTrue(actual.ShowAddProvider);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_WhenIndex_ThenShowAddProviderTrue()
        {
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.SiteCoordinator } });

            ViewResult result = Target.Index();

            PartnerListOptionsModel actual = result.AssertGetViewModel<PartnerListOptionsModel>();
            Assert.IsTrue(actual.ShowAddProvider);
        }

        [TestMethod]
        public void GivenUserIsProvider_WhenIndex_ThenShowAddProviderFalse()
        {
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.Provider } });

            ViewResult result = Target.Index();

            PartnerListOptionsModel actual = result.AssertGetViewModel<PartnerListOptionsModel>();
            Assert.IsFalse(actual.ShowAddProvider);
        }

        [TestMethod]
        public void GivenUserHasNoRoles_WhenIndex_ThenShowAddProviderFalse()
        {
            ViewResult result = Target.Index();

            PartnerListOptionsModel actual = result.AssertGetViewModel<PartnerListOptionsModel>();
            Assert.IsFalse(actual.ShowAddProvider);
        }

        [TestMethod]
        public void GivenProviderNamesMatchTerm_WhenAutoCompletePartnerName_ThenJsonResultContainsMatchingProviderNames()
        {
            var expected = new[] { "Yellow", "YMCA" };
            MockProviderLogicManager.Expect(m => m.SearchProviderNames("Y")).Return(expected);
            MockProgramLogicManager.Expect(m => m.SearchProgramNames("Y")).Return(Enumerable.Empty<string>());
            MockServiceTypeManager.Expect(m => m.SearchNames("Y")).Return(Enumerable.Empty<string>());

            var result = Target.AutocompletePartnerName("Y") as JsonResult;

            IEnumerable<string> actual = result.AssertGetData<IEnumerable<string>>();
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        [TestMethod]
        public void GivenProgramNamesMatchTerm_WhenAutoCompletePartnerName_ThenJsonResultContainsMatchingProgramNames()
        {
            var expected = new[] { "Yellow", "YMCA" };
            MockProviderLogicManager.Expect(m => m.SearchProviderNames("Y")).Return(Enumerable.Empty<string>());
            MockProgramLogicManager.Expect(m => m.SearchProgramNames("Y")).Return(expected);
            MockServiceTypeManager.Expect(m => m.SearchNames("Y")).Return(Enumerable.Empty<string>());

            var result = Target.AutocompletePartnerName("Y") as JsonResult;

            IEnumerable<string> actual = result.AssertGetData<IEnumerable<string>>();
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        [TestMethod]
        public void GivenServiceTypeNamesMatchTerm_WhenAutoCompletePartnerName_ThenJsonResultContainsMatchingServiceTypeNames()
        {
            var expected = new[] { "Yellow", "Yeppers" };
            MockProviderLogicManager.Expect(m => m.SearchProviderNames("Y")).Return(Enumerable.Empty<string>());
            MockProgramLogicManager.Expect(m => m.SearchProgramNames("Y")).Return(Enumerable.Empty<string>());
            MockServiceTypeManager.Expect(m => m.SearchNames("Y")).Return(expected);

            var result = Target.AutocompletePartnerName("Y") as JsonResult;

            IEnumerable<string> actual = result.AssertGetData<IEnumerable<string>>();
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        [TestMethod]
        public void GivenProviderNamesMatchTerm_AndProgramNamesMatchTerm_AndServiceTypesMatchTerm_WhenAutoCompletePartnerName_ThenJsonResultContainsMatchingProviderNames_AndMatchingProgramNames()
        {
            var expectedProviderMatches = new[] { "Yellow", "YMCA" };
            var expectedProgramMatches = new[] { "Playground Activities" };
            var expectedServiceTypeMatches = new[] { "Service Type" };
            var expected = new[] { "Playground Activities", "Service Type", "Yellow", "YMCA" };
            MockProviderLogicManager.Expect(m => m.SearchProviderNames("Y")).Return(expectedProviderMatches);
            MockProgramLogicManager.Expect(m => m.SearchProgramNames("Y")).Return(expectedProgramMatches);
            MockServiceTypeManager.Expect(m => m.SearchNames("Y")).Return(expectedServiceTypeMatches);

            var result = Target.AutocompletePartnerName("Y") as JsonResult;

            IEnumerable<string> actual = result.AssertGetData<IEnumerable<string>>();
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }
    }
}
