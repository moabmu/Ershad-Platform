using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.DashboardViewModels
{
    public class RatedAndUnratedEduProgramsItemViewModel
    {
        public string Name { get; set; }
        public DateTime ReportDeadline { get; set; }
        public int SentReportsCount { get; set; }
        public int RatedReportsCount { get; set; }
        public Dictionary<string,string> RouteDataForUnratedLink { get; set; }
    }
}
