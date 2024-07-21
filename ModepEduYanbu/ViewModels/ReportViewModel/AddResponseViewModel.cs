using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.ReportViewModel
{
    public class AddResponseViewModel
    {
        public string ReportNo { get; set; }

        [Display(Name = "الرد")]
        [Required(ErrorMessage = "الحقل مطلوب.")]
        [StringLength(2000, ErrorMessage = "{0}" + " يجب أن يكون طوله بين " + "{2}" + " و " + "{1}" + " أحرف", MinimumLength = 5)]
        public string Content { get; set; }
    }
}
