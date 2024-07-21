using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.ReportViewModel
{
    public class EvaluateViewModel
    {
        public string ReportNo { get; set; }

        [Required(ErrorMessage = "الحقل مطلوب.")]
        [Display(Name = "التقييم")]
        [Range(0, 10, ErrorMessage = "القيمة المدخلة يجب أن تكون بين 0 و 10.")]
        public double Evaluation { get; set; }
    }
}
