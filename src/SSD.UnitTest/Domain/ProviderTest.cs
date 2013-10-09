using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SSD.Domain
{
    [TestClass]
    public class ProviderTest
    {
        [TestMethod]
        public void WhenProviderConstructed_ThenAddressIsNotNull()
        {
            Assert.IsNotNull(new Provider().Address);
        }

        [TestMethod]
        public void WhenProviderConstructed_ThenContactIsNotNull()
        {
            Assert.IsNotNull(new Provider().Contact);
        }

        [TestMethod]
        public void WhenProviderConstructed_ThenApprovingStudentsIsNotNull()
        {
            Assert.IsNotNull(new Provider().ApprovingStudents);
        }

        [TestMethod]
        public void WhenProviderConstructed_ThenUserRolesIsNotNull()
        {
            Assert.IsNotNull(new Provider().UserRoles);
        }

        [TestMethod]
        public void WhenProviderConstructed_ThenServiceOfferingsIsNotNull()
        {
            Assert.IsNotNull(new Provider().ServiceOfferings);
        }
    }
}
