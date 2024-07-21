using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {

        private const string ERROR_MSG_REQUIRED = "حقل {0} مطلوب.";

        [Display(Name = "رقم الهوية")]
        [Required(ErrorMessage = ERROR_MSG_REQUIRED)]
        public string IdNo { get; set; }
    }
}
