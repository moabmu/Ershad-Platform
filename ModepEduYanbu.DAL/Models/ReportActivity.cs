using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    public class ReportActivity
    {
        public string ReportActivityId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Content { get; set; }

        public int Rating { get; set; }

        public string IpAddress { get; set; }

        public string ReportId { get; set; }
        [ForeignKey("ReportId")]
        public Report Report { get; set; }
    }
}
