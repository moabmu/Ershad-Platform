using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.AccountViewModels
{
    public class LoginViewModel
    {
        private const string ERROR_MSG_REQUIRED = "حقل {0} مطلوب.";

        [Required(ErrorMessage = ERROR_MSG_REQUIRED)]
        [Display(Name = "رقم الهوية")]
        public string Username { get; set; }

        [Required(ErrorMessage = ERROR_MSG_REQUIRED)]
        [Display(Name = "كلمة المرور")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "تذكر بيانات الدخول؟")]
        public bool RememberMe { get; set; }
    }
}
