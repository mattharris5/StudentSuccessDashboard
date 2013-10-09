using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Business;
using SSD.Data;
using SSD.Repository;
using SSD.ViewModels.DataTables;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SSD.Controllers
{
    [TestClass]
    public class PartnersControllerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private PartnersController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            ProviderManager providerManager = new ProviderManager(repositoryContainer, new DataTableBinder());
            ProgramManager programManager = new ProgramManager(repositoryContainer, new DataTableBinder());
            ServiceTypeManager serviceTypeManager = new ServiceTypeManager(repositoryContainer, new DataTableBinder());
            Target = new PartnersController(providerManager, programManager, serviceTypeManager);
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
        public void GivenTermHasMatchingProviderNames_WhenGettingAutocompleteList_ThenListContainsMatches()
        {
            JsonResult result = Target.AutocompletePartnerName("and");

            IEnumerable<string> actual = result.AssertGetData<IEnumerable<string>>();
            CollectionAssert.AreEquivalent(new List<string> { "Boys and Girls Club", "Joe's World-class Tutoring Services and Eatery!" }, actual.ToList());
        }
    }
}
