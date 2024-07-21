using ModepEduYanbu.ViewModels.Shared;
using Sakura.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.ViewModels.ViewModelsForPartialViews
{
    public class PartialReportsViewerViewModel
    {
        public IPagedList<ReportSummary> Reports { get; set; }
        public string ErrorMessageWhenReportsNull { get; set; }
        public bool DisplayEditReportButton { get; set; }
        public bool DisplayTotalCountOfReports { get; set; }
        public bool DisplayPager { get; set; }
        /// <summary>
        /// To use it with anchor tag helper asp-fragment or a href=#
        /// </summary>
        public string HtmlId { get; set; }
    }
}
