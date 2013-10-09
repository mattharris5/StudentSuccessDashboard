using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Data;
using SSD.Repository;
using System.Linq;

namespace SSD.Business
{
    [TestClass]
    public class WidgetManagerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private WidgetManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            Target = new WidgetManager(repositoryContainer);
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
        public void WhenGenerateServiceRequestsBySchoolModel_ThenFieldsAreCalculated()
        {
            var actual = Target.GenerateServiceRequestsBySchoolModel();

            Assert.IsTrue(actual.Count() > 0);
            Assert.AreEqual(actual.ElementAt(0).Total, actual.ElementAt(0).Open + actual.ElementAt(0).Fulfilled);
            Assert.AreEqual(actual.ElementAt(1).Total, actual.ElementAt(1).Open + actual.ElementAt(1).Fulfilled);
            Assert.AreEqual(actual.ElementAt(2).Total, actual.ElementAt(2).Open + actual.ElementAt(2).Fulfilled);
        }
    }
}
