using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SSD.IO
{
    public class AzureBlobContainer : IBlobContainer
    {
        private CloudBlobContainer Container { get; set; }

        public AzureBlobContainer(CloudBlobContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            Container = container;
        }

        public string Name
        {
            get { return Container.Name; }
        }

        public void DownloadToStream(string address, Stream target)
        {
            CloudBlockBlob blob = LoadBlob(address);
            DownloadToStream(target, blob);
        }

        public void UploadFromStream(string address, Stream source)
        {
            CloudBlockBlob blob = LoadBlob(address);
            UploadFromStream(blob, source);
        }

        public void UploadFromStream(string address, Stream source, string contentType)
        {
            CloudBlockBlob blob = LoadBlob(address);
            blob.Properties.ContentType = contentType;
            source.Position = 0;
            UploadFromStream(blob, source);
        }

        public void Delete(string address)
        {
            CloudBlockBlob blob = LoadBlob(address);
            blob.Delete();
        }

        public void DeleteAged(DateTime olderThanUtc)
        {
            IEnumerable<IListBlobItem> blobs = Container.ListBlobs();
            foreach (IListBlobItem blobDefinition in blobs)
            {
                DeleteIfAged(olderThanUtc, blobDefinition);
            }
        }

        private void DeleteIfAged(DateTime olderThanUtc, IListBlobItem blobDefinition)
        {
            CloudBlockBlob blob = LoadBlob(blobDefinition.Uri.ToString());
            if (IsBlobAged(blob, olderThanUtc))
            {
                DeleteIfExists(blob);
            }
        }

        private CloudBlockBlob LoadBlob(string address)
        {
            try
            {
                return Container.GetBlockBlobReference(address);
            }
            catch (StorageException e)
            {
                throw new BlobException(string.Format(CultureInfo.InvariantCulture, "Could not GetBlockBlobReference for blob at address '{0}' from container.  See inner exception for details.", address), e);
            }
        }

        private static bool IsBlobAged(CloudBlockBlob blob, DateTime olderThanUtc)
        {
            try
            {
                blob.FetchAttributes();
                return blob.Properties.LastModified < olderThanUtc;
            }
            catch (StorageException e)
            {
                throw new BlobException(string.Format(CultureInfo.InvariantCulture, "Could not FetchAttributes or examine LastModified from properties for blob named '{0}' to see if it is aged older than '{1}' (UTC).  See inner exception for details.", blob.Name, olderThanUtc.ToString()), e);
            }
        }

        private static void DeleteIfExists(CloudBlockBlob blob)
        {
            try
            {
                blob.DeleteIfExists();
            }
            catch (StorageException e)
            {
                throw new BlobException(string.Format(CultureInfo.InvariantCulture, "Could not DeleteIfExists blob named '{0}' from container.  See inner exception for details.", blob.Name), e);
            }
        }

        private static void DownloadToStream(Stream target, CloudBlockBlob blob)
        {
            try
            {
                blob.DownloadToStream(target);
            }
            catch (StorageException e)
            {
                throw new BlobException(string.Format(CultureInfo.InvariantCulture, "Could not DownloadToStream from blob named '{0}' in container.  See inner exception for details.", blob.Name), e);
            }
        }

        private void UploadFromStream(CloudBlockBlob blob, Stream source)
        {
            try
            {
                blob.UploadFromStream(source);
            }
            catch (StorageException e)
            {
                throw new BlobException(string.Format(CultureInfo.InvariantCulture, "Could not UploadFromStream to blob named '{0}' in container.  See inner exception for details.", blob.Name), e);
            }
        }
    }
}
