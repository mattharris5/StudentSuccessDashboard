using DocumentFormat.OpenXml.Packaging;
using SSD.ViewModels;
using System.Collections.Generic;

namespace SSD.IO
{
    public interface IWorksheetWriter
    {
        string SheetName { get; }
        IList<FileRowModel> ErrorRows { get; }
        void CreateHeader(WorksheetPart worksheetPart);
        void CreateErrorRows(WorksheetPart worksheetPart);
    }
}
