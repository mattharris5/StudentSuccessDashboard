using Castle.MicroKernel;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.DependencyInjection
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel _Kernel;

        public WindsorControllerFactory(IKernel kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }
            _Kernel = kernel;
        }

        public override void ReleaseController(IController controller)
        {
            base.ReleaseController(controller);
            _Kernel.ReleaseComponent(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return base.GetControllerInstance(requestContext, controllerType);
            }
            return (IController)_Kernel.Resolve(controllerType);
        }
    }
}