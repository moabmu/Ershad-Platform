using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.AdminViewModels
{
    public class ChangeUserSchoolsViewModel
    {
        public string UserIdNo { get; set; }
        public string UserFullName { get; set; }
        public string RoleName { get; set; }
        public IEnumerable<SchoolInfoViewModel> UserSchools { get; set; }
    }

    public class SchoolInfoViewModel
    {
        public string SchoolId { get; set; }
        public string SchoolName { get; set; }
    }
}
