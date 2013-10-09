using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Controllers;
using SSD.Domain;
using SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace SSD.IO
{
    [TestClass]
    public class WorksheetWriterTest
    {
        private WorksheetWriter Target { get; set; }
        private TestData TestData { get; set; }
        private ServiceOffering ServiceOffering { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            TestData = new TestData();
            ServiceOffering = new ServiceOffering() { Id = 1, Provider = TestData.Providers[0], ServiceType = TestData.ServiceTypes[0], Program = TestData.Programs[0] };
            Target = new WorksheetWriter(ServiceOffering, "Assign Service Offering");
        }

        [TestMethod]
        public void GivenNullServiceOffering_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new WorksheetWriter(null, "blah"));
        }

        [TestMethod]
        public void GivenNullWorksheetPart_WhenCreateHeader_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.CreateHeader(null));
        }

        [TestMethod]
        public void GivenWorksheet_WhenCreateHeader_ThenCorrectHeaderCreated()
        {
            byte[] byteArray = File.ReadAllBytes(Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "Templates/" + ServiceOfferingController.TemplateFile));
            MemoryStream stream = new MemoryStream(byteArray);
            WorksheetPart actual;
            using (SpreadsheetDocument spreadsheetDoc = SpreadsheetDocument.Open(stream, true))
            {
                actual = ExcelUtility.GetWorksheetPartByName(spreadsheetDoc, Target.SheetName);
                Target.CreateHeader(actual);
            }

            Assert.AreEqual(GetCell(actual.Worksheet, "B", 2).CellValue.InnerText, ServiceOffering.Id.ToString());
            Assert.AreEqual(GetCell(actual.Worksheet, "C", 2).CellValue.InnerText, ServiceOffering.Name);
        }

        [TestMethod]
        public void GivenWorksheetWithFourColumns_WhenCreateErrorRows_ThenCorrectRowsAreCreated()
        {
            byte[] byteArray = File.ReadAllBytes(Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "Templates/" + ServiceOfferingController.TemplateFile));
            MemoryStream stream = new MemoryStream(byteArray);
            WorksheetPart actual;
            using (SpreadsheetDocument spreadsheetDoc = SpreadsheetDocument.Open(stream, true))
            {
                actual = ExcelUtility.GetWorksheetPartByName(spreadsheetDoc, Target.SheetName);
                Target.ErrorRows.Add(new FileRowModel { RowErrors = new List<string> { "1", "1/1/1900", "1/1/1999", "Notes" } });
                Target.CreateErrorRows(actual);
            }
            Assert.AreEqual(GetCell(actual.Worksheet, "B", 4).CellValue.InnerText, "1");
            Assert.AreEqual(GetCell(actual.Worksheet, "C", 4).CellValue.InnerText, "1/1/1900");
            Assert.AreEqual(GetCell(actual.Worksheet, "D", 4).CellValue.InnerText, "1/1/1999");
            Assert.AreEqual(GetCell(actual.Worksheet, "E", 4).CellValue.InnerText, "Notes");
        }

        [TestMethod]
        public void GivenWorksheetWithFiveColumns_WhenCreateErrorRows_ThenCorrectRowsAreCreated()
        {
            byte[] byteArray = File.ReadAllBytes(Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "Templates/" + ServiceOfferingController.TemplateFile));
            MemoryStream stream = new MemoryStream(byteArray);
            WorksheetPart actual;
            using (SpreadsheetDocument spreadsheetDoc = SpreadsheetDocument.Open(stream, true))
            {
                actual = ExcelUtility.GetWorksheetPartByName(spreadsheetDoc, Target.SheetName);
                Target.ErrorRows.Add(new FileRowModel { RowErrors = new List<string> { "1", "1/1/1900", "Subject", "12", "Notes" } });
                Target.CreateErrorRows(actual);
            }
            Assert.AreEqual(GetCell(actual.Worksheet, "B", 4).CellValue.InnerText, "1");
            Assert.AreEqual(GetCell(actual.Worksheet, "C", 4).CellValue.InnerText, "1/1/1900");
            Assert.AreEqual(GetCell(actual.Worksheet, "D", 4).CellValue.InnerText, "Subject");
            Assert.AreEqual(GetCell(actual.Worksheet, "E", 4).CellValue.InnerText, "12");
            Assert.AreEqual(GetCell(actual.Worksheet, "F", 4).CellValue.InnerText, "Notes");
        }

        private static Cell GetCell(Worksheet worksheet, string columnName, uint rowIndex)
        {
            Row row = GetRow(worksheet, rowIndex);
            return row.Elements<Cell>().Where(c => string.Compare
                   (c.CellReference.Value, columnName +
                   rowIndex, true) == 0).First();
        }

        // Given a worksheet and a row index, return the row.
        private static Row GetRow(Worksheet worksheet, uint rowIndex)
        {
            return worksheet.GetFirstChild<SheetData>().Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
        }
    }
}
