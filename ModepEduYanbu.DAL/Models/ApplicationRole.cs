using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    public class ApplicationRole : IdentityRole
    {
        // e.g.: "قائد مدرسة"
        public string Description { get; set; }
        // e.g.: "قائد المدرسة"
        public string DescriptionAsTitle { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
