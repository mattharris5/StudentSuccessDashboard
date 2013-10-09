using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace SSD.IO
{
    public static class ExcelParser
    {
        public static DataTable ExtractExcelSheetValues(Stream excelStream, string sheetName)
        {
            DataTable dt = new DataTable();
            using (SpreadsheetDocument spreadsheetDoc = SpreadsheetDocument.Open(excelStream, true))
            {
                //Access the main Workbook part, which contains data
                WorkbookPart workbookPart = spreadsheetDoc.WorkbookPart;
                Sheet ss = ExcelUtility.FindSheet(sheetName, workbookPart);
                WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(ss.Id);
                SharedStringTablePart stringTablePart = workbookPart.SharedStringTablePart;
                if (worksheetPart != null)
                {
                    Row lastRow = worksheetPart.Worksheet.Descendants<Row>().LastOrDefault();
                    Row headerRow = worksheetPart.Worksheet.Descendants<Row>().Skip(1).First();
                    if (headerRow != null)
                    {
                        foreach (Cell c in headerRow.ChildElements)
                        {
                            string value = GetValue(c, stringTablePart);
                            dt.Columns.Add(value);
                        }
                    }
                    if (lastRow != null)
                    {
                        for (int i = 1; i <= lastRow.RowIndex; i++)
                        {
                            DataRow dr = dt.NewRow();
                            bool empty = true;
                            Row row = worksheetPart.Worksheet.Descendants<Row>().Where(r => i == r.RowIndex).FirstOrDefault();
                            int j = 0;
                            if (row != null)
                            {
                                foreach (Cell c in row.ChildElements)
                                {
                                    //Get cell value
                                    string value = GetValue(c, stringTablePart);
                                    if (!string.IsNullOrEmpty(value) && value != " ")
                                    {
                                        empty = false;
                                    }
                                    var col = ConvertColumnLettering(c.CellReference.Value[0]);
                                    if (col >= 0)
                                    {
                                        dr[col] = value;
                                    }
                                    j++;
                                    if (j == dt.Columns.Count)
                                    {
                                        break;
                                    }
                                }
                                if (empty)
                                {
                                    break;
                                }
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                }
            }
            return dt;
        }

        private static string GetValue(Cell cell, SharedStringTablePart stringTablePart)
        {
            if (cell.ChildElements.Count == 0)
            {
                return null;
            }
            //get cell value
            string value = cell.ElementAt(0).InnerText;//CellValue.InnerText;
            //Look up real value from shared string table
            if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
            {
                value = stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            return value;
        } 

        private static int ConvertColumnLettering(char letter)
        {
            switch (letter)
            {
                // NOTE: First column (A) is empty and skipped
                case 'B':
                    return 1;
                case 'C':
                    return 2;
                case 'D':
                    return 3;
                case 'E':
                    return 4;
                case 'F':
                    return 5;
                default:
                    return -1;
            }
        }
    }
}
