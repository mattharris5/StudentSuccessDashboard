using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.IO;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSD.Business
{
    [TestClass]
    public class PrivateHealthFieldManagerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private PrivateHealthFieldManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            Target = new PrivateHealthFieldManager(repositoryContainer, MockRepository.GenerateMock<IBlobClient>(), new DataTableBinder(), new UserAuditor());
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

        [TestMethod]
        public void GivenDefaultModel_WhenRetrieveStudentsList_ThenApprovedProvidersIncluded()
        {
            var defaultModel = new StudentProfileExportModel();

            var actual = Target.RetrieveStudentsList(defaultModel);

            Assert.IsTrue(actual.Any(s => s.ApprovedProviders.Count > 0));
        }

        [TestMethod]
        public void GivenDefaultModel_WhenRetrieveStudentsList_ThenStudentAssignedOfferingsDataIncluded()
        {
            var defaultModel = new StudentProfileExportModel();

            var actual = Target.RetrieveStudentsList(defaultModel);

            Assert.IsTrue(actual.Any(s => s.StudentAssignedOfferings.Where(a => a.ServiceOffering.Program != null && a.ServiceOffering.Provider != null && a.ServiceOffering.ServiceType != null).Count() > 0));
        }
    }
}
