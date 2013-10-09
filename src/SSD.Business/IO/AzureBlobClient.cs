using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Reflection;

namespace SSD.IO
{
    public class AzureBlobClient : IBlobClient
    {
        public AzureBlobClient(CloudStorageAccount storageAccount)
        {
            if (storageAccount == null)
            {
                throw new ArgumentNullException("storageAccount");
            }
            StorageAccount = storageAccount;
        }

        private CloudStorageAccount StorageAccount { get; set; }

        public IBlobContainer CreateContainer(string containerName)
        {
            CloudBlobContainer container = CreateBlobContainer(StorageAccount, containerName);
            return new AzureBlobContainer(container);
        }

        private static CloudBlobContainer CreateBlobContainer(CloudStorageAccount storageAccount, string containerName)
        {
            CloudBlobClient blobClient = CreateBlobClient(storageAccount);
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            if (container.CreateIfNotExists())
            {
                Initialize(container);
            }
            container.FetchAttributes();
            return container;
        }

        private static void Initialize(CloudBlobContainer container)
        {
            container.Metadata.Add("Author", MethodInfo.GetCurrentMethod().GetType().Assembly.GetName().Name);
            container.Metadata.Add("DateCreated", DateTime.UtcNow.ToString());
            container.SetMetadata();
        }

        private static CloudBlobClient CreateBlobClient(CloudStorageAccount storageAccount)
        {
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient;
        }
    }
}
