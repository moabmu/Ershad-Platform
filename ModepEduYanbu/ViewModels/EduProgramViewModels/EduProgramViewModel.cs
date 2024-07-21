using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModepEduYanbu.Helpers.ValidationsAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.EduProgramViewModels
{
    public class EduProgramViewModel
    {
        public string EduProgramId { get; set; }

        [Required(ErrorMessage ="الحفل مطلوب.")]
        [Display(Name = "اسم البرنامج")]
        public string Name { get; set; }

        [Display(Name = "الملف التوضيحي")]
        public IFormFile DescriptionFile { get; set; }

        [Required(ErrorMessage = "الحقل مطلوب.")]
        [Display(Name = "تاريخ التنفيذ")]
        [ValidateHijriDate]
        public String BeginDate { get; set; }

        [Required(ErrorMessage = "الحقل مطلوب.")]
        [Display(Name = "تاريخ الانتهاء")]
        [ValidateHijriDate]
        public String EndDate { get; set; }

        [Required(ErrorMessage = "الحقل مطلوب.")]
        [Display(Name = "التاريخ النهائي لرفع التقرير")]
        [ValidateHijriDate]
        public String ReportDeadline { get; set; }

        [Required(ErrorMessage = "الحقل مطلوب.")]
        public IEnumerable<string> SelectedSchoolLevels { get; set; }
        public ICollection<SelectListItem> SchoolLevels { get; set; }
    }
}
