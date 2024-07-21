using ModepEduYanbu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Repositories.Interfaces
{
    public interface IReportsRepo : IRepository<Report>
    {
        Report GetWithUploads(string reportNo);
        Report GetWithResponses(string reportNo);
        Report GetWithActivities(string reportNo);
        IEnumerable<ReportUploadedFile> GetUploads(Report report);
        IEnumerable<ReportResponse> GetResponses(Report report);
        IEnumerable<ReportResponse> GetActivities(Report report);
        IEnumerable<Report> GetTopReports(int count);
        IEnumerable<Report> GetTopReportsAsEnumerable(int count);
        Report GetByReportNo(string reportNo, bool includeResponses =false, bool includeActivities = false, bool includeUploads = false);
        void AddReponse(ReportResponse response, Report report);
        void AddActivity(ReportActivity activity, Report report);
    }
}
