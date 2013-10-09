using System;
using System.IO;

namespace SSD.IO
{
    public interface IBlobContainer
    {
        string Name { get; }
        void DownloadToStream(string address, Stream target);
        void UploadFromStream(string address, Stream source);
        void UploadFromStream(string address, Stream source, string contentType);
        void Delete(string address);
        void DeleteAged(DateTime olderThan);
    }
}
