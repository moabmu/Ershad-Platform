using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.AccountViewModels
{
    public class VerifyCodeViewModel
    {
        [Required]
        [Display(Name = "وسيلة التحقق")]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "الرمز")]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "تذكر هذا المتصفح؟")]
        public bool RememberBrowser { get; set; }

        [Display(Name = "تذكرني؟")]
        public bool RememberMe { get; set; }
    }
}
