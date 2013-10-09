using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSD.IO
{
    [TestClass]
    public class ExcelParserTest
    {
        private static string UploadFileTemplatePath = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"]);

        [TestInitialize]
        public void InitializeTest()
        {

        }

        [TestMethod]
        public void GivenANullWorkSheetName_WhenIParse_ThenADataTableWillBeReturned()
        {
            var filePath = UploadFileTemplatePath + "NullStartEndDateAndNotesCol.xlsx";
            var destinationPath = CopyTestFile(filePath);

            using(FileStream fs = File.Open(destinationPath, FileMode.Open))
            {
                var dataTable = ExcelParser.ExtractExcelSheetValues(fs, null) as DataTable;

                Assert.IsNotNull(dataTable);
                Assert.IsTrue(dataTable.Rows.Count > 0);
            }

            DestroyTestFile(destinationPath);
        }

        private string CopyTestFile(string currentPath)
        {
            var destinationPath = currentPath.Replace(".xlsx", "-test.xlsx");
            File.Copy(currentPath, destinationPath, true);
            return destinationPath;
        }

        private void DestroyTestFile(string testPath)
        {
            File.Delete(testPath);
        }
    }
}
