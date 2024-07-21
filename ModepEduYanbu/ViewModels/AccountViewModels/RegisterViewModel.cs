using ModepEduYanbu.Helpers.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        private const string ERROR_MSG_PASSWORD = "يجب أن تحتوي كلمة المرور على أحرف انجليزية صغيرة وكبيرة ورموز/أرقام.";
        private const string ERROR_MSG_REQUIRED = "حقل {0} مطلوب.";
        private const string ERROR_MSG_EMAIL = "البريد الإلكتروني المدخل غير صحيح.";

        [Required(ErrorMessage = ERROR_MSG_REQUIRED)]
        [StringLength(100, MinimumLength =3, ErrorMessage = "عدد الأحرف المدخل غير مسموح به.")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; }

        [Required(ErrorMessage = ERROR_MSG_REQUIRED)]
        [EmailAddress(ErrorMessage = ERROR_MSG_EMAIL)]
        [Display(Name = "البريد الالكتروني")]
        public string Email { get; set; }

        [Display(Name = "تأكيد البريد الالكتروني")]
        [Compare("Email", ErrorMessage = "تأكيد البريد الإلكتروني غير متطابق مع حقل البريد الإلكتروني.")]
        public string ConfirmEmail { get; set; }

        [Required(ErrorMessage = ERROR_MSG_REQUIRED)]
        [StringLength(25, ErrorMessage = "{0}" + " يجب أن يكون طولها بين " + "{2}" + " و " + "{1}" + " أحرف", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[^a-zA-Z]).{6,25}$", ErrorMessage = ERROR_MSG_PASSWORD)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "تأكيد كلمة المرور غير متطابق مع كلمة المرور.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = ERROR_MSG_REQUIRED)]
        [StringLength(10, MinimumLength = 10, ErrorMessage = ("{0}" + " غير صحيح"))]
        [RegularExpression(@"05[0-9]{8}", ErrorMessage = "صيغة رقم الجوال غير صحيحة.")]
        [Display(Name = "رقم الجوال")]
        public string PhoneNumber { get; set; }

        [Display(Name = "تأكيد رقم الجوال")]
        [Compare("PhoneNumber", ErrorMessage = "تأكيد رقم الجوال غير متطابق مع حقل رقم الجوال.")]
        public string ConfirmPhoneNumber { get; set; }

        [StringLength(100, MinimumLength = 0, ErrorMessage = "عدد الأحرف المدخل غير مسموح به.")]
        [Display(Name = "المسمى الوظيفي")]
        public string PositionTitle { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}
