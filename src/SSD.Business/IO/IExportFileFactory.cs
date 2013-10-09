using System;

namespace SSD.IO
{
    public interface IExportFileFactory
    {
        IExportFile Create(Type t);
    }
}
