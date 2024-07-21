using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    public class AzureContainer
    {
        public string AzureContainerId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public long Length { get; set; }
        public string Uri { get; set; }
    }
}
