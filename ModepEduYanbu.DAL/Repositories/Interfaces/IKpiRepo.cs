using ModepEduYanbu.Data;
using ModepEduYanbu.Models;
using ModepEduYanbu.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Repositories.Interfaces
{
    public interface IKpiRepo
    {
        Task AdjustReportKpisAsync(Report report, ReportKpiRecord kpiRecord);
        TKpi GetReportKpi<TKpi>(string ownerId, string yearId) where TKpi : ReportKpiBase;
        Task AdjustReportKpisForNonSentReportsAsync(EduProgram eduProgram);
    }
}
