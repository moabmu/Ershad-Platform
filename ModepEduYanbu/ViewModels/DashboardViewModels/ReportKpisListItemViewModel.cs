using ModepEduYanbu.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.DashboardViewModels
{
    public class ReportKpisListItemViewModel : KpiBase
    {
        public string OwnerName { get; set; }
        public string OwnerId { get; set; } // use to if there is a url to the owner card.
    }
}
