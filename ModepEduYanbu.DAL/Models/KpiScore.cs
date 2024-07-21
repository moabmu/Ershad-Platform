using System;
using System.Collections.Generic;
using System.Text;

namespace ModepEduYanbu.DAL.Models
{
    /// <summary>
    /// Used to give a name for kpi value (i.e. very good for 8.5/10)
    /// </summary>
    public class KpiGradeForBootstrap
    {
        private const string ERR_MSG = "Value should be between zero and one";
        public byte Order { get; set; }
        public string GradeTitle { get; set; }
        public string CssColor { get; set; }

        private decimal _highestValue;
        /// <summary>
        /// Domain between zero and one inclusive [0,1]
        /// </summary>
        public decimal HighestValue { 
            get 
            { 
                return _highestValue;
            }
            set 
            {
                if (!IsValidScore(value))
                    throw new ArgumentOutOfRangeException(ERR_MSG);
                _highestValue = value;
            } 
        }

        private decimal _lowestValue;
        /// <summary>
        /// Domain between zero and one inclusive [0,1]
        /// </summary>
        public decimal LowestValueInclusive
        {
            get
            {
                return _lowestValue;
            }
            set
            {
                if (!IsValidScore(value))
                    throw new ArgumentOutOfRangeException(ERR_MSG);
                _lowestValue = value;
            }
        }

        private static bool IsValidScore(decimal score)
        {
            return score >= 0.0m && score <= 1.0m;
        }

        /// <summary>
        /// Check if value matches this grade.
        /// </summary>
        /// <param name="kpiValue"></param>
        /// <returns></returns>
        public bool KpiValueMatchesThisKpiScore(decimal kpiValue)
        {
            if(!IsValidScore(kpiValue))
                throw new ArgumentOutOfRangeException(ERR_MSG);
            if (this.LowestValueInclusive > this.HighestValue)
                throw new InvalidOperationException("LowestValueInclusive is larger than HighestValue");

            bool result = kpiValue >= this.LowestValueInclusive;
            if (this.HighestValue == 1.0m)
            {
                return kpiValue <= this.HighestValue && result;
            }
            else
            {
                return kpiValue < this.HighestValue && result;
            }
        }
    }
}
