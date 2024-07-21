using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.Abstract
{
    public class ReportKpiBase : KpiBase
    {
        [ForeignKey(nameof(EducationalYearId))]
        public EducationalYear EducationalYear { get; set; }
        public string EducationalYearId { get; set; }

        // Navigation Property
        public ICollection<ReportKpiRecord> KpiRecords { get; set; }
    }
}
