using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.DashboardViewModels
{
    public class SelectEducationalYearViewModel
    {
        public string SelectedYear { get; set; }
        public ICollection<SelectListItem> Years { get; set; }
    }
}
