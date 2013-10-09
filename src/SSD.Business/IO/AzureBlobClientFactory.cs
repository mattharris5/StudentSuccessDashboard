using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;

namespace SSD.IO
{
    public static class AzureBlobClientFactory
    {
        public static AzureBlobClient Create()
        {
            string azureStorageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureStorageConnectionString);
            return new AzureBlobClient(storageAccount);
        }
    }
}
