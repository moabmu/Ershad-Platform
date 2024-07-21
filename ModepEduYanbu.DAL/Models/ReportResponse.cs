using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    public class ReportResponse
    {
        public string ReportResponseId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Content { get; set; }

        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public ApplicationUser Owner { get; set; }
        public string OwnerFullName { get; set; }
        public string OwnerIdNo { get; set; }

        public string ReportId { get; set; }
        [ForeignKey("ReportId")]
        public Report Report { get; set; }
    }
}
