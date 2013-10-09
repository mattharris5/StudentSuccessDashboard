using Castle.MicroKernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.DependencyInjection
{
    [TestClass]
    public class WindsorControllerFactoryTest
    {
        private IKernel Kernel { get; set; }
        private TestWindsorControllerFactory Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Kernel = MockRepository.GenerateMock<IKernel>();
            Target = new TestWindsorControllerFactory(Kernel);
        }

        [TestMethod]
        public void GivenNullKernel_WhenIConstruct_ThenThrowArgumentNullException()
        {
            Target.ExpectException<ArgumentNullException>(() => new WindsorControllerFactory(null));
        }

        [TestMethod]
        public void GivenNullType_WhenICreateController_ThenThrowHttpException()
        {
            RequestContext request = new RequestContext(MockHttpContextFactory.Create(), new RouteData());

            Target.ExpectException<HttpException>(() => Target.ExecuteGetControllerInstance(request, null));
        }

        [TestMethod]
        public void GivenIControllerType_WhenICreateController_ThenContructTheType()
        {
            RequestContext request = new RequestContext(MockHttpContextFactory.Create(), new RouteData());
            IController expected = new TestController();
            Type controllerType = expected.GetType();
            Kernel.Expect(m => m.Resolve(controllerType)).Return(expected);
            IController actual = Target.ExecuteGetControllerInstance(request, controllerType);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WhenIReleaseController_ThenKernelReleasesComponent()
        {
            IController expectedReleased = new TestController();
            Target.ReleaseController(expectedReleased);
            Kernel.AssertWasCalled(m => m.ReleaseComponent(expectedReleased));
        }

        private class TestController : Controller
        { }

        private class TestWindsorControllerFactory : WindsorControllerFactory
        {
            public TestWindsorControllerFactory(IKernel kernel)
                : base(kernel)
            { }

            public IController ExecuteGetControllerInstance(RequestContext requestContext, Type controllerType)
            {
                return GetControllerInstance(requestContext, controllerType);
            }
        }
    }
}
