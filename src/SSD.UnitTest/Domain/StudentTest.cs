using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SSD.Domain
{
    [TestClass]
    public class StudentTest
    {
        [TestMethod]
        public void WhenIConstruct_ThenAssignedOfferingListIsNotNull()
        {
            Assert.IsNotNull(new Student().StudentAssignedOfferings);
        }

        [TestMethod]
        public void WhenIConstruct_ThenClassListIsNotNull()
        {
            Assert.IsNotNull(new Student().Classes);
        }

        [TestMethod]
        public void WhenIConstruct_ThenServiceRequestListIsNotNull()
        {
            Assert.IsNotNull(new Student().ServiceRequests);
        }

        [TestMethod]
        public void WhenIConstruct_ThenApprovedProviderListIsNotNull()
        {
            Assert.IsNotNull(new Student().ApprovedProviders);
        }

        [TestMethod]
        public void WhenGetFullName_ThenFullNameIsFormattedCorrectly()
        {
            Student target = new Student { FirstName = "Bob", LastName = "Smith", MiddleName = "Allen" };

            Assert.AreEqual("Smith, Bob Allen", target.FullName);
        }
    }
}
