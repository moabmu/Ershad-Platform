using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.ReportViewModel
{
    public class AddActivityViewModel
    {
        private const string ERROR_MSG_REQUIRED = "حقل {0} مطلوب.";
        private const string ERROR_MSG_PASSWORD = "يجب أن تحتوي كلمة المرور على أحرف انجليزية صغيرة وكبيرة ورموز/أرقام.";
        private const string ERROR_MSG_EMAIL = "البريد الإلكتروني المدخل غير صحيح.";

        public string ReportNo { get; set; }

        [Display(Name = "رأي الزائر")]
        [Required(ErrorMessage = ERROR_MSG_REQUIRED)]
        [StringLength(2000, ErrorMessage = "{0}" + " يجب أن يكون طوله بين " + "{2}" + " و " + "{1}" + " أحرف", MinimumLength = 5)]
        public string Content { get; set; }

        [Required(ErrorMessage = ERROR_MSG_REQUIRED)]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; }

        [Required(ErrorMessage = ERROR_MSG_REQUIRED)]
        [Display(Name = "البريد الالكتروني")]
        [EmailAddress(ErrorMessage = ERROR_MSG_EMAIL)]
        [StringLength(100, ErrorMessage = "{0}" + " يجب أن يكون طوله بين " + "{2}" + " و " + "{1}" + " أحرف", MinimumLength = 5)]
        public string Email { get; set; }

        [HiddenInput]
        [Display(Name = "التقييم")]
        public int Rating { get; set; }
    }
}
