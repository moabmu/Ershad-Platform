using Microsoft.EntityFrameworkCore;
using ModepEduYanbu.DAL.DbContexts;
using ModepEduYanbu.DAL.Extensions;
using ModepEduYanbu.Models;
using ModepEduYanbu.Repositories.Interfaces;
using Quartz;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Jobs
{
    /// <summary>
    /// Each finished eduProgram has its own job to compute KPIs in background.
    /// Job arguments are set using JobDataMap for the trigger of the job.
    /// Trigger will be used to identify each job.
    /// </summary>
    public class ComputeReportKpisForFinishedEduProgram : IJob
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IKpiRepo _kpiRepo;

        public ComputeReportKpisForFinishedEduProgram(ApplicationDbContext dbContext, IKpiRepo kpiRepo)
        {
            _dbContext = dbContext;
            _kpiRepo = kpiRepo;
        }
        public Task Execute(IJobExecutionContext context)
        {
            var eduProgramId = context.Trigger.JobDataMap["EduProgramId"].ToString();
            var eduProgram = _dbContext.EduPrograms.Find(eduProgramId);

            // Indicate that this job starts running
            eduProgram.QuartzJob_ComputeReportKpisJobStartsRunning = true;
            _dbContext.SaveChangesAsync().Wait();

            _kpiRepo.AdjustReportKpisForNonSentReportsAsync(eduProgram: eduProgram).Wait();

            Debug.WriteLine($"Done for EduProgram {eduProgramId}");

            return Task.CompletedTask;
        }
    }
}
