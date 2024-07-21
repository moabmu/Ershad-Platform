using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    /// <summary>
    /// Key: UserID, SchooId
    /// </summary>
    public class UserSchool
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string SchooId { get; set; }
        public School School { get; set; }
    }
}
