using Azure.Storage.Blobs;

namespace ApiAzurePracticaExamen.Services
{
    public class ServiceStorageBlob
    {
        private BlobServiceClient client;

        public ServiceStorageBlob(BlobServiceClient client)
        {
            this.client = client;
        }
        public string GetContainerUrl(string containerName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);

            return containerClient.Uri.AbsoluteUri;
        }
    }
}
