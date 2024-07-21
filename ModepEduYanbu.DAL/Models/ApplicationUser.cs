using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ModepEduYanbu.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegDate { get; set; }
        public ApplicationUser AddedByUser { get; set; }
        public bool ViaNoor { get; set; }
        public int FailedAttemptsToVerfiyPhoneNumber { get; set; }
        public DateTime? LastManualSmsSendRequest { get; set; }

        public UserPositionTitle PositionTitle { get; set; }
        public List<UserSchool> UserSchools { get; set; }

        /// <summary>
        /// Current school appears on the dashboard for the user.
        /// </summary>
        public string CurrentSchoolId { get; set; }
        [ForeignKey("CurrentSchoolId")]
        public School CurrentSchool { get; set; }

        /// <summary>
        /// Authorized people whom added to the system by this user.
        /// </summary>
        public List<AuthorizedPerson> PeopleWhomAddedByUser { get; set; }

        /// <summary>
        /// ReportKpis for user if he is a mentor. Every mentor has a report kpi related to each educational year in the system.
        /// </summary>
        public List<ReportKpiForMentor> ReportsKpis_IfUserIsMentor { get; set; }

        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; } = new List<IdentityUserClaim<string>>();

        /// <summary>
        /// Navigation property for this users login accounts.
        /// </summary>
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; } = new List<IdentityUserLogin<string>>();
    }
}
