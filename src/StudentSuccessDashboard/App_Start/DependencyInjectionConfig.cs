using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using SSD.DependencyInjection;
using System;
using System.Web.Mvc;

namespace SSD
{
    public static class DependencyInjectionConfig
    {
        private static IWindsorContainer _Container;
        private static IControllerFactory _OriginalControllerFactory;
        private static IDependencyResolver _OriginalDependencyResolver;

        public static string AssemblySearchPath { get; set; }

        public static void RegisterDependencyInjection()
        {
            _Container = CreateContainer();
            InitializeControllerBuilder(_Container);
            InitializeDependencyResolver();
        }

        public static void ReleaseDependencyInjection()
        {
            ControllerBuilder.Current.SetControllerFactory(_OriginalControllerFactory);
            DependencyResolver.SetResolver(_OriginalDependencyResolver);
            _Container.Dispose();
            _Container = null;
        }

        private static IWindsorContainer CreateContainer()
        {
            if (string.IsNullOrWhiteSpace(AssemblySearchPath))
            {
                throw new InvalidOperationException("AssemblySearchPath must be set to a valid directory path.");
            }
            var container = new WindsorContainer().Install(FromAssembly.InDirectory(new AssemblyFilter(AssemblySearchPath, "SSD.*"))).Install(FromAssembly.This());
            return container;
        }

        private static void InitializeControllerBuilder(IWindsorContainer container)
        {
            var controllerFactory = new WindsorControllerFactory(container.Kernel);
            _OriginalControllerFactory = ControllerBuilder.Current.GetControllerFactory();
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

        private static void InitializeDependencyResolver()
        {
            _OriginalDependencyResolver = DependencyResolver.Current;
            DependencyResolver.SetResolver(new WindsorDependencyResolver(_Container));
        }
    }
}