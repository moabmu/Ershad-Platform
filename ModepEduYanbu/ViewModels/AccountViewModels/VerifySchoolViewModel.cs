using ModepEduYanbu.Helpers.ValidationsAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.AccountViewModels
{
    public class VerifySchoolViewModel
    {
        [Key]
        public string SchoolId { get; set; }

        [Display(Name ="اسم المدرسة")]
        public string SchoolName { get; set; }

        [Required]
        [Display(Name ="الرقم الوزاري")]
        //[ValidateSchoolMinistryNo(ErrorMessage ="الرقم المدخل غير صحيح", [CallerMemeberName]PersonIdNo = x)]
        public string SchoolMinistryNo { get; set; }

    }
}
