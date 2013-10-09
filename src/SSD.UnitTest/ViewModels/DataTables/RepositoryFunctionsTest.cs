using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSD.ViewModels.DataTables
{
    [TestClass]
    public class RepositoryFunctionsTest
    {
        [TestMethod]
        public void GivenDouble_WhenStringConvert_ThenStringReturned()
        {
            string actual = RepositoryFunctions.StringConvert(20);

            Assert.AreEqual("20", actual);
        }

        [TestMethod]
        public void GivenNull_WhenStringConvert_ThenStringReturned()
        {
            string actual = RepositoryFunctions.StringConvert(null);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenNonMinuteDatePart_WhenDateDiff_ThenExceptionThrown()
        {
            TestExtensions.ExpectException<NotSupportedException>(() => RepositoryFunctions.DateDiff("", null, null));
        }

        [TestMethod]
        public void GivenValidArguments_WhenDateDiff_ThenDifferenceReturned()
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(1);
            int expected = (int)endDate.Subtract(startDate).TotalMinutes;

            int? actual = RepositoryFunctions.DateDiff("mi", startDate, endDate);

            Assert.AreEqual(expected, actual);
        }
    }
}
