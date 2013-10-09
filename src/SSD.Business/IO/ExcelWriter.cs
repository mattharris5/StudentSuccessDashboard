using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.IO;
using System.Reflection;

namespace SSD.IO
{
    public class ExcelWriter
    {
        public const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public Stream FileContentStream { get; private set; }

        public void Write(IBlobContainer blobContainer, string blobAddress)
        {
            if (blobContainer == null)
            {
                throw new ArgumentNullException("blobContainer");
            }
            if (FileContentStream == null)
            {
                throw new InvalidOperationException(string.Format("Cannot {0} prior to initializing from a template.", MethodBase.GetCurrentMethod().Name));
            }
            blobContainer.UploadFromStream(blobAddress, FileContentStream, ContentType);
        }

        public void InitializeFrom(string templatePath, IWorksheetWriter worksheetWriter)
        {
            if (worksheetWriter == null)
            {
                throw new ArgumentNullException("worksheetWriter");
            }
            byte[] byteArray = File.ReadAllBytes(templatePath);
            MemoryStream stream = new MemoryStream(byteArray);
            using (SpreadsheetDocument spreadsheetDoc = SpreadsheetDocument.Open(stream, true))
            {
                // Change from template type to workbook type
                spreadsheetDoc.ChangeDocumentType(SpreadsheetDocumentType.Workbook);
                WorksheetPart worksheetPart = ExcelUtility.GetWorksheetPartByName(spreadsheetDoc, worksheetWriter.SheetName);
                if (worksheetPart != null)
                {
                    worksheetWriter.CreateHeader(worksheetPart);
                }
            }
            FileContentStream = stream;
        }

        public void InitializeFrom(string filePath)
        {
            byte[] data = File.ReadAllBytes(filePath);
            FileContentStream = new MemoryStream(data);
        }

        public void AppendErrorRows(string sheetName, IWorksheetWriter worksheetWriter)
        {
            if (worksheetWriter == null)
            {
                throw new ArgumentNullException("worksheetWriter");
            }
            if (FileContentStream == null)
            {
                throw new InvalidOperationException(string.Format("Cannot {0} prior to initializing from a template.", MethodBase.GetCurrentMethod().Name));
            }
            using (SpreadsheetDocument spreadsheetDoc = SpreadsheetDocument.Open(FileContentStream, true))
            {
                //Access the main Workbook part, which contains data
                WorkbookPart workbookPart = spreadsheetDoc.WorkbookPart;
                Sheet ss = ExcelUtility.FindSheet(sheetName, workbookPart);
                if (ss == null)
                {
                    throw new InvalidOperationException("Cannot find sheet named '" + sheetName + "' in workbook.");
                }
                WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(ss.Id);
                if (worksheetPart != null)
                {
                    worksheetWriter.CreateErrorRows(worksheetPart);
                    worksheetPart.Worksheet.Save();
                }
            }
        }
    }
}
