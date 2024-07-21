using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.Abstract
{
    /// <summary>
    /// Base class for any report kpi. E.g.: ReportKpiForMentor...etc
    /// </summary>
    /// <typeparam name="TOwner">Owner could be: Mentor, School, or EduProgram</typeparam>
    public abstract class ReportKpiBase<TOwner> : ReportKpiBase
    {
        [ForeignKey(nameof(OwnerId))]
        public TOwner Owner { get; set; }
        public string OwnerId { get; set; }

        /// <summary>
        /// Get type of the owner of the report kpi. Owner of ReportKpi could be:
        /// Mentor, School, or EduProgram
        /// </summary>
        /// <returns></returns>
        public static Type GetOwnerType()
        {
            return typeof(TOwner);
        }
    }
}
