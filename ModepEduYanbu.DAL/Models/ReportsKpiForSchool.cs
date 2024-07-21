using ModepEduYanbu.Models.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    public class ReportKpiForSchool : ReportKpiBase<School>
    {
        //public string ReportKpiForSchoolId { get; set; }

        //[ForeignKey(nameof(SchoolId))]
        //public School School { get; set; }
        //public string SchoolId { get; set; }

        //[ForeignKey(nameof(EducationalYearId))]
        //public EducationalYear EducationalYear { get; set; }
        //public string EducationalYearId { get; set; }

        //public ICollection<ReportKpiRecord> KpiRecords { get; set; }
    }
}
