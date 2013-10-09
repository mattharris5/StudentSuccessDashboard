using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SSD.Domain
{
    [TestClass]
    public class ServiceTypeTest
    {
        [TestMethod]
        public void WhenServiceTypeConstructed_ThenCategoriesIsNotNull()
        {
            Assert.IsNotNull(new ServiceType().Categories);
        }

        [TestMethod]
        public void WhenServiceTypeConstructed_ThenServiceOfferingsIsNotNull()
        {
            Assert.IsNotNull(new ServiceType().ServiceOfferings);
        }
    }
}
