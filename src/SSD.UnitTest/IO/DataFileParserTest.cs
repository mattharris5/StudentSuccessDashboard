using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;

namespace SSD.IO
{
    [TestClass]
    public class DataFileParserTest
    {
        [TestInitialize]
        public void InitializeTest()
        {

        }

        [TestMethod]
        public void WhenIExtractValues_ThenTheAppropriateNumberOfRowsArePresent()
        {
            var stream = new FileStream(Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/HappyPath.txt"), FileMode.Open);
            
            DataTable actual = DataFileParser.ExtractValues(stream, '\t', 3, 1, 2, 5);

            Assert.AreEqual(3, actual.Rows.Count);
        }

        [TestMethod]
        public void GivenFileHasTwoColumnsInAdditionToStudentId_AndLastIntegerColumnMissingData_AndColumnHasNoDelimiter_WhenExtractValues_ThenRowWithMissingDelimiterHasRowError()
        {
            var stream = new FileStream(Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/LastIntegerColumnMissingValueAndDelimiter.txt"), FileMode.Open);
            
            DataTable actual = DataFileParser.ExtractValues(stream, '\t', 3, 1, 2, 5);

            DataRow errorRow = actual.GetErrors().SingleOrDefault();
            Assert.IsNotNull(errorRow);
            Assert.AreEqual("40", errorRow[1]);
            Assert.AreEqual("Incorrect number of columns.", errorRow.RowError);
        }
    }
}
