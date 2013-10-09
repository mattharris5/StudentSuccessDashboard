using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SSD.Controllers
{
    [TestClass]
    public class WidgetControllerTest
    {
        private IWidgetManager MockLogicManager { get; set; }
        private WidgetController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<IWidgetManager>();
            Target = new WidgetController(MockLogicManager);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new WidgetController(null));
        }

        [TestMethod]
        public void WhenServiceTypeMetrics_ThenPartialViewResultIsCreated()
        {
            PartialViewResult actual = Target.ServiceTypeMetrics();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenLogicManagerGeneratesViewModels_WhenServiceTypeMetrics_ThenPartialViewContainsViewModels()
        {
            IEnumerable<ServiceTypeMetricModel> expected = new List<ServiceTypeMetricModel>();
            MockLogicManager.Expect(m => m.GenerateServiceTypeMetricModels()).Return(expected);

            var actual = Target.ServiceTypeMetrics();

            actual.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenServiceRequestsBySchool_ThenPartialViewResultReturned()
        {
            var actual = Target.ServiceRequestsBySchool() as PartialViewResult;

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenViewModelGenerated_WhenServiceRequestsBySchool_ThenViewHasViewModel()
        {
            IEnumerable<ServiceRequestsBySchoolModel> expected = new List<ServiceRequestsBySchoolModel>();
            MockLogicManager.Expect(m => m.GenerateServiceRequestsBySchoolModel()).Return(expected);

            var actual = Target.ServiceRequestsBySchool();

            actual.AssertGetViewModel(expected);
        }
    }
}
