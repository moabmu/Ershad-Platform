using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    // One-to-one relationship: User and his position title.
    public class UserPositionTitle
    {
        public string UserForeignKey { get; set; }
        public ApplicationUser User { get; set; }

        public string UserPositionTitleId { get; set; }
        public string PositionTitle { get; set; }
    }
}
