using System;

namespace SSD.IO
{
    public class ExportFileFactory : IExportFileFactory
    {
        private static IExportFileFactory _instance = new ExportFileFactory();

        public static IExportFileFactory Current
        {
            get { return _instance; }
        }

        public static void SetCurrent(IExportFileFactory current)
        {
            if (current == null)
            {
                throw new ArgumentNullException("current");
            }
            _instance = current;
        }

        public IExportFile Create(Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }
            if (t == typeof(StudentProfileExportFile))
            {
                return new StudentProfileExportFile();
            }

            throw new InvalidOperationException("Specified type unrecognized.");
        }
    }
}
