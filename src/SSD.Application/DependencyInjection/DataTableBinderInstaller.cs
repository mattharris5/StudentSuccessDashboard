using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SSD.ViewModels.DataTables;
using System;

namespace SSD.DependencyInjection
{
    public class DataTableBinderInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            container.Register(Component.For<IDataTableBinder>()
                .ImplementedBy<DataTableBinder>()
                .LifestyleSingleton());
        }
    }
}
