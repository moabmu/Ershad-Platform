using Microsoft.AspNetCore.Mvc.Rendering;
using ModepEduYanbu.ViewModels.Shared;
using ModepEduYanbu.ViewModels.ViewModelsForPartialViews;
using Sakura.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.DashboardViewModels
{
    public class DashboardViewModel
    {
        public List<EduProgram> EduPrograms { get; set; }
        public PartialReportsViewerViewModel PagedReportsViewer{get;set;}

        #region Search Arguments
        public string SchoolMinistryNoForFilter { get; set; }
        public string EduProgramIdForFilter { get; set; }
        public string ShowRatedOrUnratedReports { get; set; }

        //public IEnumerable<string> SelectedSchoolsForSearchFilter{ get; set; }
        //public ICollection<SelectListItem> SchoolsForSearchFilter { get; set; }

        //public IEnumerable<string> SelectedEduProgramsForSearchFilter { get; set; }
        //public ICollection<SelectListItem> EduProgramsForSearchFilter { get; set; }
        #endregion

        #region KPIs
        public IEnumerable<ReportKpiForSchool> TopSchools { get; set; }
        public IEnumerable<ReportKpiForMentor> TopMentors { get; set; }
        #endregion
    }
}
