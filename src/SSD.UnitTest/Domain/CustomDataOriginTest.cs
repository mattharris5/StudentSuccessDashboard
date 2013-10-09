using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.Domain
{
    [TestClass]
    public class CustomDataOriginTest
    {
        [TestMethod]
        public void WhenConstruct_ThenCreateTimeSet()
        {
            CustomDataOrigin actual = new CustomDataOrigin();

            Assert.IsTrue(actual.CreateTime.WithinTimeSpanOf(TimeSpan.FromMilliseconds(20), DateTime.Now));
        }
    }
}
