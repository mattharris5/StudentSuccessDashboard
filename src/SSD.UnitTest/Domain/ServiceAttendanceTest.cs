using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.Domain
{
    [TestClass]
    public class ServiceAttendanceTest
    {
        [TestMethod]
        public void WhenConstruct_ThenCreateTimeSet()
        {
            ServiceAttendance actual = new ServiceAttendance();

            Assert.IsTrue(actual.CreateTime.WithinTimeSpanOf(TimeSpan.FromMilliseconds(20), DateTime.Now));
        }
    }
}
