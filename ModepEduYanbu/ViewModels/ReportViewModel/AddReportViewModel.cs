using ModepEduYanbu.Helpers.ValidationsAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.ReportViewModel
{
    public class AddReportViewModel
    {
        private const string ERR_MSG_REQUIRED = "حقل {0} مطلوب.";
        private const string ERR_MSG_REGEX_INTEGERS = ".البيانات المدخلة يجب أن تكون عدد صحيح.";
        private const string ERR_MSG_REGEX_PERCENTAGE = "القيمة المدخلة يجب أن تكون بن 0 و 100.";
        private const string ERR_MSG_RANGE = "القيمة المدخلة غير مقبولة.";
        private const string REGEX_PATTERN_INTEGERS = @"[0-9]{1,6}";
        private const string REGEX_PATTERN_PERCENTAGE = @"[0-9]{1,3}(.?[0-9]{0,2})?";

        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "الشريحة المستهدفة")]
        [StringLength(150, ErrorMessage = "{0}" + " يجب أن يكون طولها بين " + "{2}" + " و " + "{1}" + " أحرف", MinimumLength = 5)]
        public string TargetedSlice { get; set; }

        //[RegularExpression(REGEX_PATTERN_INTEGERS, ErrorMessage = ERR_MSG_REGEX_INTEGERS)]
        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "أعداد المستهدفين")]
        [Range(0, 999999999, ErrorMessage = ERR_MSG_RANGE)]
        public int TargetedCount { get; set; }

        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "المجال")]
        [StringLength(25, ErrorMessage = "العدد الاقصى للأحرف المدخلة هو " + "{1}")]
        public string Field { get; set; }

        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "تاريخ التنفيذ")]
        [ValidateHijriDate]
        public String ExecutionDate { get; set; }

        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "عدد أيام التنفيذ")]
        [Range(1, 365, ErrorMessage = ERR_MSG_RANGE)]
        public int ExecutionPeriod { get; set; }

        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "بيانات التنفيذ")]
        [StringLength(2000, ErrorMessage = "{0}" + " يجب أن يكون طولها بين " + "{2}" + " و " + "{1}" + " أحرف", MinimumLength = 5)]
        [DataType(DataType.MultilineText)]
        public string ExecutionData { get; set; }

        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "النسبة المئوية للمشاركين")]
        [Range(0, 100, ErrorMessage = ERR_MSG_REGEX_PERCENTAGE)]
        public double ParticipantsRatio { get; set; }


        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "التحديات والحلول")]
        [StringLength(2000, ErrorMessage = "{0}" + " يجب أن يكون طولها بين " + "{2}" + " و " + "{1}" + " أحرف", MinimumLength = 5)]
        [DataType(DataType.MultilineText)]
        public string ChallengesSolus { get; set; }

        [Display(Name = "الرابط إلى شواهد التنفيذ (اختياري)")]
        [StringLength(2000, ErrorMessage = "{0}" + " يجب أن يكون طوله بين " + "{2}" + " و " + "{1}" + " أحرف", MinimumLength = 5)]
        public string UploadsLink { get; set; }

        // new elements
        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "عدد الإجراءات المنفذة")]
        [Range(0, 50, ErrorMessage = ERR_MSG_RANGE)]
        public int ProceduresCount { get; set; }

        [Required(ErrorMessage = ERR_MSG_REQUIRED)]
        [Display(Name = "مقترحات لإجراءات تنفيذية جديدة")]
        [StringLength(2000, ErrorMessage = "{0}" + " يجب أن يكون طولها بين " + "{2}" + " و " + "{1}" + " أحرف", MinimumLength = 5)]
        [DataType(DataType.MultilineText)]
        public string ProceduresSuggestions { get; set; }
    }
}
