using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.Domain
{
    [TestClass]
    public class ServiceRequestTest
    {
        [TestMethod]
        public void WhenConstruct_ThenCreateTimeSet()
        {
            ServiceRequest actual = new ServiceRequest();

            Assert.IsTrue(actual.CreateTime.WithinTimeSpanOf(TimeSpan.FromMilliseconds(20), DateTime.Now));
        }
    }
}
