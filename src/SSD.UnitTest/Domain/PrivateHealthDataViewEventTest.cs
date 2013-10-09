using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.Domain
{
    [TestClass]
    public class PrivateHealthDataViewEventTest
    {
        [TestMethod]
        public void WhenConstruct_ThenCreateTimeSet()
        {
            PrivateHealthDataViewEvent actual = new PrivateHealthDataViewEvent();

            Assert.IsTrue(actual.CreateTime.WithinTimeSpanOf(TimeSpan.FromMilliseconds(20), DateTime.Now));
        }
    }
}
