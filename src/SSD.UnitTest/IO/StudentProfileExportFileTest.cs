using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SSD.IO
{
    [TestClass]
    public class StudentProfileExportFileTest
    {
        [TestMethod]
        public void GivenCreateNotCalled_WhenSetupColumnHeaders_ThenThrowException()
        {
            using (var target = new StudentProfileExportFile())
            {
                target.ExpectException<InvalidOperationException>(() => target.SetupColumnHeaders(new[] { "whatever" }));
            }
        }

        [TestMethod]
        public void GivenNullColumnNames_WhenSetupColumnHeaders_ThenThrowException()
        {
            using (var target = new StudentProfileExportFile())
            {
                target.ExpectException<ArgumentNullException>(() => target.SetupColumnHeaders(null));
            }
        }

        [TestMethod]
        public void GivenCreateCalled_WhenSetupColumnHeaders_ThenSucceed()
        {
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.SetupColumnHeaders(new[] { "whatever" });
                }
            }
        }

        [TestMethod]
        public void GivenCreateCalled_AndHeaderTextProvided_WhenSetupColumnHeaders_ThenSavedFileContainsHeaderText()
        {
            string expectedHeaderText = "whatever";
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.SetupColumnHeaders(new[] { expectedHeaderText });
                }

                File.WriteAllBytes(outputPath, stream.ToArray());
            }
            using (XLWorkbook workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                IXLCell actualE2Cell = worksheet.Cell(2, "A");
                Assert.AreEqual(expectedHeaderText, actualE2Cell.Value);
            }
        }

        [TestMethod]
        public void GivenCreateCalled_AndListOfHeaderNamesProvided_WhenSetupColumnHeaders_ThenSavedFileContainsEachHeaderName()
        {
            string expectedE2Column = "Test";
            string expectedF2Column = "Column";
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.SetupColumnHeaders(new[] { expectedE2Column, expectedF2Column });
                }

                File.WriteAllBytes(outputPath, stream.ToArray());
            }
            using (XLWorkbook workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                IXLCell actualE2Cell = worksheet.Cell(2, "A");
                IXLCell actualF2Cell = worksheet.Cell(2, "B");
                Assert.AreEqual(expectedE2Column, actualE2Cell.Value);
                Assert.AreEqual(expectedF2Column, actualF2Cell.Value);
            }
        }

        [TestMethod]
        public void GivenCreateCalled_AndHeaderTextProvided_WhenSetupColumnHeaders_ThenSavedFileContainsAddedHeaderCellWithFormatting()
        {
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.SetupColumnHeaders(new[] { "whatever", "to be formatted" });
                }

                File.WriteAllBytes(outputPath, stream.ToArray());
            }
            using (XLWorkbook workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                IXLCell preexistingHeaderCell = worksheet.Cell(2, "A");
                IXLCell actualAddedHeaderCell = worksheet.Cell(2, "B");
                Assert.AreEqual(preexistingHeaderCell.Style, actualAddedHeaderCell.Style);
            }
        }

        [TestMethod]
        public void GivenCreateNotCalled_WhenFillData_ThenThrowException()
        {
            using (var target = new StudentProfileExportFile())
            {
                target.ExpectException<InvalidOperationException>(() => target.FillData(new List<IEnumerable<object>> { new[] { new object() } }));
            }
        }

        [TestMethod]
        public void GivenNullObjects_WhenFillData_ThenThrowException()
        {
            using (var target = new StudentProfileExportFile())
            {
                target.ExpectException<ArgumentNullException>(() => target.FillData(null));
            }
        }

        [TestMethod]
        public void GivenCreateCalled_WhenFillData_ThenSuceed()
        {
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.FillData(new List<IEnumerable<object>> { new[] { new object() } });
                }
            }
        }

        [TestMethod]
        public void GivenCreateCalled_AndStringDataRows_WhenFillData_ThenFileContainsDataInColumns()
        {
            var data = new List<List<object>>
            {
                new List<object> { "bob", "smith" },
                new List<object> { "george", "jones" }
            };
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.FillData(data);
                }

                File.WriteAllBytes(outputPath, stream.ToArray());
            }
            using (XLWorkbook workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                Assert.AreEqual(data[0][0], worksheet.Cell("A3").Value);
                Assert.AreEqual(data[0][1], worksheet.Cell("B3").Value);
                Assert.AreEqual(data[1][0], worksheet.Cell("A4").Value);
                Assert.AreEqual(data[1][1], worksheet.Cell("B4").Value);
            }
        }

        [TestMethod]
        public void GivenCreateCalled_AndDateDataRows_WhenFillData_ThenDatePrecisionToSecondsPreserved()
        {
            var nowWithSecondPrecision = DateTime.Now.Truncate(TimeSpan.FromSeconds(1));
            var data = new List<List<object>>
            {
                new List<object> { nowWithSecondPrecision },
                new List<object> { nowWithSecondPrecision.AddDays(2) },
                new List<object> { nowWithSecondPrecision.TimeOfDay }
            };
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.FillData(data);
                }

                File.WriteAllBytes(outputPath, stream.ToArray());
            }
            using (XLWorkbook workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                var date1 = worksheet.Cell("A3").GetDateTime();
                var date2 = worksheet.Cell("A4").GetDateTime();
                var timeSpan = worksheet.Cell("A5").GetTimeSpan();
                Assert.AreEqual(data[0][0], date1);
                Assert.AreEqual(data[1][0], date2);
                Assert.AreEqual(data[2][0], timeSpan);
            }
        }

        [TestMethod]
        public void GivenCreateCalled_AndNumericDataRow_WhenFillData_ThenDataValuesPreserved()
        {
            var data = new List<List<object>>
            {
                new List<object> { 273451076528.23824732952M, 4728237293822871085L, 235 }
            };
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.FillData(data);
                }

                File.WriteAllBytes(outputPath, stream.ToArray());
            }
            using (XLWorkbook workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                var value1 = worksheet.Cell("A3").GetValue<decimal>();
                var value2 = worksheet.Cell("B3").GetValue<long>();
                var value3 = worksheet.Cell("C3").GetValue<int>();
                Assert.AreEqual(data[0][0], value1);
                Assert.AreEqual(data[0][1], value2);
                Assert.AreEqual(data[0][2], value3);
            }
        }

        [TestMethod]
        public void GivenCreateCalled_AndNullDataRows_WhenFillData_ThenNullRowsIgnored()
        {
            var data = new List<List<object>>
            {
                null,
                null,
                null,
                new List<object> { 1 }
            };
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.FillData(data);
                }

                File.WriteAllBytes(outputPath, stream.ToArray());
            }
            using (XLWorkbook workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                Assert.AreEqual(data[3][0], worksheet.Cell("A3").GetValue<int>());
            }
        }

        [TestMethod]
        public void GivenCreateCalled_AndNullishValuesInDataRow_WhenFillData_ThenNullishCellsEmpty()
        {
            var expectedNonString = "non-nullish";
            var data = new List<List<object>>
            {
                new List<object> { null, new Nullable<DateTime>(), "", " \r\n\t", expectedNonString }
            };
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.FillData(data);
                }

                File.WriteAllBytes(outputPath, stream.ToArray());
            }
            using (XLWorkbook workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                Assert.AreEqual(string.Empty, worksheet.Cell("A3").Value);
                Assert.AreEqual(string.Empty, worksheet.Cell("B3").Value);
                Assert.AreEqual(string.Empty, worksheet.Cell("C3").Value);
                Assert.AreEqual(string.Empty, worksheet.Cell("D3").Value);
                Assert.AreEqual(expectedNonString, worksheet.Cell("E3").Value);
            }
        }

        [TestMethod]
        public void GivenCreateCalled_AndListValuesInDataRow_WhenFillData_ThenListItemsCommaSeparated()
        {
            string expected = "bob, smith";
            var data = new List<List<object>>
            {
                new List<object> { expected.Split(new [] { ", " }, StringSplitOptions.None) }
            };
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.FillData(data);
                }

                File.WriteAllBytes(outputPath, stream.ToArray());
            }
            using (XLWorkbook workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                Assert.AreEqual(expected, worksheet.Cell("A3").Value);
            }
        }

        [TestMethod]
        public void GivenCreateCalled_AndListValuesOfDifferingTypesInDataRow_WhenFillData_ThenListItemsCommaSeparated()
        {
            var values = new object[] { "bob", 12, DateTime.Now };
            var expected = string.Format("{0}, {1}, {2}", values);
            var data = new List<List<object>>
            {
                new List<object> { values }
            };
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.FillData(data);
                }

                File.WriteAllBytes(outputPath, stream.ToArray());
            }
            using (XLWorkbook workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                Assert.AreEqual(expected, worksheet.Cell("A3").Value);
            }
        }

        [TestMethod]
        public void GivenCreateCalled_AndListValuesDataRowHasNull_WhenFillData_ThenIgnoreNullInCommaSeparatedString()
        {
            var values = new object[] { "bob", null, DateTime.Now };
            var expected = string.Format("{0}, {2}", values);
            var data = new List<List<object>>
            {
                new List<object> { values }
            };
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.FillData(data);
                }

                File.WriteAllBytes(outputPath, stream.ToArray());
            }
            using (XLWorkbook workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                Assert.AreEqual(expected, worksheet.Cell("A3").Value);
            }
        }

        [TestMethod]
        public void GivenCreateNotCalled_WhenSetupFooter_ThenThrowException()
        {
            using (var target = new StudentProfileExportFile())
            {
                target.ExpectException<InvalidOperationException>(() => target.SetupFooter("whatever"));
            }
        }

        [TestMethod]
        public void GivenCreateCalled_WhenSetupFooter_ThenSucceed()
        {
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.SetupFooter("whatever");
                }
            }
        }

        [TestMethod]
        public void GivenFooterText_WhenSetupFooter_ThenFooterIsCreated()
        {
            string expected = "this is the expected footer text at the end of the file";
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.SetupFooter(expected);
                }

                File.WriteAllBytes(outputPath, stream.ToArray());
            }
            using (XLWorkbook workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                string actualOdd = worksheet.PageSetup.Footer.GetText(XLHFOccurrence.OddPages);
                string actualEven = worksheet.PageSetup.Footer.GetText(XLHFOccurrence.EvenPages);
                Assert.AreEqual(expected, actualOdd);
                Assert.AreEqual(expected, actualEven);
            }
        }

        [TestMethod]
        public void GivenFooterText_AndFooterTextExceeds255Characters_WhenSetupFooter_ThenThrowException()
        {
            StringBuilder tooLong = new StringBuilder();
            for (int i = 0; i < 256; i++)
            {
                tooLong.Append('a');
            }
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);
                    
                    target.ExpectException<ArgumentException>(() => target.SetupFooter(tooLong.ToString()));
                }
            }
            
        }

        [TestMethod]
        public void GivenNullFooterText_WhenSetupFooter_ThenFooterIsEmpty()
        {
            byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {
                    target.Create(stream);

                    target.SetupFooter(null);
                }

                File.WriteAllBytes(outputPath, stream.ToArray());
            }
            using (XLWorkbook workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                string actualOdd = worksheet.PageSetup.Footer.GetText(XLHFOccurrence.OddPages);
                string actualEven = worksheet.PageSetup.Footer.GetText(XLHFOccurrence.EvenPages);
                Assert.AreEqual(string.Empty, actualOdd);
                Assert.AreEqual(string.Empty, actualEven);
            }
        }

        [TestMethod]
        public void GivenNullStream_WhenCreate_ThenIsReadyTrue()
        {
            using (var target = new StudentProfileExportFile())
            {
                target.ExpectException<ArgumentNullException>(() => target.Create(null));
            }
        }

        [TestMethod]
        public void GivenValidStream_WhenCreate_ThenIsReadyTrue()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
                stream.Write(templateData, 0, (int)templateData.Length);
                using (var target = new StudentProfileExportFile())
                {

                    target.Create(stream);

                    Assert.IsTrue(target.IsReady);
                }
            }
        }

        [TestMethod]
        public void GivenInvalidStream_WhenCreate_ThenThrowException()
        {
            using (Stream stream = new MemoryStream())
            {
                using (var target = new StudentProfileExportFile())
                {

                    target.ExpectException<OpenXmlPackageException>(() => target.Create(stream));
                }
            }
        }

        [TestMethod]
        public void GivenNoStream_WhenCreate_ThenThrowException()
        {
            using (var target = new StudentProfileExportFile())
            {
                target.ExpectException<NotSupportedException>(() => target.Create());
            }
        }

        [TestMethod]
        public void GivenCreateCalled_WhenDispose_ThenIsReadyFalse()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] templateData = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
                stream.Write(templateData, 0, (int)templateData.Length);
                var target = new StudentProfileExportFile();
                try
                {
                    target.Create(stream);
                }
                finally
                {

                    target.Dispose();
                }

                Assert.IsFalse(target.IsReady);
            }
        }

        [TestMethod]
        public void WhenGenerateMapper_ThenGetStudentProfileExportDataMapper()
        {
            using (var target = new StudentProfileExportFile())
            {
                var actual = target.GenerateMapper();

                Assert.IsInstanceOfType(actual, typeof(StudentProfileExportDataMapper));
            }
        }
    }
}
