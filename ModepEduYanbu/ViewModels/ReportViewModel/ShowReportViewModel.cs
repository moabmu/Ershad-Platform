using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.ReportViewModel
{
    public class ShowReportViewModel
    {
        [Display(Name = "رقم التقرير")]
        public string ReportNo { get; set; }

        [Display(Name ="اسم البرنامج الإرشادي")]
        public string EduProgramName { get; set; }

        [Display(Name = "تاريخ الإرسال")]
        public string SentDateTime { get; set; }

        [Display(Name = "المدرسة")]
        public string SchoolName { get; set; }

        [Display(Name = "أضيف بواسطة")]
        public string OwnerFullName { get; set; }

        [Display(Name = "الشريحة المستهدفة")]
        public string TargetedSlice { get; set; }

        [Display(Name = "أعداد المستهدفين")]
        public int TargetedCount { get; set; }

        [Display(Name = "المجال")]
        public string Field { get; set; }

        [Display(Name = "تاريخ التنفيذ")]
        public DateTime ExecutionDate { get; set; }

        [Display(Name = "عدد أيام التنفيذ")]
        public int ExecutionPeriod { get; set; }

        [Display(Name = "بيانات التنفيذ")]
        public string ExecutionData { get; set; }

        [Display(Name = "نسبة المئوية للمشاركين")]
        public double ParticipantsRatio { get; set; }

        [Display(Name = "التحديات والحلول")]
        public string ChallengesSolus { get; set; }

        [Display(Name = "الرابط إلى شواهد التنفيذ")]
        public string UploadsLink { get; set; }

        [Display(Name = "تقييم قسم التوجيه والإرشاد")]
        public double Evaluation { get; set; }

        // new element
        [Display(Name = "عدد الإجراءات المنفذة")]
        public int ProceduresCount { get; set; }

        // new element
        [Display(Name = "مقترحات لإجراءات تنفيذية جديدة")]
        public string ProceduresSuggestions { get; set; }

        public DateTime EvaluationDate { get; set; }

        public string EvaluatorFullName { get; set; }

        public int VisitorOverallRating { get; set; } = 0;
        public int VisitorRatingCount { get; set; } = 0;

        public IEnumerable<ReportResponse> Responses { get; set; }

        public IEnumerable<ReportActivity> Activities { get; set; }

        public IEnumerable<ReportUploadedFile> Uploads { get; set; }
    }
}
