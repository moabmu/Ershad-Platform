using ModepEduYanbu.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModepEduYanbu.Models;
using ModepEduYanbu.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ModepEduYanbu.DAL.DbContexts;

namespace ModepEduYanbu.Repositories
{
    public class ReportsRepo : IReportsRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IEducationalYearsRepo _educationalYearsRepo;

        public ReportsRepo(ApplicationDbContext context, IEducationalYearsRepo educationalYearsRepo)
        {
            _context = context;
            _educationalYearsRepo = educationalYearsRepo;

        }

        public enum ReportMessageId
        {
            SignByPrincipleSuccess,
            EvaluateSuccess
        }

        public Report Add(Report entity)
        {
            _context.Reports.Add(entity);
            return entity;
        }

        public IEnumerable<Report> GetAll()
        {
            return _context.Reports.ToList();
        }

        public Report GetById(string id)
        {
            return _context.Reports.FirstOrDefault(x => x.ReportId == id);
        }

        public IEnumerable<ReportResponse> GetResponses(Report report)
        {
            var r = _context.Reports
                .Include(x => x.Responses)
                .FirstOrDefault(x => x == report);
            if (r != null)
            {
                return r.Responses;
            }
            return null;
        }

        public IEnumerable<ReportUploadedFile> GetUploads(Report report)
        {
            var r = _context.Reports
                .Include(x => x.Uploads)
                .FirstOrDefault(x => x == report);
            if (r != null)
            {
                return r.Uploads;
            }
            return null;
        }

        public Report Remove(Report entity)
        {
            _context.Reports.Remove(entity);
            return entity;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public Report GetByReportNo(string reportNo, bool includeResponses = false, bool includeActivities = false, bool includeUploads = false)
        {
            if (includeResponses && includeActivities && includeUploads)
            {
                return _context.Reports
    .Include(x => x.Responses)
    .Include(x => x.Activities)
    .Include(x => x.Uploads)
    .FirstOrDefault(x => x.ReportNo == reportNo);
            }
            else if (includeResponses && includeUploads)
            {
                return _context.Reports
.Include(x => x.Responses)
.Include(x => x.Uploads)
.FirstOrDefault(x => x.ReportNo == reportNo);
            }
            else if (includeActivities && includeUploads)
            {
                return _context.Reports
.Include(x => x.Activities)
.Include(x => x.Uploads)
.FirstOrDefault(x => x.ReportNo == reportNo);
            }
            else if (includeResponses && includeActivities)
            {
                return _context.Reports
                    .Include(x => x.Responses)
                    .Include(x => x.Activities)
                    .FirstOrDefault(x => x.ReportNo == reportNo);
            }
            else if (includeResponses)
            {
                return _context.Reports
                    .Include(x => x.Responses)
                    .FirstOrDefault(x => x.ReportNo == reportNo);
            }
            else if (includeActivities)
            {
                return _context.Reports
                    .Include(x => x.Activities)
                    .FirstOrDefault(x => x.ReportNo == reportNo);
            }
            else if (includeUploads)
            {
                return _context.Reports
                    .Include(x => x.Uploads)
                    .FirstOrDefault(x => x.ReportNo == reportNo);
            }
            else
            {
                return _context.Reports.Include(x => x.Responses).FirstOrDefault(x => x.ReportNo == reportNo);
            }
        }

        public void AddReponse(ReportResponse response, Report report)
        {
            response.Report = report;
            _context.ReportResponses.Add(response);
        }

        public void AddActivity(ReportActivity activity, Report report)
        {
            activity.Report = report;
            _context.ReportActivities.Add(activity);
        }

        public Report GetWithUploads(string reportNo)
        {
            throw new NotImplementedException();
        }

        public Report GetWithResponses(string reportNo)
        {
            throw new NotImplementedException();
        }

        public Report GetWithActivities(string reportNo)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReportResponse> GetActivities(Report report)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Report> GetTopReports(int count)
        {
            return this.GetTopReportsAsQueryable().Take(count).AsEnumerable();
        }

        public IEnumerable<Report> GetTopReportsAsEnumerable(int count)
        {
            return this.GetTopReportsAsQueryable().Take(count);
        }

        private IQueryable<Report> GetTopReportsAsQueryable()
        {
            var currentYear = _educationalYearsRepo.GetCurrentYearOrCreateIfNotExist();
            return _context.Reports
                .Where(x => x.IsEvaluated && x.Evaluation >= 9 && x.EducationalYearId == currentYear.EducationalYearId) //.OrderByDescending(x => x.Evaluation)
                .OrderByDescending(x => x.SentDateTime);
        }
    }
}
