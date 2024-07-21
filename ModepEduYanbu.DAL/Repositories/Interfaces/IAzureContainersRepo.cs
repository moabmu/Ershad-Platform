using Microsoft.AspNetCore.Http;
using ModepEduYanbu.Models;
using ModepEduYanbu.Repositories.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Repositories.Interfaces
{
    public interface IAzureContainersRepo : IRepository<AzureContainer>
    {
        AzureContainer GetLatest();
        Task<AzureContainer> GenerateNewContainer();
        Task<UploadResult> Upload(IFormFile file, string newFileName = null);
        Task Delete(string container, string fileName);
        bool HasEnoughSpace(AzureContainer container, long fileSize);
        //Task<bool> HasEnoughSpace(string containerName, long fileSize);
    }
}
