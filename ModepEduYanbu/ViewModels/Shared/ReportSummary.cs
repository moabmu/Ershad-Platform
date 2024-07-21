using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.ViewModels.Shared
{
    public class ReportSummary
    {
        public string ReportNo { get; set; }
        public string EduProgramName { get; set; }
        public string SchoolName { get; set; }
        public string OwnerFullName { get; set; }
        public bool IsSignedByPrinciple { get; set; }
        public bool IsEvaluated { get; set; }
        public double Evaluation { get; set; }
        public DateTime? EvaluationDate { get; set; }
        public int VisitorRatingCount { get; set; }
        public int VisitorOverallRating { get; set; }
    }
}
