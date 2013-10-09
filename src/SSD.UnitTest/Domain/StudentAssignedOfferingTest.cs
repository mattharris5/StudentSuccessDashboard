using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.Domain
{
    [TestClass]
    public class StudentAssignedOfferingTest
    {
        [TestMethod]
        public void WhenConstruct_ThenCreateTimeSet()
        {
            StudentAssignedOffering actual = new StudentAssignedOffering();

            Assert.IsTrue(actual.CreateTime.WithinTimeSpanOf(TimeSpan.FromMilliseconds(20), DateTime.Now));
        }
    }
}
