using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CodeTweet.ImagesDal
{
    public sealed class ImagesRepository : IImagesRepository
    {
        private readonly ImagesDbConfiguration _configuration;
        private CloudBlobContainer _container;
        private AsyncLazy<CloudBlobContainer> _asyncContainer;
        
        public ImagesRepository(ImagesDbConfiguration configuration)
        {
            _configuration = configuration;
            _asyncContainer = new AsyncLazy<CloudBlobContainer>(() => CreateBlobContainer());
        }
        
        private async Task<CloudBlobContainer> CreateBlobContainer()
        {
            var account = CloudStorageAccount.Parse(_configuration.ConnectionString);
            var client = account.CreateCloudBlobClient();
            
            _container = client.GetContainerReference(_configuration.ContainerId);
            
            if (await _container.CreateIfNotExistsAsync())
            {
                await _container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            return _container;
        }
        
        public async Task<string> UploadImageAsync(string id, Stream stream)
        {
            var blob = (await _asyncContainer).GetBlockBlobReference(id);

            blob.Properties.ContentType = "image/jpeg";

            await blob.UploadFromStreamAsync(stream);

            return blob.Uri.ToString();
        }
    }
}
