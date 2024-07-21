using Microsoft.EntityFrameworkCore;
using ModepEduYanbu.DAL.DbContexts;
using ModepEduYanbu.Data;
using ModepEduYanbu.Models;
using ModepEduYanbu.Repositories;
using ModepEduYanbu.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModepEduYanbu.DAL.Extensions;
using ModepEduYanbu.Models.Abstract;

namespace ModepEduYanbu.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var testLocalDbConnStr = "Server=(localdb)\\mssqllocaldb;Database=ershad_06MAR20;Trusted_Connection=True;MultipleActiveResultSets=true;";

            using (var context = new ApplicationDbContext(testLocalDbConnStr))
            {
                #region Commented
                //IKpiRepo kpiRepo = new DirtyKpiRepo(context);
                ////var linkingEduProgramsWithSchoolLevelsCount = LinkEduProgramsWithSchoolLevels(context);
                //ComputeKpisDirty(context, kpiRepo);

                //var eduProgram = 

                //Console.WriteLine($"ComputeKpisDirty, count: {context.SaveChanges()}");
                #endregion

                //// Fix eduPrograms have have no three levels
                //WriteLevelsForEduPrograms(context);
                //MakeAllEduProgramsHaveThreeLevels(context);

                //// Fix schools with no level
                //FixSchoolsThatHaveNoLevel(context);

                //// Fix EvaulationBase for Years
                //FixEvaluationBaseForYears(context);

                //// Compute Kpis for last years and current year (current year evaluated reports will not be included)
                //ComputeKpisVer2(context, new KpiRepo(context));

                //// Compute Kpis for finished eduPrograms for this year (both evaluated reports + non-sent reports)
                //// Target eduPrograms: 4b9921ac-7ed8-4bf5-9b80-569a6c03529e , de8c20fc-1537-402c-be04-63dea723bc7e
                //string[] eduProgramsIds = { "4b9921ac-7ed8-4bf5-9b80-569a6c03529e", "de8c20fc-1537-402c-be04-63dea723bc7e" };
                //ComputeKpisForFinishedEduProgramsThisYear(context, eduProgramsIds, new KpiRepo(context));

                #region Delete all records created by the system for the following programs
                //string[] eduProgramIds = { "308fb202-e7ee-4bef-a839-c56b5da9ef64", "31082454-b2ac-4dd3-bc87-01ba5eb3f6a9", "891cdbd4-e707-4fbe-bed9-172a56ff8cfb", "a66e3b9e-086f-41eb-aa9a-1d06156b5056" };
                ////string[] eduProgramsKpis = { "9efb66db-ab48-4e0a-9470-f89e2c94f3ec", "08d7ca2a-a81d-4a61-b2a7-be2589861d1c", "d7a31b81-7506-45cb-92a5-a28fe5e29a40", "48f18c22-02c8-422f-b65d-4e8a76c10f2e" };
                //IQueryable<ReportKpiRecord> recordsToDelete = context.HistoryRecordsForReportKpis
                //    .Include(x => x.ReportKpiForEduProgram_OwnsThisRecord)
                //    .Include(x => x.ReportKpiForMentor_OwnsThisRecord)
                //    .Include(x => x.ReportKpiForSchool_OwnsThisRecord)
                //    .Where(record => eduProgramIds.Any(prog => prog == record.EduProgramId));
                //if (recordsToDelete.Count() != 354)
                //    throw new Exception("count of records to delete is not 354");
                //DeleteKpiRecordsCreatedBySystem(context, recordsToDelete);
                //DeleteKpisWithNoRecords(context);
                //FixKpiDenominatorsWhenZero(context);
                #endregion

                #region fix kpi date to match the latest record kpi
                System.Linq.Expressions.Expression<Func<ReportKpiBase, bool>> predicate = x => x.EducationalYear == context.EducationalYears.OrderByDescending(year => year.EndDate).First();
                Action<ReportKpiBase> action = kpi =>
                {

                    kpi.KpiUpdatingDate = (kpi.KpiRecords != null && kpi.KpiRecords.Count() > 0) ?
                    kpi.KpiRecords.OrderByDescending(record => record.RecordDate).FirstOrDefault().RecordDate : kpi.KpiUpdatingDate;
                };

                var mentorsKpis = context.ReportsKpisForMentors
                    .Include(x => x.EducationalYear)
                    .Include(x => x.KpiRecords)
                    .Where(predicate);
                var eduProgramKpis = context.ReportsKpisForEduPrograms
                    .Include(x => x.EducationalYear)
                    .Include(x => x.KpiRecords)
                    .Where(predicate);
                var schoolsKpis = context.ReportsKpisForSchools
                    .Include(x => x.EducationalYear)
                    .Include(x => x.KpiRecords)
                    .Where(predicate);
                Console.WriteLine("Starting ...");
                Task.WhenAll(mentorsKpis.ForEachAsync(action), eduProgramKpis.ForEachAsync(action), schoolsKpis.ForEachAsync(action)).GetAwaiter().GetResult();
                Console.WriteLine(context.SaveChangesAsync().Result);
                #endregion
            }

            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        private static void DeleteKpiRecordsCreatedBySystem(ApplicationDbContext context, IQueryable<ReportKpiRecord> recordsToDelete)
        {
            // De-affect the value of record to the target KPIs (MentorKpi, EduProgramKpi, SchoolKpi)
            int proceedEduProgramKpis = 0, proceedMentorKpis = 0, proceedSchoolKpis = 0, proceedTotal = 0;
            foreach(var chuck in recordsToDelete.OrderBy(x=> x.EduProgramId).QueryChunksOfSize(100))
            {
                foreach(var record in chuck)
                {
                    ReportKpiForEduProgram eduProgramKpi = record.ReportKpiForEduProgram_OwnsThisRecord;
                    ReportKpiForMentor mentorKpi = record.ReportKpiForMentor_OwnsThisRecord;
                    ReportKpiForSchool schoolKpi = record.ReportKpiForSchool_OwnsThisRecord;

                    void directDecreaseKpi(ReportKpiBase k)
                    {
                        k.KpiNumerator -= record.RecordValue;
                        k.KpiDenominator--;
                    }

                    directDecreaseKpi(eduProgramKpi); 
                    proceedEduProgramKpis++;
                    directDecreaseKpi(mentorKpi); 
                    proceedMentorKpis++;
                    directDecreaseKpi(schoolKpi); 
                    proceedSchoolKpis++;
                }
                proceedTotal += context.SaveChanges();
            }
            Console.WriteLine($"{nameof(proceedEduProgramKpis)} Count:\t {proceedEduProgramKpis}");
            Console.WriteLine($"{nameof(proceedMentorKpis)} Count:\t {proceedMentorKpis}");
            Console.WriteLine($"{nameof(proceedSchoolKpis)} Count:\t {proceedSchoolKpis}");
            Console.WriteLine($"{nameof(proceedTotal)} by dbContext:\t {proceedTotal}");
            Console.WriteLine($"{nameof(proceedTotal)} == Other Counts: \t {proceedTotal == (proceedEduProgramKpis+proceedMentorKpis+proceedSchoolKpis)}");

            // Delete the records
            context.RemoveRange(recordsToDelete);
            int countDeletedRecords = context.SaveChanges();
            Console.WriteLine($"Count of deleted records:\t{countDeletedRecords}");
        }

        public static void DeleteKpisWithNoRecords(ApplicationDbContext context)
        {
            // Delete KPIs that have 1/1
            Func<ReportKpiBase, bool> predicate = x => x.KpiRecords.Count() < 1;
            var eduProgramKpisToDelete = context.ReportsKpisForEduPrograms.Include(x => x.KpiRecords).Where(predicate);
            var mentorKpisToDelete = context.ReportsKpisForMentors.Include(x => x.KpiRecords).Where(predicate);
            var schoolKpisToDelete = context.ReportsKpisForSchools.Include(x => x.KpiRecords).Where(predicate);
            Console.WriteLine($"{nameof(eduProgramKpisToDelete)}:\t {eduProgramKpisToDelete.Count()}");
            Console.WriteLine($"{nameof(mentorKpisToDelete)}:\t {mentorKpisToDelete.Count()}");
            Console.WriteLine($"{nameof(schoolKpisToDelete)}:\t {schoolKpisToDelete.Count()}");

            context.RemoveRange(eduProgramKpisToDelete);
            context.RemoveRange(mentorKpisToDelete);
            context.RemoveRange(schoolKpisToDelete);

            var totalKpisToDelete = context.SaveChanges();
            Console.WriteLine($"{nameof(totalKpisToDelete)}:\t {totalKpisToDelete}");
        }

        public static void FixKpiDenominatorsWhenZero(ApplicationDbContext context)
        {
            Func<ReportKpiBase, bool> predicate = x => x.KpiDenominator == 0;
            var eduProgramKpisToFix = context.ReportsKpisForEduPrograms.Include(x => x.KpiRecords).Where(predicate);
            var mentorKpisToFix = context.ReportsKpisForMentors.Include(x => x.KpiRecords).Where(predicate);
            var schoolKpisToFix = context.ReportsKpisForSchools.Include(x => x.KpiRecords).Where(predicate);

            void fixDenominator(IEnumerable<ReportKpiBase> kpisToFix)
            {
                foreach(var kpi in kpisToFix)
                {
                    kpi.KpiDenominator = kpi.KpiRecords.Count;
                }
                Console.WriteLine(context.SaveChanges());
            }
            fixDenominator(eduProgramKpisToFix);
            fixDenominator(mentorKpisToFix);
            fixDenominator(schoolKpisToFix);

            //Action<ReportKpiBase> action = x => {
            //    if (x.KpiDenominator == 0)
            //    {
            //        x.KpiDenominator = x.KpiRecords.Count();
            //    }
            //};

            //await context.ReportsKpisForEduPrograms.Include(x => x.KpiRecords).ForEachAsync(action);
            //await context.ReportsKpisForMentors.Include(x => x.KpiRecords).ForEachAsync(action);
            //await context.ReportsKpisForSchools.Include(x => x.KpiRecords).ForEachAsync(action);
            //Console.WriteLine($"Count of fixed denominator kpis: {await context.SaveChangesAsync()}");
        }

        private static void ComputeKpisForFinishedEduProgramsThisYear(ApplicationDbContext _context, string[] eduProgramsIds, IKpiRepo _kpiRepo)
        {
            var reportsCount = 0;
            foreach (var eduProgramId in eduProgramsIds)
            {
                Console.WriteLine($"EduProgram : {eduProgramId}");

                // 1. Adjust kpis for sent and evaluated reports for last years
                Console.WriteLine("Adjusting KPIs for sent and evaluated reports");
                int stepOneResult = 0;
                var sentAndEvaluatedReports = _context.Reports.Where(x => x.IsSignedByPrinciple && x.IsEvaluated && x.EducationalYearId == eduProgramId);
                Console.WriteLine(sentAndEvaluatedReports.Count());
                foreach (var chunk in sentAndEvaluatedReports.OrderBy(x => x.ReportId).QueryChunksOfSize(100))
                {
                    foreach (var report in chunk)
                    {
                        var evalRecord = new ReportKpiRecord
                        {
                            KpiRecordId = Guid.NewGuid().ToString(),
                            RecordValue = (decimal)(report.Evaluation / 10.0),
                            RecordCreator = report.Evaluator,
                            RecordCreatorId = report.EvaluatorId,
                            RecordDate = new DateTimeOffset(report.EvaluationDate.Value,
                                TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time").GetUtcOffset(report.EvaluationDate.Value)),
                            Report = report,
                            ReportId = report.ReportId
                        };

                        _kpiRepo.AdjustReportKpisAsync(report, evalRecord).Wait();
                        Console.WriteLine($"Report #{++reportsCount} {evalRecord.ReportId}");
                    }
                    stepOneResult += _context.SaveChanges();
                }
                Console.WriteLine($"\tCount of changes from the first step: {stepOneResult}");

                // 2. Adjust kpis for non-sent reports
                Console.WriteLine("\nAdjusting KPIs for non-sent reports");
                int stepTwoResult = 0;
                var mentorRoleId = _context.Roles.SingleOrDefault(r => r.Name == "mentor").Id;
                var eduPrograms = _context.EduPrograms.Where(x => x.EduProgramId == eduProgramId);
                foreach (var chunk in eduPrograms.OrderBy(x => x.EduProgramId).QueryChunksOfSize(50))
                {
                    foreach (var eduProgram in chunk)
                    {
                        _context.Entry(eduProgram).Collection(x => x.SchoolLevels).Load();
                        _context.Entry(eduProgram).Collection(x => x.Reports).Load();
                        // mentors did not send reports for eduProgram
                        var mentors = _context.Users
                            .Include(x => x.Roles)
                            .Where(mentor => mentor.Roles.Any(role => role.RoleId == mentorRoleId));
                        mentors = mentors
                            .Include(x => x.UserSchools)
                            .ThenInclude(x => x.School)
                            .Where(mentor => eduProgram.Reports == null || !eduProgram.Reports.Any(x => x.OwnerId == mentor.Id)); // Condition (eduProgram.Reports == null) because sometime the eduProgram.Reports is null, which makes an exception.
                        foreach (var mentorsChunk in mentors.OrderBy(x => x.Id).QueryChunksOfSize(50))
                        {
                            foreach (var m in mentorsChunk)
                            {
                                // FIXED>> Bug: All schools in the database do not have a LevelId. Import level from excel sheet.
                                var mentorSchoolEntity = (m.UserSchools.Count == 1) ? m.UserSchools[0] : m.UserSchools
                                    .FirstOrDefault(x => eduProgram.SchoolLevels.Any(level => level.SchoolLevelId == x.School.LevelId));
                                var mentorSchool = mentorSchoolEntity?.School;
                                if (mentorSchool is null) continue;
                                var evalRecord = new ReportKpiRecord
                                {
                                    KpiRecordId = Guid.NewGuid().ToString(),
                                    RecordValue = 0m,
                                    RecordDate = new DateTimeOffset(eduProgram.ReportDeadline,
                              TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time").GetUtcOffset(eduProgram.ReportDeadline)),
                                    EduProgram = eduProgram,
                                    EduProgramId = eduProgram.EduProgramId,
                                    Mentor = m,
                                    MentorId = m.Id,
                                    School = mentorSchool,
                                    SchoolId = mentorSchool.SchoolId
                                };
                                _kpiRepo.AdjustReportKpisAsync(null, evalRecord).Wait();
                            }
                            stepTwoResult += _context.SaveChanges();
                        }
                    }
                    stepTwoResult += _context.SaveChanges();
                }
                Console.WriteLine($"\tCount of changes from the second step: {stepTwoResult}");
            }
        }

        private static void FixEvaluationBaseForYears(ApplicationDbContext context)
        {
            foreach (var y in context.EducationalYears.Where(x => x.EvaluationBase != 10))
            {
                y.EvaluationBase = 10;
            }
            context.SaveChanges();
        }

        private static void FixSchoolsThatHaveNoLevel(ApplicationDbContext context)
        {
            var s1 = context.Schools.Find("0011f4ea-279b-494e-a726-7f025731e5a0");
            s1.LevelId = $"{(int)Templates.SchoolLevels.Secondary}";
            var s2 = context.Schools.Find("31c17ea0-b611-43d4-9818-848381e6d1d6");
            s2.Name = "إبتدائية القراصة للبنين";
            s2.LevelId = $"{(int)Templates.SchoolLevels.Primary}";
            var s3 = context.Schools.Find("bd6cbaab-5928-422f-b830-8bf51812c68e");
            s3.Name = "إبتدائية الخوارزمي";
            s3.LevelId = $"{(int)Templates.SchoolLevels.Primary}";
            var s4 = context.Schools.Find("d90b7cae-db96-4c9f-b2eb-83088d096911");
            s4.Name = "إبتدائية كعب بن زهير";
            s3.LevelId = $"{(int)Templates.SchoolLevels.Primary}";
            context.SaveChanges();
        }

        private static void MakeAllEduProgramsHaveThreeLevels(ApplicationDbContext context)
        {
            var corruptedEduPrograms = context.EduPrograms.Include(x => x.SchoolLevels).Where(x => x.SchoolLevels.Count() != 3);
            int progsCount = 0;
            foreach (var prog in corruptedEduPrograms)
            {
                context.Add(new EduProgramSchoolLevel { EduProgramId = prog.EduProgramId, SchoolLevelId = $"{(int)Templates.SchoolLevels.Primary}" });
                context.Add(new EduProgramSchoolLevel { EduProgramId = prog.EduProgramId, SchoolLevelId = $"{(int)Templates.SchoolLevels.Middle}" });
                context.Add(new EduProgramSchoolLevel { EduProgramId = prog.EduProgramId, SchoolLevelId = $"{(int)Templates.SchoolLevels.Secondary}" });
            }
            context.SaveChanges();
        }

        static void WriteLevelsForEduPrograms(ApplicationDbContext context)
        {
            var whenCountThree = context.EduPrograms.Where(x => x.SchoolLevels.Count() == 3).Count();
            var corruptedCount = context.EduPrograms.Where(x => x.SchoolLevels.Count() != 3).Count();
            Console.WriteLine($"Count of Programs with 3 levels: {whenCountThree} - Otherwise {corruptedCount}");
        }

        static void WriteYearsAndThisReports(ApplicationDbContext context)
        {
            var years = context.EducationalYears
                    .Include(x => x.EduPrograms)
                    .Include(x => x.Reports)
                    .ThenInclude(y => y.Evaluator)
                    .AsQueryable();
            foreach (var year in years)
            {
                var reportsCount = 0;
                //foreach(var prog in year.EduPrograms)
                //{
                //    context.Entry(prog).Collection(x => x.Reports).Load();
                //    reportsCount += prog.Reports.Count();
                //}
                Console.WriteLine($"Year: {year.BeginDate} - Programs Count: {year.EduPrograms.Count()} - Reports Count: {reportsCount} - Reports Count: {year.Reports.Count()}");
            }
        }

        static void ComputeKpisVer2(ApplicationDbContext _context, IKpiRepo _kpiRepo)
        {
            var years = _context.EducationalYears
                .Include(x => x.EduPrograms) // for step 2
                .Include(x => x.Reports) // for step 1
                .ThenInclude(y => y.Evaluator) // for step 1
                .Where(year => year.EducationalYearId != "de6a716a-4a43-4e30-a782-911b964c2b75")
                .AsQueryable();
            foreach (var year in years.ToList())
            {
                Console.WriteLine($"Year {year.EducationalYearId}");
                var reportsCount = 0;

                // 1. Adjust kpis for sent and evaluated reports for last years
                Console.WriteLine("Adjusting KPIs for sent and evaluated reports");
                int stepOneResult = 0;
                var sentAndEvaluatedReports = _context.Reports.Where(x => x.IsSignedByPrinciple && x.IsEvaluated && x.EducationalYearId == year.EducationalYearId);
                Console.WriteLine(sentAndEvaluatedReports.Count());
                foreach (var chunk in sentAndEvaluatedReports.OrderBy(x => x.ReportId).QueryChunksOfSize(100))
                {
                    foreach (var report in chunk)
                    {
                        var evalRecord = new ReportKpiRecord
                        {
                            KpiRecordId = Guid.NewGuid().ToString(),
                            RecordValue = (decimal)(report.Evaluation / 10.0),
                            RecordCreator = report.Evaluator,
                            RecordCreatorId = report.EvaluatorId,
                            RecordDate = new DateTimeOffset(report.EvaluationDate.Value,
                                TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time").GetUtcOffset(report.EvaluationDate.Value)),
                            Report = report,
                            ReportId = report.ReportId
                        };

                        _kpiRepo.AdjustReportKpisAsync(report, evalRecord).Wait();
                        Console.WriteLine($"Report #{++reportsCount} {evalRecord.ReportId}");
                    }
                    stepOneResult += _context.SaveChanges();
                }


                // 2. Adjust kpis for non-sent reports
                Console.WriteLine("\nAdjusting KPIs for non-sent reports");
                var mentorRoleId = _context.Roles.SingleOrDefault(r => r.Name == "mentor").Id;
                var eduPrograms = _context.EduPrograms.Where(x => x.EducationalYearId == year.EducationalYearId);
                Console.WriteLine($"Count of EduPrograms for this year: {eduPrograms.Count()}");
                foreach (var chunk in eduPrograms.OrderBy(x => x.EduProgramId).QueryChunksOfSize(50))
                {
                    foreach (var eduProgram in chunk)
                    {
                        _context.Entry(eduProgram).Collection(x => x.SchoolLevels).Load();
                        // mentors did not send reports for eduProgram
                        var mentors = _context.Users
                            .Include(x => x.Roles)
                            .Where(mentor => mentor.Roles.Any(role => role.RoleId == mentorRoleId));
                        mentors = mentors
                            .Include(x => x.UserSchools)
                            .ThenInclude(x => x.School)
                            .Where(mentor => eduProgram.Reports == null || !eduProgram.Reports.Any(x => x.OwnerId == mentor.Id)); // Condition (eduProgram.Reports == null) because sometime the eduProgram.Reports is null, which makes an exception.
                        foreach (var mentorsChunk in mentors.OrderBy(x => x.Id).QueryChunksOfSize(50))
                        {
                            foreach (var m in mentorsChunk)
                            {
                                // FIXED>> Bug: All schools in the database do not have a LevelId. Import level from excel sheet.
                                var mentorSchoolEntity = (m.UserSchools.Count == 1) ? m.UserSchools[0] : m.UserSchools
                                    .FirstOrDefault(x => eduProgram.SchoolLevels.Any(level => level.SchoolLevelId == x.School.LevelId));
                                var mentorSchool = mentorSchoolEntity?.School;
                                if (mentorSchool is null) continue;
                                var evalRecord = new ReportKpiRecord
                                {
                                    KpiRecordId = Guid.NewGuid().ToString(),
                                    RecordValue = 0m,
                                    RecordDate = new DateTimeOffset(eduProgram.ReportDeadline,
                              TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time").GetUtcOffset(eduProgram.ReportDeadline)),
                                    EduProgram = eduProgram,
                                    EduProgramId = eduProgram.EduProgramId,
                                    Mentor = m,
                                    MentorId = m.Id,
                                    School = mentorSchool,
                                    SchoolId = mentorSchool.SchoolId
                                };
                                _kpiRepo.AdjustReportKpisAsync(null, evalRecord).Wait();
                            }
                            _context.SaveChanges();
                        }
                    }
                    _context.SaveChanges();
                }
            }
            var result = _context.SaveChanges();
        }

        static void ComputeKpis(ApplicationDbContext _context, IKpiRepo _kpiRepo)
        {
            var years = _context.EducationalYears
                .Include(x => x.EduPrograms) // for step 2
                .Include(x => x.Reports) // for step 1
                .ThenInclude(y => y.Evaluator) // for step 1
                .AsQueryable();
            foreach (var year in years.ToList())
            {
                // 1. Adjust kpis for sent reports
                int stepOneResult = 0;
                foreach (var chunk in _context.Reports.Where(x => x.IsEvaluated && x.EducationalYearId == year.EducationalYearId).OrderBy(x => x.ReportId).QueryChunksOfSize(50))
                {
                    foreach (var report in chunk)
                    {
                        var evalRecord = new ReportKpiRecord
                        {
                            KpiRecordId = Guid.NewGuid().ToString(),
                            RecordValue = (decimal)(report.Evaluation / 10.0),
                            RecordCreator = report.Evaluator,
                            RecordCreatorId = report.EvaluatorId,
                            RecordDate = new DateTimeOffset(report.EvaluationDate.Value,
TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time").GetUtcOffset(report.EvaluationDate.Value)),
                            Report = report,
                            ReportId = report.ReportId
                        };

                        _kpiRepo.AdjustReportKpisAsync(report, evalRecord).Wait();
                    }
                    stepOneResult += _context.SaveChangesAsync().Result;
                }

                // 2. Adjust kpis for non-sent reports
                var mentorRoleId = _context.Roles.SingleOrDefault(r => r.Name == "mentor").Id;
                foreach (var chunk in _context.EduPrograms.Where(x => x.EducationalYearId == year.EducationalYearId).OrderBy(x => x.EduProgramId).QueryChunksOfSize(50))
                {
                    foreach (var eduProgram in chunk)
                    {
                        _context.Entry(eduProgram).Collection(x => x.SchoolLevels).Load();
                        // mentors did not send reports for eduProgram
                        var mentors = _context.Users
                            .Include(x => x.Roles)
                            .Where(mentor => mentor.Roles.Any(role => role.RoleId == mentorRoleId));
                        mentors = mentors
                            .Include(x => x.UserSchools)
                            .ThenInclude(x => x.School)
                            .Where(mentor => eduProgram.Reports == null || !eduProgram.Reports.Any(x => x.OwnerId == mentor.Id)); // Condition (eduProgram.Reports == null) because sometime the eduProgram.Reports is null, which makes an exception.
                        foreach (var mentorsChunk in mentors.OrderBy(x => x.Id).QueryChunksOfSize(50))
                        {
                            foreach (var m in mentorsChunk)
                            {
                                // Bug: All schools in the database do not have a LevelId. Import level from excel sheet.
                                var mentorSchoolEntity = (m.UserSchools.Count == 1) ? m.UserSchools[0] : m.UserSchools
                                    .FirstOrDefault(x => eduProgram.SchoolLevels.Any(level => level.SchoolLevelId == x.School.LevelId));
                                var mentorSchool = mentorSchoolEntity?.School;
                                if (mentorSchool is null) continue;
                                var evalRecord = new ReportKpiRecord
                                {
                                    KpiRecordId = Guid.NewGuid().ToString(),
                                    RecordValue = 0m,
                                    RecordDate = new DateTimeOffset(eduProgram.ReportDeadline,
                              TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time").GetUtcOffset(eduProgram.ReportDeadline)),
                                    EduProgram = eduProgram,
                                    EduProgramId = eduProgram.EduProgramId,
                                    Mentor = m,
                                    MentorId = m.Id,
                                    School = mentorSchool,
                                    SchoolId = mentorSchool.SchoolId
                                };
                                _kpiRepo.AdjustReportKpisAsync(null, evalRecord).Wait();
                            }
                            _context.SaveChanges();
                        }
                    }
                    _context.SaveChanges();
                }
            }
            var result = _context.SaveChanges();
        }

        static void ComputeKpisRefactorNeeded(ApplicationDbContext _context, IKpiRepo _kpiRepo)
        {
            var years = _context.EducationalYears
.Include(x => x.EduPrograms) // for step 2
.Include(x => x.Reports) // for step 1
.ThenInclude(y => y.Evaluator) // for step 1
.AsQueryable();
            foreach (var year in years.Where(x => x.EducationalYearId == "c8d30e1c-1e09-4a83-86e5-921d24e16d22"))
            {
                Console.WriteLine($"***** year {year.EducationalYearId} *****");
                Console.WriteLine($"EduPrograms.Count: {year.EduPrograms.Count()}");

                // 1. Adjust kpis for sent reports
                foreach (var report in _context.Reports
                    .Where(x => x.IsEvaluated && x.EducationalYearId == year.EducationalYearId
                    && x.SchoolId == "fc125617-b392-4f7a-9637-ae745bee8636"))
                {
                    var evalRecord = new ReportKpiRecord
                    {
                        KpiRecordId = Guid.NewGuid().ToString(),
                        RecordValue = (decimal)(report.Evaluation / 10.0),
                        RecordCreator = report.Evaluator,
                        RecordCreatorId = report.EvaluatorId,
                        RecordDate = new DateTimeOffset(report.EvaluationDate.Value,
TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time").GetUtcOffset(report.EvaluationDate.Value)),
                        Report = report,
                        ReportId = report.ReportId
                    };

                    _kpiRepo.AdjustReportKpisAsync(report, evalRecord).Wait();
                }
                //Console.WriteLine($"Adjust Kpis for sent reports, count: {_context.SaveChanges()}");

                // 2. Adjust kpis for non-sent reports
                var mentorRoleId = _context.Roles.SingleOrDefault(r => r.Name == "mentor").Id;
                var mentors = _context.Users
                    .Include(x => x.Roles)
                    .Include(x => x.UserSchools)
                    .ThenInclude(x => x.School)
                    .Where(mentor => mentor.Roles.Any(role => role.RoleId == mentorRoleId) &&
                                    mentor.UserSchools.Any(x => x.SchooId == "fc125617-b392-4f7a-9637-ae745bee8636"));
                var eduPrograms = _context.EduPrograms
                    .Include(prog => prog.Reports)
                    .Where(x => x.EducationalYearId == year.EducationalYearId);
                var eduProgramsLoopCounter = 0;
                var adjustkpiCounter = 0;
                foreach (var eduProgram in eduPrograms)
                {
                    //_context.Entry(eduProgram).Collection(x => x.SchoolLevels).Load();
                    // mentors did not send reports for eduProgram
                    mentors = mentors.Where(mentor => !eduProgram.Reports.Any(x => x.IsSignedByPrinciple && x.OwnerId == mentor.Id)); // Condition (eduProgram.Reports == null) because sometime the eduProgram.Reports is null, which makes an exception.
                    if (mentors.CountAsync().Result < 1)
                    {
                        throw new Exception($"mentors 0 when eduProgram is {eduProgram.EduProgramId}");
                    }
                    foreach (var m in mentors)
                    {
                        var usersSchools = m.UserSchools.Where(x => x.SchooId == "fc125617-b392-4f7a-9637-ae745bee8636");
                        foreach (var s in usersSchools)
                        {
                            var evalRecord = new ReportKpiRecord
                            {
                                KpiRecordId = Guid.NewGuid().ToString(),
                                RecordValue = 0m,
                                RecordDate = new DateTimeOffset(eduProgram.ReportDeadline,
TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time").GetUtcOffset(eduProgram.ReportDeadline)),
                                EduProgram = eduProgram,
                                EduProgramId = eduProgram.EduProgramId,
                                Mentor = m,
                                MentorId = m.Id,
                                School = s.School,
                                SchoolId = s.School.SchoolId
                            };
                            try
                            {
                                _kpiRepo.AdjustReportKpisAsync(null, evalRecord).Wait();
                                adjustkpiCounter++;
                            }
                            catch
                            {
                                Console.WriteLine(evalRecord);
                                Console.WriteLine(s);
                                Console.WriteLine(m);
                            }
                        }
                        //// Bug: All schools in the database do not have a LevelId. Import level from excel sheet.
                        //var mentorSchoolEntity = (m.UserSchools.Count == 1) ? m.UserSchools[0] : null;
                        //var mentorSchool = mentorSchoolEntity?.School;
                        //if (mentorSchool is null)
                        //{
                        //    Console.WriteLine($"Mentor {m.Id} does not have a school. EduProgram.Id: {eduProgram.EduProgramId}");
                        //    continue;
                        //}
                    }
                    eduProgramsLoopCounter++;
                }
                //Console.WriteLine($"Adjust Kpis for non-sent reports, count: {_context.SaveChanges()}");
                Console.WriteLine($"eduProgramsLoop: {eduProgramsLoopCounter}, adjustReportKpisLoop: {adjustkpiCounter}");
            }
        }


        static int LinkEduProgramsWithSchoolLevels(ApplicationDbContext _context)
        {
            // for all edu programs, link them with all school levels.
            //var eduPrograms = _context.EduPrograms.AsEnumerable();
            foreach (var x in _context.SchoolLevels)
            {
                foreach (var y in _context.EduPrograms)
                {
                    _context.Add(new EduProgramSchoolLevel { EduProgram = y, SchoolLevel = x });
                }
            }
            return _context.SaveChanges();
        }

    }
}
