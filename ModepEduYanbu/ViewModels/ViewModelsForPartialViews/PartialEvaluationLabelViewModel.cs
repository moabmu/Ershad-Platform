using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.ViewModels.ViewModelsForPartialViews
{
    public class PartialEvaluationLabelViewModel
    {
        public decimal? EvaluationValue { get; set; }
        public decimal EvaluationBase { get; set; }
        public int DecimalsToRound { get; set; } = 2;
        /// <summary>
        /// i.e. "out of 10".
        /// </summary>
        public string Suffix { get; set; }
        /// <summary>
        /// Display grade based on Templates.KpiGradesForBootstrap.
        /// </summary>
        public bool DisplayGradeColor { get; set; }
        public bool DisplayGradeTitle { get; set; }
        public string Message { get; set; }
    }
}
