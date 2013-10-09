using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SSD.DependencyInjection
{
    public class WindsorDependencyResolver : IDependencyResolver
    {
        private readonly IWindsorContainer _Container;

        public WindsorDependencyResolver(IWindsorContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            _Container = container;
        }

        public object GetService(Type serviceType)
        {
            if (_Container.Kernel.HasComponent(serviceType))
            {
                return _Container.Resolve(serviceType);
            }
            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (_Container.Kernel.HasComponent(serviceType))
            {
                return _Container.ResolveAll(serviceType).Cast<object>();
            }
            return new object[] { };
        }
    }

}