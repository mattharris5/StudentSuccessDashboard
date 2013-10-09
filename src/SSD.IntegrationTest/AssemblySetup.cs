using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Data;
using SSD.Repository;

namespace SSD
{
    [TestClass]
    public static class AssemblySetup
    {
        [AssemblyInitialize]
        public static void InitializeAssembly(TestContext testContext)
        {
            using (EducationDataContext context = new EducationDataContext())
            {
                context.Database.Delete();
                context.Database.Initialize(true);
            }
        }

        [AssemblyCleanup]
        public static void CleanupAssembly()
        {
            using (EducationDataContext context = new EducationDataContext())
            {
                context.Database.Delete();
            }
        }

        public static void ForceDeleteEducationDatabase(string databaseName)
        {
            using (EducationDataContext context = new EducationDataContext())
            {
                if (context.Database.Exists())
                {
                    string dropUsersCommand = string.Format("USE [master]; ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", databaseName);
                    context.Database.ExecuteSqlCommand(dropUsersCommand);
                    context.Database.Delete();
                }
            }
        }

        public static WindsorContainer CreateWindsorContainer()
        {
            WindsorContainer container = new WindsorContainer();
            container.Register(Classes.FromAssemblyContaining<IRepository<object>>()
                .InSameNamespaceAs<IRepository<object>>(true)
                .WithServiceDefaultInterfaces()
                .If(t => t.GetInterface("IRepository`1") != null));
            container.Register(Classes.FromAssemblyContaining<IRepositoryContainer>()
                .InSameNamespaceAs<IRepositoryContainer>(true)
                .WithServiceDefaultInterfaces()
                .If(t => t.GetInterface("IRepositoryContainer") != null));
            container.Register(Component.For<IWindsorContainer>().Instance(container));
            return container;
        }

        public static WindsorContainer CreateWindsorContainer(IEducationContext educationDataContext)
        {
            WindsorContainer container = CreateWindsorContainer();
            container.Register(Component.For<IEducationContext>().Instance(educationDataContext));
            return container;
        }
    }
}
