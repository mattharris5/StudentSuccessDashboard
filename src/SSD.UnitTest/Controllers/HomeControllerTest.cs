using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;

namespace SSD.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        private HomeController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new HomeController();
        }

        [TestMethod]
        public void WhenIndexActionIsCalled_ThenAViewResultIsCreated()
        {
            ViewResult actual = Target.Index();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void WhenAboutActionIsCalled_ThenAViewResultIsCreated()
        {
            ViewResult actual = Target.About();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void WhenTermsActionIsCalled_ThenAViewResultIsCreated()
        {
            ViewResult actual = Target.Terms();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void WhenPrivacyActionIsCalled_ThenAViewResultIsCreated()
        {
            ViewResult actual = Target.Privacy();

            Assert.IsNotNull(actual);
        }
    }
}
