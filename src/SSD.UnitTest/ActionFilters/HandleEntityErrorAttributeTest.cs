using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using System;
using System.Web;
using System.Web.Mvc;

namespace SSD.ActionFilters
{
    [TestClass]
    public class HandleEntityErrorAttributeTest
    {
        private HttpContextBase MockHttpContext { get; set; }
        private HandleEntityErrorAttribute Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockHttpContext = MockHttpContextFactory.Create();
            Target = new HandleEntityErrorAttribute();
        }

        [TestMethod]
        public void GivenNullExceptionContext_WhenOnException_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.OnException(null));
        }

        [TestMethod]
        public void GivenExceptionIsEntityNotFoundException_WhenOnException_ThenHttpNotFound()
        {
            EntityNotFoundException exception = new EntityNotFoundException();
            ExceptionContext context = ControllerContextFactory.CreateExceptionContext(MockHttpContext, exception);

            Target.OnException(context);

            HttpNotFoundResult actual = context.Result as HttpNotFoundResult;
            Assert.IsNotNull(actual);
            MockHttpContext.Response.AssertWasCalled(m => m.StatusCode = 404);
        }

        [TestMethod]
        public void GivenExceptionIsEntityNotFoundException_WhenOnException_ThenHttpDescriptionIsExceptionMessage()
        {
            string expected = "blah blah exception message and stuff";
            EntityNotFoundException exception = new EntityNotFoundException(expected);
            ExceptionContext context = ControllerContextFactory.CreateExceptionContext(MockHttpContext, exception);

            Target.OnException(context);

            HttpStatusCodeResult actual = context.Result as HttpStatusCodeResult;
            Assert.AreEqual(expected, actual.StatusDescription);
        }

        [TestMethod]
        public void GivenExceptionHasBeenHandled_AndExceptionIsEntityNotFoundException_WhenOnException_ThenResultNull_AndStatusNotChanged()
        {
            EntityNotFoundException exception = new EntityNotFoundException();
            ExceptionContext context = ControllerContextFactory.CreateExceptionContext(MockHttpContext, exception);
            context.ExceptionHandled = true;

            Target.OnException(context);

            Assert.IsInstanceOfType(context.Result, typeof(EmptyResult));
            MockHttpContext.Response.AssertWasNotCalled(m => m.StatusCode = Arg<int>.Is.Anything);
        }

        [TestMethod]
        public void GivenResponseHasBeenWrittenTo_ExceptionIsEntityNotFoundException_WhenOnException_ThenResponseHasBeenCleared()
        {
            EntityNotFoundException exception = new EntityNotFoundException();
            ExceptionContext context = ControllerContextFactory.CreateExceptionContext(MockHttpContext, exception);

            Target.OnException(context);

            MockHttpContext.Response.AssertWasCalled(m => m.Clear());
        }

        [TestMethod]
        public void GivenExceptionIsEntityNotFoundException_WhenOnException_ThenExceptionHandled()
        {
            EntityNotFoundException exception = new EntityNotFoundException();
            ExceptionContext context = ControllerContextFactory.CreateExceptionContext(MockHttpContext, exception);

            Target.OnException(context);

            Assert.IsTrue(context.ExceptionHandled);
        }

        [TestMethod]
        public void GivenExceptionIsEntityAccessUnauthorizedException_WhenOnException_ThenHttpUnauthorized()
        {
            EntityAccessUnauthorizedException exception = new EntityAccessUnauthorizedException();
            ExceptionContext context = ControllerContextFactory.CreateExceptionContext(MockHttpContext, exception);

            Target.OnException(context);

            HttpUnauthorizedResult actual = context.Result as HttpUnauthorizedResult;
            Assert.IsNotNull(actual);
            MockHttpContext.Response.AssertWasCalled(m => m.StatusCode = 401);
        }

        [TestMethod]
        public void GivenExceptionIsEntityAccessUnauthorizedException_WhenOnException_ThenHttpDescriptionIsExceptionMessage()
        {
            string expected = "blah blah exception message and stuff";
            EntityAccessUnauthorizedException exception = new EntityAccessUnauthorizedException(expected);
            ExceptionContext context = ControllerContextFactory.CreateExceptionContext(MockHttpContext, exception);

            Target.OnException(context);

            HttpStatusCodeResult actual = context.Result as HttpStatusCodeResult;
            Assert.AreEqual(expected, actual.StatusDescription);
        }

        [TestMethod]
        public void GivenExceptionHasBeenHandled_AndExceptionIsEntityAccessUnauthorizedException_WhenOnException_ThenResultNull_AndStatusNotChanged()
        {
            EntityAccessUnauthorizedException exception = new EntityAccessUnauthorizedException();
            ExceptionContext context = ControllerContextFactory.CreateExceptionContext(MockHttpContext, exception);
            context.ExceptionHandled = true;

            Target.OnException(context);

            Assert.IsInstanceOfType(context.Result, typeof(EmptyResult));
            MockHttpContext.Response.AssertWasNotCalled(m => m.StatusCode = Arg<int>.Is.Anything);
        }

        [TestMethod]
        public void GivenResponseHasBeenWrittenTo_ExceptionIsEntityAccessUnauthorizedException_WhenOnException_ThenResponseHasBeenCleared()
        {
            EntityAccessUnauthorizedException exception = new EntityAccessUnauthorizedException();
            ExceptionContext context = ControllerContextFactory.CreateExceptionContext(MockHttpContext, exception);

            Target.OnException(context);

            MockHttpContext.Response.AssertWasCalled(m => m.Clear());
        }

        [TestMethod]
        public void GivenExceptionIsEntityAccessUnauthorizedException_WhenOnException_ThenExceptionHandled()
        {
            EntityAccessUnauthorizedException exception = new EntityAccessUnauthorizedException();
            ExceptionContext context = ControllerContextFactory.CreateExceptionContext(MockHttpContext, exception);

            Target.OnException(context);

            Assert.IsTrue(context.ExceptionHandled);
        }

        [TestMethod]
        public void GivenExceptionIsUnreserved_AndCustomErrorsEnabled_WhenOnException_ThenDefaultToGeneralServerError()
        {
            Exception exception = new Exception();
            ExceptionContext context = ControllerContextFactory.CreateExceptionContext(MockHttpContext, exception);
            MockHttpContext.Expect(m => m.IsCustomErrorEnabled).Return(true);

            Target.OnException(context);

            MockHttpContext.Response.AssertWasCalled(m => m.StatusCode = 500);
        }
    }
}
