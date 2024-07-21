using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.AdminViewModels
{
    public class AddSchoolViewModel
    {
        private const string ERR_MSG_REQUIRED = "حقل {0} مطلوب.";
        private const string ERR_MSG_REGEX_INTEGERS = ".البيانات المدخلة يجب أن تكون عدد صحيح.";
        private const string ERR_MSG_REGEX_PERCENTAGE = "القيمة المدخلة يجب أن تكون بن 0 و 100.";
        private const string ERR_MSG_RANGE = "القيمة المدخلة غير مقبولة.";
        private const string ERR_MSG_MAX_STRING_LENGTH = "العدد الاقصى للأحرف المدخلة هو " + "{1}";
        private const string REGEX_PATTERN_INTEGERS = @"[0-9]{1,6}";

        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "الرقم الوزاري")]
        [StringLength(25, ErrorMessage = ERR_MSG_MAX_STRING_LENGTH)]
        public string MinistryNo { get; set; }

        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "اسم المدرسة")]
        [StringLength(100, ErrorMessage = ERR_MSG_MAX_STRING_LENGTH)]
        public string Name { get; set; }

        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "القطاع")]
        [StringLength(25, ErrorMessage = ERR_MSG_MAX_STRING_LENGTH)]
        public string City { get; set; }

        [Display(Name = "العنوان")]
        [StringLength(200, ErrorMessage = ERR_MSG_MAX_STRING_LENGTH)]
        public string Address { get; set; }

        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "رقم هوية قائد المدرسة")]
        [StringLength(25, ErrorMessage = ERR_MSG_MAX_STRING_LENGTH)]
        public string PrincipleIdNo { get; set; }


        [Display(Name = "رقم الهاتف")]
        [StringLength(25, ErrorMessage = ERR_MSG_MAX_STRING_LENGTH)]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "المرحلة الدراسية")]
        public string SelectedLevel{ get; set; }
        public ICollection<SelectListItem> Levels { get; set; }

        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "التصنيف")]
        public string SelectedType { get; set; }
        public ICollection<SelectListItem> Types { get; set; }
    }
}
