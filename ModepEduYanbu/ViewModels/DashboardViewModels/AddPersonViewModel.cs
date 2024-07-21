using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.DashboardViewModels
{
    public class AddPersonViewModel
    {
        [Required(ErrorMessage = "الحفل مطلوب.")]
        [Display(Name ="رقم الهوية")]
        public string IdNo { get; set; }

        public string SelectedRole { get; set; }
        public ICollection<SelectListItem> Roles { get; set; }

        [Display(Name ="الاسم الكامل")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "عدد الأحرف المدخل غير مسموح به.")]
        public string FullName { get; set; }
        [StringLength(10, MinimumLength = 10, ErrorMessage = ("{0}" + " غير صحيح"))]
        [RegularExpression(@"05[0-9]{8}", ErrorMessage = "صيغة رقم الجوال غير صحيحة.")]
        [Display(Name = "رقم الجوال")]
        public string PhoneNumber { get; set; }
        [Display(Name = "البريد الالكتروني")]
        [EmailAddress]
        public string Email { get; set; }
        
        [Display(Name = "الرقم الوزاري")]
        //[ValidateSchoolMinistryNo(ErrorMessage ="الرقم المدخل غير صحيح", [CallerMemeberName]PersonIdNo = x)]
        public string SchoolMinistryNo { get; set; }
    }
}
