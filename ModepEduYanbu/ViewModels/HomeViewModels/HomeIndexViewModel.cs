using ModepEduYanbu.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.ViewModels.HomeViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<ReportSummary> TopReports { get; set; }
    }
}
