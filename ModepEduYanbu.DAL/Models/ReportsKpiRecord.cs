using ModepEduYanbu.Models.Abstract;
using ModepEduYanbu.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    /// <summary>
    /// If Report is null, then the record is created by system to give 0 because 
    /// the mentor does not send a report for an EduProgram.
    /// If RecordCreator is null, then the record is created by the system.
    /// </summary>
    public class ReportKpiRecord : KpiRecordBase
    {
        // Option 1: There is a report.
        [ForeignKey(nameof(ReportId))]
        public Report Report { get; set; }
        public string ReportId { get; set; }

        // Option 2: Thers is no report, in this case, the kpi record must mention information
        // cannot be retrived from Report property, such as: EduProgram, and School.
        #region For purposes of automatic evaluation for non-sent reports (evaluation by system).
        [ForeignKey(nameof(EduProgramId))]
        public EduProgram EduProgram { get; set; }
        public string EduProgramId { get; set; }

        [ForeignKey(nameof(SchoolId))]
        public School School { get; set; }
        public string SchoolId { get; set; }

        [ForeignKey(nameof(MentorId))]
        public ApplicationUser Mentor { get; set; }
        public string MentorId { get; set; }
        #endregion

        //public decimal RecordValue { get; set; } // equivalent to Numenator. Denominator is 1.
        //public DateTimeOffset RecordDate { get; set; }

        /// <summary>
        /// Record Creator is the report evaluator. If it is null then the system is the record creator.
        /// </summary>
        public ApplicationUser RecordCreator { get; set; }
        public string RecordCreatorId { get; set; }

        // Circular Relationship: Evaluation value of old record has been reset by this new record.
        [ForeignKey(nameof(PreviousRecordGetResetByThisRecordId))]
        public ReportKpiRecord PreviousRecordGetResetByThisRecord { get; set; }
        public string PreviousRecordGetResetByThisRecordId { get; set; }

        // Navigation Property
        /// <summary>
        /// ReportKpiForMentor that owns this record.
        /// </summary>
        public ReportKpiForMentor ReportKpiForMentor_OwnsThisRecord { get; set; }

        // Navigation Property
        /// <summary>
        /// ReportsKpiForSchool that is related to this record.
        /// </summary>
        public ReportKpiForSchool ReportKpiForSchool_OwnsThisRecord { get; set; }

        public ReportKpiForEduProgram ReportKpiForEduProgram_OwnsThisRecord { get; set; }
    }
}
