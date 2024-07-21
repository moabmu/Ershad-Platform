using ModepEduYanbu.Models.Constants;
using ModepEduYanbu.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.Abstract
{
    /// <summary>
    /// Owner could be: Mentor, School, or EduProgram
    /// </summary>
    public abstract class KpiBase: IKpi
    {
        public string KpiId { get; set; }
        [Column(TypeName = DomainConstraintsConstants.DECIMAL_FOR_KPI_NUMERATOR)]
        public decimal KpiNumerator { get ; set ; }
        public long KpiDenominator { get ; set ; }
        public DateTimeOffset KpiUpdatingDate { get; set; }

        public decimal GetKpiValue()
        {
            return KpiNumerator / KpiDenominator;
        }
        public decimal GetKpiValueAsPercentage()
        {
            return this.GetKpiValue() * 100;
        }
        public decimal GetKpiValueAsBase10()
        {
            return this.GetKpiValue() * 10;
        }
        public decimal GetKpiValueAsSpecificBase(uint baseValue)
        {
            return this.GetKpiValue() * baseValue;
        }
        public virtual string GetKpiValueAsPercentageToString()
        {
            return $"{this.GetKpiValueAsPercentage()}%";
        }
        public virtual string GetKpiValueAsBase10ToString()
        {
            return $"{this.GetKpiValueAsBase10()} من 10";
        }
        public virtual string GetKpiValueAsSpecificBaseToString(uint baseValue, string suffix, string prefix = "")
        {
            return $"{prefix}{this.GetKpiValueAsSpecificBase(baseValue)}{suffix}";
        }
    }
}
