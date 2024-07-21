using Microsoft.EntityFrameworkCore;
using ModepEduYanbu.DAL.DbContexts;
using ModepEduYanbu.DAL.Extensions;
using ModepEduYanbu.Data;
using ModepEduYanbu.Models;
using ModepEduYanbu.Models.Abstract;
using ModepEduYanbu.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ModepEduYanbu.Repositories
{
    public class KpiRepo : IKpiRepo
    {
        private readonly ApplicationDbContext _context;
        public KpiRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AdjustReportKpisAsync(Report report, ReportKpiRecord newKpiRecord)
        {
            if (report != null)
            {
                if (newKpiRecord.EduProgram != null) throw new ArgumentException("EduProgram must be null when evaluating a report.");
                if (!String.IsNullOrEmpty(newKpiRecord.EduProgramId)) throw new ArgumentException("EduProgramId must be null or empty when evaluating a report.");
                if (newKpiRecord.School != null) throw new ArgumentException("School must be null when evaluating a report.");
                if (!String.IsNullOrEmpty(newKpiRecord.SchoolId)) throw new ArgumentException("SchoolId must be null or empty when evaluating a report.");
                if (newKpiRecord.RecordCreator == null) throw new ArgumentNullException("RecordCreator cannot be null when evaluating a report.");
                if(String.IsNullOrEmpty(newKpiRecord.RecordCreatorId)) throw new ArgumentException("RecordCreatorId cannot be null or empty when evaluating a report.");
                if (!report.IsSignedByPrinciple) throw new InvalidOperationException("Report must be signed by principle.");

                newKpiRecord.Report = report;
                newKpiRecord.ReportId = report.ReportId;

                _context.Entry(report).Reference(x => x.EduProgram).Load();
                _context.Entry(report.EduProgram).Reference(x => x.EducationalYear).Load();
            }
            else
            {
                if (newKpiRecord.EduProgram == null) throw new ArgumentNullException("EduProgram cannot be null when the argument 'report' is null.");
                if (String.IsNullOrEmpty(newKpiRecord.EduProgramId)) throw new ArgumentException("EduProgramId cannot be null or empty when the argument 'report' is null.");
                if (newKpiRecord.School == null) throw new ArgumentNullException("School cannot be null when the argument 'report' is null.");
                if (String.IsNullOrEmpty(newKpiRecord.SchoolId)) throw new ArgumentException("SchoolId cannot be null or empty when the argument 'report' is null.");
                if (newKpiRecord.RecordCreator != null) throw new ArgumentNullException("RecordCreator must be null when the argument 'report' is null, because this evaluation process is done by system.");
                if (!String.IsNullOrEmpty(newKpiRecord.RecordCreatorId)) throw new ArgumentException("RecordCreatorId must be null or empty when the argument 'report' is null, because this evaluation process is done by system.");
            }


            //var mentorKpi = await AdjustReportKpiForTargetTypeAsync<ReportKpiForMentor>(kpiRecord);
            var mentorKpi = await AdjustReportKpiForMentorAsync(newKpiRecord);
            var schoolKpi = await AdjustReportKpiForSchoolAsync(newKpiRecord);
            var eduProgramKpi = await AdjustReportKpiForEduProgramAsync(newKpiRecord);
        }
        public TKpi GetReportKpi<TKpi>(string ownerId, string yearId) where TKpi : ReportKpiBase
        {
            // if(TKpi is ReportForMentor) is not working, because:
            // typeof(whatever) always returns an instance of type Type.Type doesn't derive from KpiBase.
            // typeof(TKpi) == typeof(ReportKpiForMentor)    (ownerId, yearId);
            //return _context.Query<TKpi>().FirstOrDefault();

            // Dynamic lambda expression
            // https://www.codemag.com/article/1607041/Simplest-Thing-Possible-Dynamic-Lambda-Expressions
            // x =>
            var parameterExpression = Expression.Parameter(typeof(TKpi), "x");

            // exp1: x.OwnerId == report.OwnerId
            var kpiOwnerIdProperty = Expression.Property(parameterExpression, "OwnerId");
            var reportOwnerIdConstant = Expression.Constant(ownerId);
            var exp1 = Expression.Equal(kpiOwnerIdProperty, reportOwnerIdConstant);

            // exp2: x.EducationalYearId == year.EducationYearId
            var kpiYearIdProperty = Expression.Property(parameterExpression, "EducationalYearId");
            var eduYearIdConstant = Expression.Constant(yearId);
            var exp2 = Expression.Equal(kpiYearIdProperty, eduYearIdConstant);

            // expression: exp1 && exp2
            var expresssion = Expression.AndAlso(exp1, exp2);
            var lambda = Expression.Lambda<Func<TKpi, bool>>(expresssion, parameterExpression);
            var result = _context.Set<TKpi>().SingleOrDefault(lambda);
            return result;
            //var compiledLambda = lambda.Compile();
            //TKpi myTargetEntity = default(TKpi);
            //compiledLambda(myTargetEntity);
        }
        public async Task AdjustReportKpisForNonSentReportsAsync(EduProgram eduProgram)
        {
            // START: Adjust kpis for non-sent reports
            var mentorRoleId = _context.Roles.SingleOrDefault(r => r.Name == "mentor").Id;
            _context.Entry(eduProgram).Collection(x => x.SchoolLevels).Load();
            _context.Entry(eduProgram).Collection(x => x.Reports).Load();
            var mentors = _context.Users  // mentors did not send reports for eduProgram
                .Include(x => x.Roles)
                .Where(mentor => mentor.Roles.Any(role => role.RoleId == mentorRoleId));
            mentors = mentors
                .Include(x => x.UserSchools)
                .ThenInclude(x => x.School)
                .Where(mentor => eduProgram.Reports == null || !eduProgram.Reports.Any(x => x.OwnerId == mentor.Id && x.IsSignedByPrinciple)); 
            foreach (var mentorsChunk in mentors.OrderBy(x => x.Id).QueryChunksOfSize(50))
            {
                foreach (var m in mentorsChunk)
                {
                    var mentorSchoolEntity = (m.UserSchools.Count == 1) ? m.UserSchools[0] : m.UserSchools
                        .FirstOrDefault(x => eduProgram.SchoolLevels.Any(level => level.SchoolLevelId == x.School.LevelId));
                    var mentorSchool = mentorSchoolEntity?.School;
                    if (mentorSchool is null) continue;
                    var evalRecord = new ReportKpiRecord
                    {
                        KpiRecordId = Guid.NewGuid().ToString(),
                        RecordValue = 0m,
                        RecordDate = new DateTimeOffset(eduProgram.ReportDeadline,
                        TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time")
                        .GetUtcOffset(eduProgram.ReportDeadline))
                        .AddSeconds(1.0),
                        EduProgram = eduProgram,
                        EduProgramId = eduProgram.EduProgramId,
                        Mentor = m,
                        MentorId = m.Id,
                        School = mentorSchool,
                        SchoolId = mentorSchool.SchoolId
                    };
                    await AdjustReportKpisAsync(null, evalRecord).ConfigureAwait(false);
                }
                _context.SaveChanges();
            }
        }

        #region Helpers
        private async Task<ReportKpiForEduProgram> AdjustReportKpiForEduProgramAsync(ReportKpiRecord kpiRecord)
        {
            Report report = null;
            EducationalYear year = null;
            string ownerId = null;
            if(kpiRecord.Report != null)
            {
                report = kpiRecord.Report;
                year = report.EduProgram.EducationalYear;
                ownerId = report.EduProgramId;
            }
            else
            {
                year = _context.EducationalYears.SingleOrDefault(x => x.EducationalYearId == kpiRecord.EduProgram.EducationalYearId);
                ownerId = kpiRecord.EduProgramId;
            }
            var eduProgramKpi = _context.ReportsKpisForEduPrograms
                .FirstOrDefault(x => x.OwnerId == ownerId && x.EducationalYearId == year.EducationalYearId); // report.EduProgramId

            if (eduProgramKpi is null)
            {
                eduProgramKpi = new ReportKpiForEduProgram
                {
                    EducationalYearId = year.EducationalYearId,
                    OwnerId = ownerId,
                    KpiUpdatingDate = kpiRecord.RecordDate,
                    KpiDenominator = 1,
                    KpiNumerator = 1
                };
                _context.ReportsKpisForEduPrograms.Add(eduProgramKpi);
                await _context.SaveChangesAsync();
            }
            var kpiHasDefaultValue = _context.Entry(eduProgramKpi).Collection(x => x.KpiRecords).Query().Count() < 1;
            AffectRecordOnReportKpi(eduProgramKpi, kpiRecord, kpiHasDefaultValue);
            eduProgramKpi.KpiRecords.Add(kpiRecord);
            //await _context.SaveChangesAsync();
            return eduProgramKpi;
        }
        private async Task<ReportKpiForSchool> AdjustReportKpiForSchoolAsync(ReportKpiRecord kpiRecord)
        {
            Report report = null;
            EducationalYear year = null;
            string ownerId = null;
            if (kpiRecord.Report != null)
            {
                report = kpiRecord.Report;
                year = report.EduProgram.EducationalYear;
                ownerId = report.SchoolId;
            }
            else
            {
                year = _context.EducationalYears.SingleOrDefault(x => x.EducationalYearId == kpiRecord.EduProgram.EducationalYearId);
                ownerId = kpiRecord.SchoolId;
            }
            var schoolKpi = _context.ReportsKpisForSchools
                .FirstOrDefault(x => x.OwnerId == ownerId && x.EducationalYearId == year.EducationalYearId);
            if (schoolKpi is null)
            {
                schoolKpi = new ReportKpiForSchool
                {
                    EducationalYearId = year.EducationalYearId,
                    OwnerId = ownerId,
                    KpiUpdatingDate = kpiRecord.RecordDate,
                    KpiDenominator = 1,
                    KpiNumerator = 1
                };
                _context.ReportsKpisForSchools.Add(schoolKpi);
                await _context.SaveChangesAsync();
            }
            var kpiHasDefaultValue = _context.Entry(schoolKpi).Collection(x => x.KpiRecords).Query().Count() < 1;
            AffectRecordOnReportKpi(schoolKpi, kpiRecord, kpiHasDefaultValue);
            schoolKpi.KpiRecords.Add(kpiRecord);
            //await _context.SaveChangesAsync();
            return schoolKpi;
        }
        private async Task<ReportKpiForMentor> AdjustReportKpiForMentorAsync(ReportKpiRecord kpiRecord)
        {
            Report report = null;
            EducationalYear year = null;
            string ownerId = null;
            if (kpiRecord.Report != null)
            {
                report = kpiRecord.Report;
                year = report.EduProgram.EducationalYear;
                ownerId = report.OwnerId;
            }
            else
            {
                year = _context.EducationalYears.SingleOrDefault(x => x.EducationalYearId == kpiRecord.EduProgram.EducationalYearId);
                ownerId = kpiRecord.MentorId;
            }
            var mentorKpi = _context.ReportsKpisForMentors
                .FirstOrDefault(x => x.OwnerId == ownerId && x.EducationalYearId == year.EducationalYearId); // && EduYear == CurrentYear
            if (mentorKpi is null)
            {
                // 1.0 Create Kpi
                mentorKpi = new ReportKpiForMentor
                {
                    EducationalYearId = year.EducationalYearId,
                    OwnerId = ownerId,
                    KpiUpdatingDate = kpiRecord.RecordDate,
                    KpiDenominator = 1,
                    KpiNumerator = 1
                };
                _context.ReportsKpisForMentors.Add(mentorKpi);
                await _context.SaveChangesAsync();
            }
            var kpiHasDefaultValue = _context.Entry(mentorKpi).Collection(x => x.KpiRecords).Query().Count() < 1; // mean menotrKpi has the default value: 1/1
            AffectRecordOnReportKpi(mentorKpi, kpiRecord, kpiHasDefaultValue);
            mentorKpi.KpiRecords.Add(kpiRecord);
            //await _context.SaveChangesAsync();
            return mentorKpi;
        }
        private void AffectRecordOnReportKpi(ReportKpiBase kpi, ReportKpiRecord evalRecord, bool kpiHasDefaultValue)
        {
            kpi.KpiUpdatingDate = evalRecord.RecordDate;
            if (kpiHasDefaultValue) // mean menotrKpi has the default value: 1/1
            {
                SetReportKpiThatHasNoRecords(kpi, evalRecord);
            }
            else
            {
                SetReportKpiThatHasRecords(kpi, evalRecord);
            }
        }
        /// <summary>
        /// Set report kpi when there is no records.
        /// </summary>
        /// <param name="kpi"></param>
        /// <param name="evalRecord"></param>
        private void SetReportKpiThatHasNoRecords(ReportKpiBase kpi, ReportKpiRecord evalRecord)
        {
            kpi.KpiNumerator = evalRecord.RecordValue;
            kpi.KpiDenominator = 1;
            kpi.KpiUpdatingDate = evalRecord.RecordDate;
        }
        /// <summary>
        /// Set report kpi when there are already records.
        /// </summary>
        /// <param name="kpi"></param>
        /// <param name="evalRecord"></param>
        private void SetReportKpiThatHasRecords(ReportKpiBase kpi, ReportKpiRecord evalRecord)
        {
            if (kpi is null)
                return;

            void directIncreaseKpi(ReportKpiBase k) // Local function. Instead of action.
            {
                k.KpiNumerator += evalRecord.RecordValue;
                k.KpiDenominator++;
            }

            if (evalRecord.Report is null)
            {
                directIncreaseKpi(kpi);
                return;
            }

            // Check if report was evaluated before, if so then reset by cancel the last eval.
            //_context.Entry(mentorKpi).Collection(x => x.KpiRecords).Load();
            var reportHistoryRecords = _context.HistoryRecordsForReportKpis
                .Where(x => x.ReportId == evalRecord.Report.ReportId);
            if (reportHistoryRecords.Count() < 1) // report was not evaluated before
            {
                directIncreaseKpi(kpi);
            }
            else // report was evaluated before, reset MentorKpi to replace the old evaluation with the new evaluation
            {
                var lastRecord = reportHistoryRecords.OrderByDescending(x => x.RecordDate).FirstOrDefault();
                if (evalRecord.KpiRecordId != lastRecord.KpiRecordId)
                {
                    evalRecord.PreviousRecordGetResetByThisRecord = lastRecord;
                    evalRecord.PreviousRecordGetResetByThisRecordId = lastRecord.PreviousRecordGetResetByThisRecordId;

                    kpi.KpiNumerator -= lastRecord.RecordValue;
                    kpi.KpiNumerator += evalRecord.RecordValue;
                }
            }
        }
        #endregion

        #region Commented Code: Dynamic Programming for AdjustReportKpiFor(Mentor | School | EduProgram)
        //// TKpi is of type ReportKpiBase. But because the class ReportKpiBase is generic [ReportKpiBase<TOwner>]
        //// then, I need to use reflection to get the owner by calling the static method GetOwnerType
        //// in ReportKpiBase.
        //private async Task<TKpi> AdjustReportKpiForTargetTypeAsync<TKpi>(ReportKpiRecord kpiRecord) where TKpi: KpiBase
        //{
        //    // Reflection to make x in the filter of type ReportKpiBase<TOwner>
        //    Type ownerType = typeof(TKpi)
        //        .GetMethod("GetOwnerType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        //        .Invoke(null, null) as Type;

        //    // TKpi is ReportKpiForMentor..etc, and of type ReportKpiBase<TOwner>

        //    var report = kpiRecord.Report;
        //    var year = report.EduProgram.EducationalYear;

        //    MethodInfo getDbSetMethod = _context.GetType().GetMethod("GetDbSet", BindingFlags.Public | BindingFlags.Instance);
        //    getDbSetMethod = getDbSetMethod.MakeGenericMethod(typeof(TKpi));
        //    var reportKpiForOwnerSet = getDbSetMethod.Invoke(_context, null) as IQueryable<TKpi>;

        //    var firstOrDefaultMethod = _context.GetDbSet<TKpi>(typeof(TKpi)).GetType().GetMethod("FirstOrDefault");

        //    // Dynamic lambda expression
        //    // https://www.codemag.com/article/1607041/Simplest-Thing-Possible-Dynamic-Lambda-Expressions
        //    // x =>
        //    var parameterExpression = Expression.Parameter(typeof(TKpi), "x");

        //    // exp1: x.OwnerId == report.OwnerId
        //    var kpiOwnerIdProperty = Expression.Property(parameterExpression, "OwnerId");
        //    var reportOwnerIdConstant = Expression.Constant(report.OwnerId);
        //    var exp1 = Expression.Equal(kpiOwnerIdProperty, reportOwnerIdConstant);

        //    // exp2: x.EducationalYearId == year.EducationYearId
        //    var kpiYearIdProperty = Expression.Property(parameterExpression, "EducationalYearId");
        //    var eduYearIdConstant = Expression.Constant(year.EducationalYearId);
        //    var exp2 = Expression.Equal(kpiYearIdProperty, eduYearIdConstant);

        //    // expression: exp1 && exp2
        //    var expresssion = Expression.AndAlso(exp1, exp2);
        //    var lambda = Expression.Lambda<Func<TKpi, bool>>(expresssion, parameterExpression);
        //    //var compiledLambda = lambda.Compile();
        //    //TKpi myTargetEntity = default(TKpi);
        //    //compiledLambda(myTargetEntity);

        //    var dbSet = _context.GetDbSet<TKpi>(typeof(TKpi));
        //    var kpiForHisOwner = dbSet.FirstOrDefault(lambda);

        //    if (kpiForHisOwner is default(TKpi)) // is null
        //    {
        //        kpiForHisOwner = ObjectCreation.CreateInstanceUsingLamdaExpression<TKpi>();

        //        var ownerIdSetter = ObjectCreation.GetPropertySetter<TKpi, string>(kpiForHisOwner, "OwnerId");
        //        ownerIdSetter(kpiForHisOwner, report.OwnerId);

        //        var eduYearSetter = ObjectCreation.GetPropertySetter<TKpi, string>(kpiForHisOwner, "EducationalYearId");
        //        eduYearSetter(kpiForHisOwner, year.EducationalYearId);

        //        var kpiNumeratorSetter = ObjectCreation.GetPropertySetter<TKpi, decimal>(kpiForHisOwner, "KpiNumerator");
        //        kpiNumeratorSetter(kpiForHisOwner, 1m);

        //        var kpiDenominatorSetter = ObjectCreation.GetPropertySetter<TKpi, long>(kpiForHisOwner, "KpiDenominator");
        //        kpiDenominatorSetter(kpiForHisOwner, 1);

        //        _context.Add(kpiForHisOwner);
        //        await _context.SaveChangesAsync();
        //    }


        //    if (_context.Entry(kpiForHisOwner).Collection(x => x.KpiRecords).Query().Count() < 1) // mean menotrKpi has the default value: 1/1
        //    {
        //        SetReportKpiThatHasNoRecords(kpiForHisOwner, kpiRecord);
        //    }
        //    else
        //    {
        //        SetReportKpiThatHasRecords(kpiForHisOwner, kpiRecord);
        //    }
        //    kpiForHisOwner.KpiRecords.Add(kpiRecord);
        //    await _context.SaveChangesAsync();

        //    return kpiForHisOwner;
        //}
        #endregion
    }
}
