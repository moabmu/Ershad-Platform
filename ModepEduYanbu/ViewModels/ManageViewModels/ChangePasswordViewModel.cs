using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.ManageViewModels
{
    public class ChangePasswordViewModel
    {
        private const string REQUIRED_ERROR_MSG = "الحقل مطلوب.";
        [Required(ErrorMessage = REQUIRED_ERROR_MSG)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور الحالية")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = REQUIRED_ERROR_MSG)]
        [StringLength(25, ErrorMessage = "{0}" + " يجب أن يكون طولها بين " + "{2}" + " و " + "{1}" + " أحرف", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور الجديدة")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        [Compare("NewPassword", ErrorMessage = "لم تتطابق كلمة المرور الجديدة مع تأكيد كلمة المرور.")]
        public string ConfirmPassword { get; set; }
    }
}
