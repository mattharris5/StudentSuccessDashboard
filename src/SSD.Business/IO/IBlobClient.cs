using System;

namespace SSD.IO
{
    public interface IBlobClient
    {
        IBlobContainer CreateContainer(string containerName);
    }
}
