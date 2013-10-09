using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SSD.IO
{
    public class StudentProfileExportFile : IExportFile
    {
        public const uint HeaderRowIndex = 2;
        public const uint FirstDataRowIndex = HeaderRowIndex + 1;

        private SpreadsheetDocument Document { get; set; }
        private Worksheet Worksheet { get; set; }
        public bool IsReady
        {
            get { return Document != null && Worksheet != null; }
        }

        public void SetupColumnHeaders(IEnumerable<string> columnNames)
        {
            if (columnNames == null)
            {
                throw new ArgumentNullException("columnNames");
            }
            if (!IsReady)
            {
                throw new InvalidOperationException("A call to Create is required to begin working with this object.");
            }
            uint columnIndex = 1;
            foreach (string headerName in columnNames)
            {
                WriteColumnHeading(columnIndex, headerName);
                columnIndex++;
            }
        }

        public void FillData(IEnumerable<IEnumerable<object>> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (!IsReady)
            {
                throw new InvalidOperationException("A call to Create is required to begin working with this object.");
            }
            uint rowIndex = FirstDataRowIndex;
            foreach (IEnumerable<object> row in data)
            {
                if (row != null)
                {
                    WriteDataRow(rowIndex, row);
                    rowIndex++;
                }
            }
        }

        public void SetupFooter(string footer)
        {
            if (!IsReady)
            {
                throw new InvalidOperationException("A call to Create is required to begin working with this object.");
            }
            if (footer != null)
            {
                if (footer.Length > 255)
                {
                    throw new ArgumentException("Footer text cannot exceed 255 characters.");
                }
                WriteFooter(footer);
            }
        }

        public void Create(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            Document = SpreadsheetDocument.Open(stream, true);
            Document.ChangeDocumentType(SpreadsheetDocumentType.Workbook);
            Worksheet = Document.WorkbookPart.WorksheetParts.First().Worksheet;
        }

        public void Create()
        {
            throw new NotSupportedException(string.Format("{0} instances must be created via a stream to a valid Excel template file.", GetType().Name));
        }

        public void Save(Stream stream)
        {
            throw new NotImplementedException();
        }

        public IExportDataMapper GenerateMapper()
        {
            return new StudentProfileExportDataMapper();
        }

        public void Dispose()
        {
            if (Document != null)
            {
                Document.Dispose();
                Document = null;
            }
            Worksheet = null;
        }

        private void WriteColumnHeading(uint columnIndex, string headerName)
        {
            string columnName = ExcelUtility.GetColumnNameFromIndex(columnIndex);
            ExcelUtility.EnsureColumn(Worksheet, columnIndex);
            if (columnIndex > 1)
            {
                ExcelUtility.CopyCell(Worksheet, "A", HeaderRowIndex, columnName, HeaderRowIndex);
            }
            ExcelUtility.SetSharedStringCell(Document, Worksheet, columnName, HeaderRowIndex, headerName);
        }

        private void WriteDataRow(uint rowIndex, IEnumerable<object> row)
        {
            uint columnIndex = 1;
            foreach (object value in row)
            {
                if (value != null)
                {
                    WriteDataValue(rowIndex, columnIndex, value);
                }
                columnIndex++;
            }
        }

        private void WriteDataValue(uint rowIndex, uint columnIndex, object value)
        {
            string columnName = ExcelUtility.GetColumnNameFromIndex(columnIndex);
            IEnumerable<object> listValue = value as IEnumerable<object>;
            if (listValue != null)
            {
                value = string.Join(", ", listValue.Where(item => item != null).Select(item => item.ToString()));
            }
            ExcelUtility.SetSharedStringCell(Document, Worksheet, columnName, rowIndex, value.ToString());
        }

        private void WriteFooter(string footer)
        {
            HeaderFooter headerFooter = Worksheet.Descendants<HeaderFooter>().FirstOrDefault();
            if (headerFooter == null)
            {
                headerFooter = new HeaderFooter();
                Worksheet.AppendChild<HeaderFooter>(headerFooter);
            }
            headerFooter.EvenFooter = new EvenFooter();
            headerFooter.EvenFooter.Text = footer;
            headerFooter.OddFooter = new OddFooter();
            headerFooter.OddFooter.Text = footer;
        }
    }
}
