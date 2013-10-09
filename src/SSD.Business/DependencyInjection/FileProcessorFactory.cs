using Castle.MicroKernel;
using SSD.IO;
using SSD.Repository;

namespace SSD.DependencyInjection
{
    public static class FileProcessorFactory
    {
        public static IFileProcessor Create(IKernel kernel, string typeKey)
        {
            var blobClient = kernel.Resolve<IBlobClient>();
            var repositories = kernel.Resolve<IRepositoryContainer>();
            if (typeKey == "ServiceOffering")
            {
                return new ServiceOfferingFileProcessor(blobClient, repositories);
            }
            else if (typeKey == "ServiceAttendance")
            {
                return new ServiceAttendanceFileProcessor(blobClient, repositories);
            }
            return null;
        }
    }
}
