using QuartzWrapper.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    public class EduProgram : ITriggerKeyEntity
    {
        public string EduProgramId { get; set; }
        public string Name { get; set; }
        public string DescriptionFileExtension { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ReportDeadline { get; set; }

        public List<Report> Reports { get; set; }
        public EduProgramUploadedFile DescriptionFile { get; set; }

        public EducationalYear EducationalYear { get; set; }
        public string EducationalYearId { get; set; }

        /// <summary>
        /// Report kpis for an edu program for an educational year. This collection is similar to ReportsKpis_IfUserIsMentor in ApplicationUser class.
        /// </summary>
        public List<ReportKpiForEduProgram> ReportsKpisForEduPrograms { get; set; }
        
        /// <summary>
        /// When an edu program deadline passed, the system will automatically evaluate 0 for each mentor
        /// did not send a report for an edu program, consequently, the report column in ReportsKpiRecord will
        /// be null. Because of that, we added another column in ReportsKpiRecord for edu program to serve in case the 
        /// report column is null.
        /// </summary>
        public virtual ICollection<ReportKpiRecord> ReportKpiRecordsRelatedToEduProgram_CreatedBySystem { get; set; }

        public List<EduProgramSchoolLevel> SchoolLevels { get; set; }

        public string QuartzJob_CurrentTriggerKeyName { get; set; }
        public string QuartzJob_CurrentTriggerKeyGroup { get; set; }

        /// <summary>
        /// Indicates if the job starts running. if so, then editing the reportdeadline is not allowed.
        /// </summary>
        public bool QuartzJob_ComputeReportKpisJobStartsRunning { get; set; }
    }
}
