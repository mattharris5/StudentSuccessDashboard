using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Business;
using SSD.Data;
using SSD.Repository;
using SSD.ViewModels.DataTables;

namespace SSD.Controllers
{
    [TestClass]
    public class ProviderControllerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private ProviderController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            ProviderManager manager = new ProviderManager(repositoryContainer, new DataTableBinder());
            Target = new ProviderController(manager);
        }

        [TestCleanup]
        public void CleanupTest()
        {
            if (Container != null)
            {
                Container.Dispose();
            }
            if (EducationContext != null)
            {
                EducationContext.Dispose();
            }
        }
    }
}
