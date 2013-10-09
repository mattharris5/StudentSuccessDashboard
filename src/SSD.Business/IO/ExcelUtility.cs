using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.IO
{
    public static class ExcelUtility
    {
        public static WorksheetPart GetWorksheetPartByName(SpreadsheetDocument document, string sheetName)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>().
                                            Elements<Sheet>().Where(s => s.Name == sheetName);
            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(relationshipId);
            return worksheetPart;
        }

        public static Sheet FindSheet(string sheetName, WorkbookPart workbookPart)
        {
            if (string.IsNullOrEmpty(sheetName))
            {
                return workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault();
            }
            return workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).SingleOrDefault<Sheet>();
        }

        public static int IndexOfSharedString(SpreadsheetDocument spreadsheet, string stringItem)
        {
            SharedStringTable sharedStringTable = spreadsheet.WorkbookPart.SharedStringTablePart.SharedStringTable;
            bool found = false;
            int index = 0;
            foreach (SharedStringItem sharedString in sharedStringTable.Elements<SharedStringItem>())
            {
                if (sharedString.InnerText == stringItem)
                {
                    found = true;
                    break; ;
                }
                index++;
            }
            return found ? index : -1;
        }

        public static void EnsureColumn(Worksheet worksheet, uint columnIndex)
        {
            var columns = worksheet.Elements<Columns>().FirstOrDefault();
            if (columns == null)
            {
                columns = worksheet.InsertAt(new Columns(), 0);
            }
            if (columns.Elements<Column>().Where(item => item.Min == columnIndex).Count() == 0)
            {
                Column previousColumn = null;
                for (uint counter = columnIndex - 1; counter > 0; counter--)
                {
                    previousColumn = columns.Elements<Column>().Where(item => item.Min == counter).FirstOrDefault();
                    if (previousColumn != null)
                    {
                        break;
                    }
                }
                columns.InsertAfter(new Column()
                   {
                       Min = columnIndex,
                       Max = columnIndex,
                       CustomWidth = true,
                       Width = 9
                   }, previousColumn);
            }
        }

        public static bool AddSharedString(SpreadsheetDocument spreadsheet, string stringItem, bool save = true)
        {
            SharedStringTable sharedStringTable = spreadsheet.WorkbookPart.SharedStringTablePart.SharedStringTable;
            if (0 == sharedStringTable.Where(item => item.InnerText == stringItem).Count())
            {
                sharedStringTable.AppendChild(new SharedStringItem(new Text(stringItem)));
                if (save)
                {
                    sharedStringTable.Save();
                }
            }
            return true;
        }

        public static void SetSharedStringCell(SpreadsheetDocument spreadsheet, Worksheet worksheet, string colName, uint rowIndex, string value)
        {
            Cell cell = GetCell(worksheet, colName, rowIndex);
            if (IndexOfSharedString(spreadsheet, value) == -1)
            {
                AddSharedString(spreadsheet, value, true);
            }
            string columnValue = IndexOfSharedString(spreadsheet, value).ToString();
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
            cell.CellValue = new CellValue(columnValue);
        }

        public static void SetStringCell(Worksheet worksheet, string colName, uint rowIndex, string value)
        {
            Cell cell = GetCell(worksheet, colName, rowIndex);
            cell.DataType = new EnumValue<CellValues>(CellValues.String);
            cell.CellValue = new CellValue(value);
        }

        // Given a worksheet, a column name, and a row index, 
        // gets the cell at the specified column and 
        private static Cell GetCell(Worksheet worksheet, string columnName, uint rowIndex)
        {
            Row row = GetRow(worksheet, rowIndex);
            string cellReference = FormatCellReference(columnName, rowIndex);
            Cell cell = row.Elements<Cell>().Where(c => c.CellReference.Value.Equals(cellReference, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (cell == null)
            {
                cell = new Cell { CellReference = cellReference };
                row.AppendChild(cell);
            }
            return cell;
        }

        public static void CopyCell(Worksheet worksheet, string sourceColumnName, uint sourceRowIndex, string targetColumnName, uint targetRowIndex)
        {
            Cell cloneSource = GetCell(worksheet, sourceColumnName, sourceRowIndex);
            Cell clone = (Cell)cloneSource.CloneNode(true);
            Row row = GetRow(worksheet, targetRowIndex);
            clone.CellReference.InnerText = FormatCellReference(targetColumnName, targetRowIndex);
            row.Append(clone);
        }

        private static string FormatCellReference(string columnName, uint rowIndex)
        {
            return columnName + rowIndex.ToString();
        }

        // Given a worksheet and a row index, return the row.
        private static Row GetRow(Worksheet worksheet, uint rowIndex)
        {
            Row row;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            if (sheetData.Elements<Row>().Where(item => item.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(item => item.RowIndex == rowIndex).First();
            }
            else
            {
                Row previousRow = null;
                row = new Row() { RowIndex = rowIndex };
                for (uint counter = rowIndex - 1; counter > 0; counter--)
                {
                    previousRow = sheetData.Elements<Row>().Where(item => item.RowIndex == counter).FirstOrDefault();
                    if (previousRow != null)
                    {
                        break;
                    }
                }
                sheetData.InsertAfter(row, previousRow);
            }
            return row;
        }

        public static bool TryGetOADate(string value, out DateTime? date)
        {
            Double oaDateFormat = 0;
            date = null;
            if (!string.IsNullOrEmpty(value) && !Double.TryParse(value, out oaDateFormat))
            {
                return false;
            }
            if (oaDateFormat > 0)
            {
                date = DateTime.FromOADate(oaDateFormat);
            }
            return true;
        }

        public static string GetColumnNameFromIndex(uint columnIndex)
        {
            uint remainder;
            string columnName = "";
            while (columnIndex > 0)
            {
                remainder = (columnIndex - 1) % 26;
                columnName = System.Convert.ToChar(65 + remainder).ToString() + columnName;
                columnIndex = (uint)((columnIndex - remainder) / 26);
            }
            return columnName;
        }  
    }
}
