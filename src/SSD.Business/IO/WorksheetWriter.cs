using DocumentFormat.OpenXml.Packaging;
using SSD.Domain;
using SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SSD.IO
{
    public class WorksheetWriter : IWorksheetWriter
    {
        private ServiceOffering ServiceOffering { get; set; }
        public string SheetName { get; private set; }
        public IList<FileRowModel> ErrorRows { get; private set; }

        private static readonly IReadOnlyDictionary<int, string> CellDictionary = new ReadOnlyDictionary<int, string>(new Dictionary<int, string>
        {
            { 0, "B" },
            { 1, "C" },
            { 2, "D" },
            { 3, "E" },
            { 4, "F" }
        });

        public WorksheetWriter(ServiceOffering offering, string sheetName)
        {
            if (offering == null)
            {
                throw new ArgumentNullException("offering");
            }
            ServiceOffering = offering;
            SheetName = sheetName;
            ErrorRows = new List<FileRowModel>();
        }

        public void CreateHeader(WorksheetPart worksheetPart)
        {
            if (worksheetPart == null)
            {
                throw new ArgumentNullException("worksheetPart");
            }
            ExcelUtility.SetStringCell(worksheetPart.Worksheet, "B", 2, ServiceOffering.Id.ToString());
            ExcelUtility.SetStringCell(worksheetPart.Worksheet, "C", 2, ServiceOffering.Name);
        }

        public void CreateErrorRows(WorksheetPart worksheetPart)
        {
            for (int rowIndex = 0; rowIndex < ErrorRows.Count; rowIndex++)
            {
                for (int i = 0; i < ErrorRows[rowIndex].RowErrors.Count; i++)
                {
                    ExcelUtility.SetStringCell(worksheetPart.Worksheet, CellDictionary[i], (uint)rowIndex + 4, ErrorRows[rowIndex].RowErrors[i]);
                }
            }
        }
    }
}
