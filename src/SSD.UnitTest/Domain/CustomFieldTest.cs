using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.Domain
{
    [TestClass]
    public class CustomFieldTest
    {
        [TestMethod]
        public void WhenConstruct_ThenCreateTimeSet()
        {
            PublicField actual = new PublicField();

            Assert.IsTrue(actual.CreateTime.WithinTimeSpanOf(TimeSpan.FromMilliseconds(20), DateTime.Now));
        }

        [TestMethod]
        public void WhenConstruct_ThenCategoriesNotNull()
        {
            var actual = new PublicField();

            Assert.IsNotNull(actual.Categories);
        }
    }
}
