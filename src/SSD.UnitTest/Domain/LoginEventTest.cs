using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.Domain
{
    [TestClass]
    public class LoginEventTest
    {
        [TestMethod]
        public void WhenConstruct_ThenCreateTimeSet()
        {
            LoginEvent actual = new LoginEvent();

            Assert.IsTrue(actual.CreateTime.WithinTimeSpanOf(TimeSpan.FromMilliseconds(20), DateTime.Now));
        }
    }
}
