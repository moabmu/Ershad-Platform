using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        private const string ERROR_MSG_PASSWORD = "يجب أن تحتوي كلمة المرور على أحرف انجليزية صغيرة وكبيرة ورموز/أرقام.";
        private const string ERROR_MSG_REQUIRED = "حقل {0} مطلوب.";
        private const string ERROR_MSG_EMAIL = "البريد الإلكتروني المدخل غير صحيح.";

        [Display(Name = "رقم الهوية")]
        [Required(ErrorMessage = ERROR_MSG_REQUIRED)]
        public string IdNo { get; set; }

        [Required(ErrorMessage = ERROR_MSG_REQUIRED)]
        [StringLength(25, ErrorMessage = "{0}" + " يجب أن يكون طولها بين " + "{2}" + " و " + "{1}" + " أحرف", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "تأكيد كلمة المرور غير متطابق مع كلمة المرور.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "رمز التحقق لاستعادة كلمة المرور")]
        public string Code { get; set; }
    }
}
