using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SSD.Domain
{
    [TestClass]
    public class ServiceOfferingTest
    {
        [TestMethod]
        public void WhenConstruct_ThenUsersLinkingAsFavoriteNotNull()
        {
            ServiceOffering target = new ServiceOffering();
            Assert.IsNotNull(target.UsersLinkingAsFavorite);
        }

        [TestMethod]
        public void WhenConstruct_ThenAssignedOfferingsNotNull()
        {
            ServiceOffering target = new ServiceOffering();
            Assert.IsNotNull(target.StudentAssignedOfferings);
        }

        [TestMethod]
        public void GivenProviderSet_AndServiceTypeSet_AndProgramSet_WhenGetName_ThenProgramIsAlsoConcatenated()
        {
            ServiceOffering target = new ServiceOffering
            {
                Provider = new Provider { Name = "Bob" },
                ServiceType = new ServiceType { Name = "Tutoring" },
                Program = new Program { Name = "Test Program"}
            };
            Assert.AreEqual("Bob Tutoring/Test Program", target.Name);
        }
    }
}
