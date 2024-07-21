using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    public class School
    {
        public string SchoolId { get; set; }
        public string MinistryNo { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PrincipleIdNo { get; set; }
        public string PhoneNumber { get; set; }

        [ForeignKey(nameof(LevelId))]
        public SchoolLevel Level { get; set; }
        public string LevelId { get; set; }
        public SchoolType Type { get; set; }
        public List<UserSchool> UsersForSchool { get; set; }

        /// <summary>
        /// Current users who select this school to be appeared on their dashboards.
        /// </summary>
        public List<ApplicationUser> CurrentUsers { get; set; }

        public List<Report> Reports { get; set; }
        public List<ReportKpiForSchool> ReportsKpisForSchool { get; set; }

        public virtual ICollection<ReportKpiRecord> ReportKpiRecordsRelatedToSchool_CreatedBySystem { get; set; }
    }
}
