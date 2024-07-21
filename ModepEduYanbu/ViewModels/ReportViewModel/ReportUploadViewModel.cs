using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.ReportViewModel
{
    public class ReportUploadViewModel
    {
        [Required(ErrorMessage = "Required field.")]
        public string ReportNo { get; set; }
    }
}
