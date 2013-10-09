using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.Data;
using SSD.Domain;
using SSD.IO;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class ServiceOfferingControllerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private EducationSecurityPrincipal User { get; set; }
        private ServiceOfferingController Target { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            ServiceOfferingManager logicManager = new ServiceOfferingManager(repositoryContainer, new DataTableBinder());
            ServiceTypeManager serviceTypeManager = new ServiceTypeManager(repositoryContainer, new DataTableBinder());
            ProviderManager providerManager = new ProviderManager(repositoryContainer, new DataTableBinder());
            ProgramManager programManager = new ProgramManager(repositoryContainer, new DataTableBinder());
            HttpContext context = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));
            User userEntity = EducationContext.Users.Include("UserRoles.Role").Include("UserRoles.Schools").Include("UserRoles.Providers").Single(s => s.UserKey == "Bob");
            User = new EducationSecurityPrincipal(userEntity);
            context.User = User;
            Target = new ServiceOfferingController(logicManager, serviceTypeManager, providerManager, programManager, MockRepository.GenerateMock<IFileProcessor>());
            ControllerContext controllerContext = new ControllerContext(new HttpContextWrapper(context), new RouteData(), Target);
            Target.ControllerContext = controllerContext;
        }

        [TestCleanup]
        public void TestCleanup()
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
        public void GivenUserHasFavorites_WhenIManage_ThenFavoritesAreLoaded()
        {
            ViewResult result = Target.Index();

            ServiceOfferingListOptionsModel viewModel = result.AssertGetViewModel<ServiceOfferingListOptionsModel>();
            List<ServiceOffering> actual = viewModel.Favorites.ToList();
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Big Brothers, Big Sisters", actual[0].Provider.Name);
            Assert.AreEqual("Provide College Access", actual[0].ServiceType.Name);
        }

        [TestMethod]
        public void GivenTermHasMatchingNames_WhenGettingAutocompleteList_ThenListContainsMatches()
        {
            JsonResult result = Target.AutocompleteServiceTypeProviderOrProgram("as");

            IEnumerable<string> actual = result.AssertGetData<IEnumerable<string>>();
            CollectionAssert.AreEquivalent(new List<string> { "After School Basketball", "Joe's World-class Tutoring Services and Eatery!" }, actual.ToList());
        }

        private void CreateFavoriteServiceOfferingInDatabase(int serviceOfferingId)
        {
            using (EducationDataContext context = new EducationDataContext())
            {
                User currentUser = context.Users.Single(u => u.UserKey == User.Identity.User.UserKey);
                ServiceOffering offeringToLink = context.ServiceOfferings.Single(o => o.Id == serviceOfferingId);
                currentUser.FavoriteServiceOfferings.Add(offeringToLink);
                offeringToLink.UsersLinkingAsFavorite.Add(currentUser);
                context.SaveChanges();
            }
        }
    }
}
