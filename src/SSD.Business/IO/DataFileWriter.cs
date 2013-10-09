using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SSD.IO
{
    public class DataFileWriter
    {
        public Stream FileContentStream { get; private set; }

        public void Write(IBlobContainer blobContainer, string blobAddress)
        {
            if (blobContainer == null)
            {
                throw new ArgumentNullException("blobContainer");
            }
            if (FileContentStream == null)
            {
                throw new InvalidOperationException(string.Format("Cannot {0} prior to initialize from a template.", MethodBase.GetCurrentMethod().Name));
            }
            blobContainer.UploadFromStream(blobAddress, FileContentStream);
        }

        public void BuildTemplate(IEnumerable<string> erroredRows)
        {
            MemoryStream memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            foreach (var row in erroredRows)
            {
                streamWriter.WriteLine(row);
            }
            streamWriter.Flush();
            FileContentStream = memoryStream;
            FileContentStream.Position = 0;
        }

        public void SetContentStream(Stream stream)
        {
            if (stream != null)
            {
                FileContentStream = stream;
            }
        }
    }
}
