using ModepEduYanbu.Models.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    public class ReportKpiForEduProgram : ReportKpiBase<EduProgram>
    {
        //public string ReportKpiForEduProgramId { get; set; }

        //[ForeignKey(nameof(EduProgramId))]
        //public EduProgram EduProgram { get; set; }
        //public string EduProgramId { get; set; }

        //[ForeignKey(nameof(EducationalYearId))]
        //public EducationalYear EducationalYear { get; set; }
        //public string EducationalYearId { get; set; }

        //// Navigation Property
        //public ICollection<ReportKpiRecord> KpiRecords { get; set; }
    }
}
