using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Business;
using SSD.Data;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class ServiceControllerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private EducationSecurityPrincipal User { get; set; }
        private ServiceController Target {get; set;}

        [TestInitialize]
        public void TestInitialize()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            ScheduledServiceManager logicManager = new ScheduledServiceManager(repositoryContainer);
            HttpContext context = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));
            User = new EducationSecurityPrincipal(new UserRepository(EducationContext).Items.Where(s => s.UserKey == "Bob").Single());
            context.User = User;
            Target = new ServiceController(logicManager);
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
        public void GivenUserIsDataAdmin_WhenDeleteScheduledOffering_ThenViewModelReturnsServiceOfferingNameSuccessfully()
        {
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } });

            var result = Target.DeleteScheduledOffering(2) as PartialViewResult;

            DeleteServiceOfferingScheduleModel model = result.AssertGetViewModel<DeleteServiceOfferingScheduleModel>();
            Assert.IsNotNull(model.Name);
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndUserHasFavorites_AndStudentIdsAreValid_WhenIScheduleOffering_ThenFavoritesAreLoaded()
        {
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } });
            Target.TempData["ScheduleOfferingIds"] = new int[] { 1 };
            Target.TempData["ScheduleOfferingReturnUrl"] = "/student/finder";

            var result = Target.ScheduleOffering() as ViewResult;

            ScheduleServiceOfferingListOptionsModel viewModel = result.AssertGetViewModel<ScheduleServiceOfferingListOptionsModel>();
            List<ServiceOffering> actual = viewModel.Favorites.ToList();
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Big Brothers, Big Sisters", actual[0].Provider.Name);
            Assert.AreEqual("Provide College Access", actual[0].ServiceType.Name);
        }
    }
}
