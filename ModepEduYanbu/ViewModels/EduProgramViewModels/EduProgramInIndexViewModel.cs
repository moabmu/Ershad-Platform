using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.EduProgramViewModels
{
    public class EduProgramInIndexViewModel
    {
        public string EduProgramId { get; set; }
        [Display(Name = "اسم البرنامج")]
        public string Name { get; set; }
        public string DescriptionFileExtension { get; set; }
        [Display(Name = "تاريخ التنفيذ")]
        public DateTime BeginDate { get; set; }
        [Display(Name = "تاريخ الانتهاء")]
        public DateTime EndDate { get; set; }
        [Display(Name = "التاريخ النهائي لرفع التقرير")]
        public DateTime ReportDeadline { get; set; }
    }
}
