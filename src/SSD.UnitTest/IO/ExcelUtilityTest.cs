using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SSD.IO
{
    [TestClass]
    public class ExcelUtilityTest
    {
        [TestMethod]
        public void GivenNullDocument_WhenGetWorksheetPartByName_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => ExcelUtility.GetWorksheetPartByName(null, ""));
        }

        [TestMethod]
        public void Given26_WhenGetColumnNameFromIndex_ThenZ()
        {
            string expected = "Z";

            string actual = ExcelUtility.GetColumnNameFromIndex(26);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Given27_WhenGetColumnNameFromIndex_ThenAA()
        {
            string expected = "AA";

            string actual = ExcelUtility.GetColumnNameFromIndex(27);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Given52_WhenGetColumnNameFromIndex_ThenAZ()
        {
            string expected = "AZ";

            string actual = ExcelUtility.GetColumnNameFromIndex(52);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Given53_WhenGetColumnNameFromIndex_ThenBA()
        {
            string expected = "BA";

            string actual = ExcelUtility.GetColumnNameFromIndex(53);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WhenCloneCell_ThenCellIsCloned()
        {
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (SpreadsheetDocument spreadsheetDoc = SpreadsheetDocument.Open(stream, true))
                {
                    spreadsheetDoc.ChangeDocumentType(SpreadsheetDocumentType.Workbook);
                    Worksheet worksheet = spreadsheetDoc.WorkbookPart.WorksheetParts.First().Worksheet;

                    ExcelUtility.CopyCell(worksheet, "D", 2, "E", 2);
                }
                File.WriteAllBytes(outputPath, stream.ToArray());
            }

            using (XLWorkbook workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                IXLCell originalCell = worksheet.Cell(2, "D");
                IXLCell clonedCell = worksheet.Cell(2, "E");
                Assert.AreEqual(originalCell.DataType, clonedCell.DataType);
                Assert.AreEqual(originalCell.FormulaA1, clonedCell.FormulaA1);
                Assert.AreEqual(originalCell.FormulaR1C1, clonedCell.FormulaR1C1);
                Assert.AreEqual(originalCell.RichText.Text, clonedCell.RichText.Text);
                Assert.AreEqual(originalCell.ShareString, clonedCell.ShareString);
                Assert.AreEqual(originalCell.Style, clonedCell.Style);
                Assert.AreEqual(originalCell.Value, clonedCell.Value);
                Assert.AreEqual(originalCell.ValueCached, clonedCell.ValueCached);
            }
        }
    }
}
