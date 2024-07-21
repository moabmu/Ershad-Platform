using ModepEduYanbu.Models.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    public class ReportKpiForMentor : ReportKpiBase<ApplicationUser>
    {
        //public string ReportKpiForMentorId { get; set; }

        //[ForeignKey(nameof(MentorId))]
        //public ApplicationUser Mentor { get; set; }
        //public string MentorId { get; set; }

        //[ForeignKey(nameof(EducationalYearId))]
        //public EducationalYear EducationalYear { get; set; }
        //public string EducationalYearId { get; set; }

        //// Navigation Property
        //public ICollection<ReportKpiRecord> KpiRecords { get; set; }
    }
}
