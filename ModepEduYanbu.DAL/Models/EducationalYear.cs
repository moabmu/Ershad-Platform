using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    public class EducationalYear
    {
        public string EducationalYearId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public uint EvaluationBase { get; set; } = 10;

        public ICollection<EduProgram> EduPrograms { get; set; }
        /// <summary>
        /// It is not neccesary to make one-to-many relationship between EducationalYear and
        /// Reports since report already has a relationship with EduProgam which already
        /// has a relationship with EducationalYear (In other words: Report >> EduProgram >> EducationalYear),
        /// but we need to archive all informations about each report inside of it, so that is why
        /// we created this relationship between Report and Educational Year. In addition, this will help
        /// to improve performance when query reports and their educational years.
        /// </summary>
        public ICollection<Report> Reports { get; set; }

        /// <summary>
        /// ReportKpis for all mentors for an educational year. It was called ReportsKpisForMentors_ForThisYear.
        /// </summary>
        public List<ReportKpiForMentor> ReportsKpisForMentors { get; set; }
        /// <summary>
        /// ReportKpis for all schools for an educational year. It was called ReportsKpisForSchools_ForThisYear.
        /// </summary>
        public List<ReportKpiForSchool> ReportsKpisForSchools { get; set; }
        /// <summary>
        /// ReportKpis for all edu programs for an educational year. It was called ReportsKpisForEduPrograms_ForThisYear.
        /// </summary>
        public List<ReportKpiForEduProgram> ReportKpisForEduPrograms { get; set; }
    }
}
