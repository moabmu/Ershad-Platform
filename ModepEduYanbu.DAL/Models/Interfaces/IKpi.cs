using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.Interfaces
{
    public interface IKpi
    {
        string KpiId { get; set; }
        decimal KpiNumerator { get; set; }
        long KpiDenominator { get; set; }
        DateTimeOffset KpiUpdatingDate { get; set; }
    }
}
