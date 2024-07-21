using ModepEduYanbu.Models.Constants;
using ModepEduYanbu.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModepEduYanbu.Models.Abstract
{
    public class KpiRecordBase : IKpiRecord
    {
        [Key]
        public string KpiRecordId { get; set; }

        [Column(TypeName = DomainConstraintsConstants.DECIMAL_FOR_KPI_RECORD_NUMERATOR)]
        public decimal RecordValue { get; set; }

        public DateTimeOffset RecordDate { get; set; }
    }
}
