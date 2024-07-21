using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ModepEduYanbu.DAL.DbContexts;
using ModepEduYanbu.Data;
using ModepEduYanbu.Models;
using ModepEduYanbu.Repositories.Helpers;
using ModepEduYanbu.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Repositories
{
    public class AzureContainersRepo : IAzureContainersRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly string azureStorageConnString;

        public AzureContainersRepo(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            azureStorageConnString = _config["AzurePlatform:StorageConnString"];
        }

        public AzureContainer Add(AzureContainer entity)
        {
            _context.Add(entity);
            return entity;
        }

        public async Task<AzureContainer> GenerateNewContainer()
        {
            // Set the connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureStorageConnString);

            // Create a blob client. 
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to a container  
            var newContainerName = this.GenerateNewName();
            CloudBlobContainer container = blobClient.GetContainerReference(newContainerName);

            var newContainer = new AzureContainer
            {
                AzureContainerId = newContainerName,
                Name = newContainerName,
                CreatedDate = DateTime.Now,
                Length = 0
            };
            _context.AzureContainers.Add(newContainer);
            await _context.SaveChangesAsync();

            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(
    new BlobContainerPermissions
    {
        PublicAccess = BlobContainerPublicAccessType.Blob
    });
            return newContainer;
        }

        private string GenerateNewName()
        {
            string latestContainerName = this.GetLatest().Name;
            string containerName = _config["AzurePlatform:ContainerName"];
            int number = int.Parse(latestContainerName
                .Substring(latestContainerName.IndexOf(containerName) + containerName.Length));

            return $"{containerName}{++number}";
        }

        public IEnumerable<AzureContainer> GetAll()
        {
            throw new NotImplementedException();
            // Set the connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureStorageConnString);

            // Create a blob client. 
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //return blobClient.ListContainers().Select(x => new AzureContainer {
            //    AzureContainerId = x.Name,
            //    Name = x.Name,
            //    Length = x.Properties.
            //});
        }

        public AzureContainer GetById(string id)
        {
            return _context.AzureContainers.FirstOrDefault(x => x.AzureContainerId == id);
        }

        #region GetContainerLengthFromAzure Commented
        //public long GetContainerLengthFromAzure(string containerName)
        //{
        //    long size = 0;
        //    var list = _blobClient
        //        .GetContainerReference(containerName)
        //        .ListBlobs();
        //    foreach (CloudBlockBlob blob in list)
        //    {
        //        size += blob.Properties.Length;
        //    }
        //    return size;
        //}
        #endregion

        /// <summary>
        /// Upload() method will use the latest container, which can be called by GetLatest().
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<UploadResult> Upload(IFormFile file, string newFileName)
        {
            var latestContainer = this.GetLatest();
            try
            {
                if (!this.HasEnoughSpace(latestContainer, file.Length))
                {
                    latestContainer = await this.GenerateNewContainer();
                }

                if(latestContainer == null)
                {
                    throw new Exception("Latest Container is null.");
                }
            }
            catch
            {
                return null;
            }

            var containerName = latestContainer.Name; //this.GetLatest().Name;
            // Set the connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureStorageConnString);

            // Create a blob client. 
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to a container  
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            // Get a reference to a blob  
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(newFileName);

            // Create or overwrite the blob with the contents of a local file 
            using (var fileStream = file.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }

            latestContainer.Length += file.Length;
            await _context.SaveChangesAsync();

            return new UploadResult { Uri = blockBlob.Uri.ToString(), BlobName = newFileName, ContainerName = containerName, Length = file.Length };
        }

        

        public AzureContainer GetLatest()
        {
            return _context.AzureContainers.OrderByDescending(x => x.CreatedDate).FirstOrDefault();
        }


        public bool HasEnoughSpace(AzureContainer container, long fileSize)
        {
            long maxSize = 3298534883328; // 3TB
            return (maxSize - container.Length >= fileSize);
        }

        public AzureContainer Remove(AzureContainer entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task Delete(string containerName, string fileName)
        {
            var container = _context.AzureContainers.FirstOrDefault(x => x.Name == containerName);
            try
            {
                if (container == null)
                {
                    throw new Exception("Latest Container is null.");
                }
            }
            catch
            {
                return;
            }

            // Set the connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureStorageConnString);

            // Create a blob client. 
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to a container  
            CloudBlobContainer azureContainer = blobClient.GetContainerReference(containerName);

            // Get a reference to a blob  
            CloudBlockBlob blockBlob = azureContainer.GetBlockBlobReference(fileName);

            container.Length -= blockBlob.Properties.Length;
            await _context.SaveChangesAsync();
            await blockBlob.DeleteAsync();
        }
    }
}
