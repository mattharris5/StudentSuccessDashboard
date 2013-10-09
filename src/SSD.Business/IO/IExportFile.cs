using System;
using System.Collections.Generic;
using System.IO;

namespace SSD.IO
{
    public interface IExportFile : IDisposable
    {
        void SetupColumnHeaders(IEnumerable<string> columnNames);
        void FillData(IEnumerable<IEnumerable<object>> data);
        void SetupFooter(string footer);
        void Create(Stream stream);
        void Create();
        void Save(Stream stream);
        IExportDataMapper GenerateMapper();
    }
}
