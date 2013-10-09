using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.Domain
{
    [TestClass]
    public class UserRoleTest
    {
        [TestMethod]
        public void WhenConstruct_ThenCreateTimeSet()
        {
            UserRole actual = new UserRole();

            Assert.IsTrue(actual.CreateTime.WithinTimeSpanOf(TimeSpan.FromMilliseconds(20), DateTime.Now));
        }

        [TestMethod]
        public void WhenConstruct_ThenSchoolsNotNull()
        {
            UserRole target = new UserRole();

            Assert.IsNotNull(target.Schools);
        }

        [TestMethod]
        public void WhenConstruct_ThenProvidersNotNull()
        {
            UserRole target = new UserRole();

            Assert.IsNotNull(target.Providers);
        }
    }
}
