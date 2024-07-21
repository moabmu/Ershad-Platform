using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.ManageViewModels
{
    public class AddPhoneNumberViewModel
    {
        private const string ERR_MSG_REQUIRED = "حقل {0} مطلوب.";

        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        //[Phone]
        [StringLength(10, MinimumLength = 10, ErrorMessage = ("{0}" + " غير صحيح"))]
        [RegularExpression(@"05[0-9]{8}", ErrorMessage = "صيغة رقم الجوال غير صحيحة.")]
        [Display(Name = "رقم الجوال")]
        public string PhoneNumber { get; set; }
    }
}
