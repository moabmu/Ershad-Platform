using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.InfosViewModels
{
    public class UserInfoViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public IEnumerable<UserInfoSchools> Schools { get; set; }
        public string Role { get; set; }
        public string PositionTitle { get; set; }
        public DateTime RegistrationDate { get; set; }
        public IEnumerable<UserInfoReports> Reports { get; set; }
        public int SignedReportsCount { get; set; }
        public double? ReportsEvaluationAverage { get; set; }
        public PrivateUserInfos PrivateUserInfos { get; set; }
    }

    public class PrivateUserInfos
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AddedByUserFullname { get; set; }
        public string AddedByUserId { get; set; }
    }

    public class UserInfoSchools
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class UserInfoReports
    {
        public string Id { get; set; }
        public string ReportNo { get; set; }
        public string EduProgramName { get; set; }
        public double Evaluation { get; set; }
    }
}
