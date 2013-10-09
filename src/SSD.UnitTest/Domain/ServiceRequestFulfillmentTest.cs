using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.Domain
{
    [TestClass]
    public class ServiceRequestFulfillmentTest
    {
        [TestMethod]
        public void WhenConstruct_ThenCreateTimeSet()
        {
            ServiceRequestFulfillment actual = new ServiceRequestFulfillment();

            Assert.IsTrue(actual.CreateTime.WithinTimeSpanOf(TimeSpan.FromMilliseconds(20), DateTime.Now));
        }
    }
}
