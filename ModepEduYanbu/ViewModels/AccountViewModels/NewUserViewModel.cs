using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ModepEduYanbu.Helpers.ValidationAttributes;

namespace ModepEduYanbu.Models.AccountViewModels
{
    public class NewUserViewModel
    {
        [Display(Name = "رقم الهوية")]
        [Required(ErrorMessage = "الرجاء إدخال رقم الهوية.")]
        [StringLength(12, MinimumLength = 7, ErrorMessage = "الرقم المدخل غير صحيح.")]
        [ValidateAuthorizedPersonId]
        public string UserIdNo { get; set; }
    }
}
