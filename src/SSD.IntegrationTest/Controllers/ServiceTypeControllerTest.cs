using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Business;
using SSD.Data;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class ServiceTypeControllerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private ServiceTypeManager LogicManager { get; set; }
        private EducationSecurityPrincipal User { get; set; }
        private ServiceTypeController Target { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            LogicManager = new ServiceTypeManager(repositoryContainer, new DataTableBinder());
            HttpContext context = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));
            HttpContext.Current = context;
            User = new EducationSecurityPrincipal(new UserRepository(EducationContext).Items.Where(s => s.UserKey == "Bob").Single());
            context.User = User;
            Target = new ServiceTypeController(LogicManager);
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
        public void WhenManageIsCalled_ThenAViewWithAListOfCategoriesIsReturned()
        {
            var expected = new List<string> 
            { 
                "Basic Needs", "Consumer Services", "Criminal Justice and Legal Services", "Education", "Environmental Quality",
                "Health Care", "Income Support and Employment", "Individual and Family Life", "Mental Health Care and Counseling",
                "Organizational/Community Services", "Support Groups", "Target Populations", "Test Category," 
            };

            ViewResult result = Target.Index();

            ServiceTypeListOptionsModel model = result.AssertGetViewModel<ServiceTypeListOptionsModel>();
            CollectionAssert.AreEquivalent(expected, model.CategoryFilterList.ToList());
        }
    }
}
