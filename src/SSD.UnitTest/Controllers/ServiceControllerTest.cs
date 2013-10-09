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
    public class ServiceControllerTest : BaseControllerTest
    {
        private IScheduledServiceManager MockLogicManager { get; set; }
        private ServiceController Target { get; set; }
        
        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<IScheduledServiceManager>();
            Target = new ServiceController(MockLogicManager);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
            MockHttpContext.Expect(m => m.User).Return(User);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenIContruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceController(null));
        }

        [TestMethod]
        public void GivenViewModelGenerated_WhenIScheduleServiceOffering_ThenViewResultReturned_AndViewModelSet()
        {
            ScheduleServiceOfferingListOptionsModel expected = new ScheduleServiceOfferingListOptionsModel();
            var students = new int[] { 1, 2 };
            MockLogicManager.Expect(m => m.GenerateScheduleOfferingViewModel(User, students)).Return(expected);
            Target.TempData["ScheduleOfferingIds"] = students;

            var result = Target.ScheduleOffering() as ViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenViewModelIsGenerated_AndAReturnUrl_WhenIScheduleServiceOffering_ThenViewModelHasReturnUrl()
        {
            var students = new int[] { 1, 2 };
            MockLogicManager.Expect(m => m.GenerateScheduleOfferingViewModel(User, students)).Return(new ScheduleServiceOfferingListOptionsModel());
            string expected = "blahlsdkjfsdlfkjsdlkfjs";
            Target.TempData["ScheduleOfferingIds"] = students;
            Target.TempData["ScheduleOfferingReturnUrl"] = expected;

            ViewResult result = Target.ScheduleOffering() as ViewResult;

            ScheduleServiceOfferingListOptionsModel actual = result.AssertGetViewModel<ScheduleServiceOfferingListOptionsModel>();
            Assert.AreEqual(expected, actual.ReturnUrl);
        }

        [TestMethod]
        public void GivenViewModelGenerated_WhenCreateScheduledOffering_ThenPartailViewResultReturned_AndViewModelSet()
        {
            ServiceOfferingScheduleModel expected = new ServiceOfferingScheduleModel();
            MockLogicManager.Expect(m => m.GenerateCreateViewModel(1)).Return(expected);

            PartialViewResult result = Target.CreateScheduledOffering(1) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenSelectedStudents_WhenIPostCreateServiceOfferingSchedule_ThenJsonResultYieldsTrue()
        {
            JsonResult result = Target.CreateScheduledOffering(new ServiceOfferingScheduleModel()) as JsonResult;

            result.AssertGetData(true);
        }

        [TestMethod]
        public void GivenValidAssignedServiceOfferingId_AndValidStudentId_WhenIEditScheduledServiceOffering_ThenViewModelReturned()
        {
            StudentServiceOfferingScheduleModel expected = new StudentServiceOfferingScheduleModel();
            MockLogicManager.Expect(m => m.GenerateEditViewModel(User, 1)).Return(expected);

            PartialViewResult result = Target.EditScheduledOffering(1) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenValidAssignedServiceOfferingViewModel_WhenIEditScheduledServiceOffering_ThenJsonResultYieldsTrue()
        {
            var result = Target.EditScheduledOffering(new StudentServiceOfferingScheduleModel()) as JsonResult;

            result.AssertGetData(true);
        }

        [TestMethod]
        public void GivenViewModelGenerated_WhenIDeleteScheduledServiceOffering_ThenViewModelReturned()
        {
            DeleteServiceOfferingScheduleModel expected = new DeleteServiceOfferingScheduleModel();
            MockLogicManager.Expect(m => m.GenerateDeleteViewModel(User, 1)).Return(expected);

            var result = Target.DeleteScheduledOffering(1) as PartialViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenValidAssignedServiceOfferingId_AndValidStudentId_WhenICallDeleteScheduledOfferingConfirmed_ThenJsonResultYeildsTrue()
        {
            var result = Target.DeleteScheduledOfferingConfirmed(1) as JsonResult;

            Assert.AreEqual(true, result.Data);
        }
    }
}