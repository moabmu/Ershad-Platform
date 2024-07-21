using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Repositories.Helpers
{
    public class UploadResult
    {
        public string BlobName { get; set; }
        public string ContainerName { get; set; }
        public string Uri { get; set; }
        public long Length { get; set; }
    }
}
