using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.Interfaces
{
    public interface IAzureFile
    {
        string Filename { get; set; }
        string Extension { get; set; }
        string Uri { get; set; }
        string FileTitle { get; set; }
        string AzureContainer { get; set; }
        string AzureBlobName { get; set; }
        DateTime UploadedDate { get; set; }
    }
}
