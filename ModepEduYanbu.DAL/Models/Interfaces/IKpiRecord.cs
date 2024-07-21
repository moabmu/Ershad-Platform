using System;

namespace ModepEduYanbu.Models.Interfaces
{
    public interface IKpiRecord
    {
        decimal RecordValue { get; set; } // equivalent to Numenator. Denominator is 1.
        DateTimeOffset RecordDate { get; set; }
    }
}
