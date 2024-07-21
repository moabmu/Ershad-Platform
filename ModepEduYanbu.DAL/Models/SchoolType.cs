using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    public class SchoolType
    {
        public string SchoolTypeId { get; set; }
        public string ClassificationName { get; set; }

        public List<School> Schools { get; set; }
    }
}
