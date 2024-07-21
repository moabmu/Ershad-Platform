using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ModepEduYanbu.Data;
using ModepEduYanbu.Repositories;
using ModepEduYanbu.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModepEduYanbu.Models.DashboardViewModels;
using ModepEduYanbu.Helpers.Extensions;
using ModepEduYanbu.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Sakura.AspNetCore;
using Microsoft.AspNetCore.Http;
using ModepEduYanbu.Helpers.Common;
using ModepEduYanbu.DAL.DbContexts;
using ModepEduYanbu.ViewModels.Shared;
using ModepEduYanbu.ViewModels.ViewModelsForPartialViews;

namespace ModepEduYanbu.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        public readonly ISchoolRepo _schoolRepo;
        private readonly IEduProgramsRepo _eduProgramsRepo;
        private readonly IReportsRepo _reportsRepo;
        private readonly IAuthorizedPeopleRepo _authorizedPeopleRepo;
        private readonly IEducationalYearsRepo _educationalYearsRepo;

        public DashboardController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            ILoggerFactory loggerFactory,
            IConfiguration config,
            ApplicationDbContext context,
            ISchoolRepo schoolRepo,
            IEduProgramsRepo eduProgramsRepo,
            IReportsRepo reportsRepo,
            IAuthorizedPeopleRepo authorizedPeopleRepo,
            IEducationalYearsRepo educationalYearsRepo
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _config = config;
            _context = context;
            _schoolRepo = schoolRepo;
            _eduProgramsRepo = eduProgramsRepo;
            _reportsRepo = reportsRepo;
            _authorizedPeopleRepo = authorizedPeopleRepo;
            _educationalYearsRepo = educationalYearsRepo;
        }

        [HttpGet]
        [Authorize(Roles = Templates.mentorRoleUp)]
        public async Task<IActionResult> Index(string searchString = null,
            IEnumerable<string> schoolMinistryNos = null,
            IEnumerable<string> eduProgramIds = null,
            string showRatedOrUnratedReports = null,
            int page = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            var role = (await _userManager.GetRolesAsync(user)).ToList()[0];
            IQueryable<Report> reports = null;
            List<EduProgram> eduPrograms = _eduProgramsRepo.GetForMonth(DateTime.Now).ToList();
            EducationalYear year = null;

            if (role == "mentor" || role == "principle")
            {
                #region Validation
                // Search is not for School Employees
                if (!String.IsNullOrEmpty(searchString)) 
                {
                    return View("Error");
                }
                if(eduProgramIds.Count() > 0)
                {
                    return View("Error");
                }
                if(schoolMinistryNos.Count() > 0)
                {
                    return View("Error");
                }

                if (user.CurrentSchoolId == null)
                {
                    return RedirectToAction(nameof(SelectSchool));
                }

                // User try to access a school he is not belong to.
                if (!_schoolRepo.GetAllForUser(User.Identity.Name).Any(x => x == user.CurrentSchool))
                {
                    user.CurrentSchool = null;
                    user.CurrentSchoolId = null;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(SelectSchool));
                }
                #endregion

                year = _educationalYearsRepo.GetCurrentYearOrCreateIfNotExist();
                reports = _context.Reports
                    .Where(r => r.SchoolId == user.CurrentSchoolId && r.EducationalYearId == year.EducationalYearId)
                    .OrderByDescending(r => r.SentDateTime);

                if (role == "mentor")
                {
                    reports = reports.Where(r => r.OwnerId == user.Id);
                    // eduPrograms equals only programs that do not have reports by the user.
                    eduPrograms = eduPrograms.Where(x => !reports.Any(r => r.EduProgramId == x.EduProgramId)).ToList();
                    // Exclude eduPrograms that do not match mentor's school level.
                    var mentorSchools = _schoolRepo.GetAllForUser(user.UserName);
                    eduPrograms = eduPrograms.Where(prg => prg.SchoolLevels.Any(sl => mentorSchools.Any(ms => ms.LevelId == sl.SchoolLevelId))).ToList();
                }
                else if (role == "principle")
                {
                    var mentorRoleId = _roleManager.Roles.SingleOrDefault(r => r.Name == "mentor").Id;
                    var allMentorsInSchool = _userManager.Users.Include(x => x.Roles).Include(x => x.UserSchools)
                    .Where(mentor => mentor.UserSchools.Any(school => school.SchooId == user.CurrentSchoolId) && mentor.Roles.Any(r => r.RoleId == mentorRoleId))
                    .AsNoTracking()
                    .ToList();

                    if (allMentorsInSchool.Count > 0)
                    {
                        eduPrograms = eduPrograms.Where(
                            p => !allMentorsInSchool.All(mentor => reports.Any(r => r.EduProgramId == p.EduProgramId && r.OwnerId == mentor.Id))
                            ).ToList();
                    }
                    else
                    {
                        eduPrograms = eduPrograms.Where(x => !reports.Any(r => r.EduProgramId == x.EduProgramId))
                            .ToList();
                    }
                }
            }
            else
            {
                var educationalYearId = HttpContext.Request.Cookies[Cookies.EducationalYear];
                if (!String.IsNullOrEmpty(educationalYearId))
                {
                    year = _educationalYearsRepo.GetById(educationalYearId);
                    if (year == null)
                        return RedirectToAction(nameof(SelectEducationalYear), "Dashboard");

                    ViewBag.CurrentArchive = year.Name;
                    ViewBag.ShowChangeArchiveButton = _educationalYearsRepo.GetAll().Count() > 1;
                }
                else
                {
                    return RedirectToAction(nameof(SelectEducationalYear), "Dashboard");
                }
                reports = _context.Reports
                    .Where(x => x.IsSignedByPrinciple && x.EducationalYearId == year.EducationalYearId);

                // School List
                List<SelectListItem> schoolsForFilterList = _context.Schools
                    .Select(x => new SelectListItem { Text = $"{x.Name} - {x.MinistryNo}", Value = x.MinistryNo })
                    .ToList();
                if (schoolsForFilterList != null && schoolsForFilterList.Count > 0)
                {
                    ViewBag.SchoolsList = schoolsForFilterList;
                }

                // Program List
                var eduProgramsForFilterList = _context.EduPrograms
                    .Include(x => x.EducationalYear)
                    .Where(x => x.EducationalYear == year)
                    .Select(x => new SelectListItem { Text = x.Name, Value = x.EduProgramId })
                    .ToList();
                if (eduProgramsForFilterList != null && eduProgramsForFilterList.Count > 0)
                {
                    ViewBag.EduProgramsList = eduProgramsForFilterList;
                }   

                // Years List
                var educationalYearsForFilterList = _educationalYearsRepo.GetAll()
                    .Select(x => new SelectListItem { Text = x.ShortName, Value = x.EducationalYearId }).ToList();
                if (educationalYearsForFilterList != null && educationalYearsForFilterList.Count > 0)
                {
                    ViewBag.EducationalYearsList = educationalYearsForFilterList;
                }

                // List for search filter (Show rated report, show unrated reports)
                var showRatedReportsOptionStr = "ShowRatedReports";
                var showUnratedReportsOptionStr = "ShowUnratedReports";
                ViewBag.ShowRatedOrUnratedReportsList = new List<SelectListItem> { 
                    new SelectListItem{ Text = "إظهار التقارير المقيمة", Value = showRatedReportsOptionStr},
                    new SelectListItem{ Text = "إظهار التقارير غير المقيمة", Value = showUnratedReportsOptionStr}
                };

                if (schoolMinistryNos != null && schoolMinistryNos.Count() > 0)
                    reports = reports.Where(report => schoolMinistryNos.Any(s => s == report.SchoolMinistryNo));
                
                if (eduProgramIds != null && eduProgramIds.Count() > 0)
                    reports = reports.Where(report => eduProgramIds.Any(prog => prog == report.EduProgramId));
                
                if(!String.IsNullOrEmpty(showRatedOrUnratedReports))
                {
                    if (showRatedOrUnratedReports == showRatedReportsOptionStr)
                        reports = reports.Where(x => x.IsEvaluated);
                    else if (showRatedOrUnratedReports == showUnratedReportsOptionStr)
                        reports = reports.Where(x => !x.IsEvaluated);
                    else
                        return View("Error");
                }

                ViewData["searchString"] = searchString;
                if (!String.IsNullOrEmpty(searchString))
                {
                    reports = reports.Where(x =>
                    x.ReportNo == searchString
                    || x.SchoolMinistryNo == searchString
                    || x.SchoolName.Contains(searchString)
                    || x.SigningPrincipleFullName.Contains(searchString)
                    || x.OwnerFullName.Contains(searchString)
                    || x.EduProgramName.Contains(searchString)
                    );
                }
                reports = reports.OrderByDescending(r => r.SigningDateTime);
            }
            var topSchools = _context.ReportsKpisForSchools.Include(x => x.Owner)
                .Where(x => x.EducationalYearId == year.EducationalYearId)
                .OrderByDescending(x => x.KpiNumerator / x.KpiDenominator).Take(5).AsNoTracking();
            foreach (var s in topSchools)
            {
                var mm = Math.Round(s.KpiNumerator / s.KpiDenominator * 10, 2);
            }
            var topMentors = _context.ReportsKpisForMentors.
                Include(x => x.Owner)
                .ThenInclude(x => x.UserSchools)
                .ThenInclude(x => x.School)
                .Where(x => x.EducationalYearId == year.EducationalYearId)
                .OrderByDescending(x => x.KpiNumerator / x.KpiDenominator).Take(3).AsNoTracking();

            var reportsViewerModel = new PartialReportsViewerViewModel
            {
                DisplayPager =true,
                DisplayEditReportButton = role == "mentor",
                DisplayTotalCountOfReports = true,
                ErrorMessageWhenReportsNull = (role == "mentor" || role == "principle") ? 
                    "لا توجد تقارير مرسلة من طرفكم حتى الآن." : "لا توجد نتائج.",
                HtmlId = "SentReports",
                Reports = reports.Select(r => new ReportSummary
                {
                    ReportNo = r.ReportNo,
                    EduProgramName = r.EduProgramName,
                    SchoolName = r.SchoolName,
                    OwnerFullName = r.OwnerFullName,
                    IsEvaluated = r.IsEvaluated,
                    Evaluation = r.Evaluation,
                    EvaluationDate = r.EvaluationDate,
                    IsSignedByPrinciple = r.IsSignedByPrinciple,
                    VisitorRatingCount = r.VisitorRatingCount,
                    VisitorOverallRating = r.VisitorOverallRating
                })
                .AsNoTracking()
                .AsEnumerable()
                .ToPagedList(30, page)
            };

            var model = new DashboardViewModel
            {
                EduPrograms = eduPrograms,
                PagedReportsViewer = reportsViewerModel,
                TopSchools = topSchools,
                TopMentors = topMentors
            };// await reports.ToListAsync() });
            var sumNum = 0m;
            await _context.ReportsKpisForEduPrograms.Where(x => x.EducationalYearId == year.EducationalYearId).ForEachAsync(x => sumNum += (x.KpiNumerator / x.KpiDenominator));
            var kpisCount = _context.ReportsKpisForEduPrograms.Where(x => x.EducationalYearId == year.EducationalYearId).Count();
            var kpiResult = (kpisCount != 0) ? Math.Round((sumNum / kpisCount) * 100, 2) : (decimal)kpisCount;
            ViewBag.DepartmentKpi = kpiResult;
            //ViewBag.DepartmentKpi = 92.34m;
            ViewBag.VisitorsCount = _context.ReportActivities.
                Include(x => x.Report).
                Where(x => x.Report.EducationalYearId == year.EducationalYearId).
                Count();
            ViewBag.CurrentEduProgramsCount = (model.EduPrograms == null || model.EduPrograms.Count < 1) ? 0 : model.EduPrograms.Count;
            ViewBag.FinishedEduProgramsCount = _context.EduPrograms.Count(x => x.EducationalYearId == year.EducationalYearId) - (int)ViewBag.CurrentEduProgramsCount;
            ViewBag.NonEvaluatedReports = _context.Reports.Where(x => x.EducationalYearId == year.EducationalYearId && !x.IsEvaluated && x.IsSignedByPrinciple).Count();
            ViewBag.EvaluatedReports = _context.Reports.Where(x => x.EducationalYearId == year.EducationalYearId && x.IsEvaluated && x.IsSignedByPrinciple).Count();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = Templates.eduemployeeRoleUp)]
        public async Task<IActionResult> ReportKpisList(string target = null,
            string order = null,
            string searchString = null,
            int page = 1)
        {
            IActionResult getSelectedYear = GetSelectedEducationalYear(out EducationalYear year, out string educationalYearId);
            if (getSelectedYear != null) return getSelectedYear;

            // Order List
            ViewBag.OrderList = new List<SelectListItem> {
                new SelectListItem{ Text= "بدون", Value= null},
                new SelectListItem{ Text= "ترتيب تنازلي", Value= "desc"},
                new SelectListItem{ Text= "ترتيب تصاعدي", Value= "asc"}
            };
            ViewData["SearchString"] = searchString;

            IQueryable<ReportKpisListItemViewModel> model = null;
            switch (target)
            {
                case "schools":
                    ViewData["Target"] = "schools";
                    ViewData["TargetTitle"] = "المدارس";
                    model = _context.ReportsKpisForSchools
                        .Include(x => x.Owner)
                        .Where(x => x.EducationalYearId == year.EducationalYearId)
                        .Select(x => new ReportKpisListItemViewModel
                        {
                            OwnerName = x.Owner.Name,
                            OwnerId = x.OwnerId,
                            KpiId = x.KpiId,
                            KpiDenominator = x.KpiDenominator,
                            KpiNumerator = x.KpiNumerator,
                            KpiUpdatingDate = x.KpiUpdatingDate
                        });
                    ;
                    break;
                case "mentors":
                    ViewData["Target"] = "mentors";
                    ViewData["TargetTitle"] = "المرشدين";
                    model = _context.ReportsKpisForMentors
                        .Include(x => x.Owner)
                        .ThenInclude(x => x.UserSchools)
                        .ThenInclude(x => x.School)
                        .Where(x => x.EducationalYearId == year.EducationalYearId)
                        .Select(x => new ReportKpisListItemViewModel
                        {
                            OwnerName = x.Owner.FullName, //$"{x.Owner.FullName} (المدرسة: {((x.Owner.UserSchools.Count > 0)?x.Owner.UserSchools[0].School.Name:"\"بدون مدرسة حالياً\"")})"
                            OwnerId = x.OwnerId,
                            KpiId = x.KpiId,
                            KpiDenominator = x.KpiDenominator,
                            KpiNumerator = x.KpiNumerator,
                            KpiUpdatingDate = x.KpiUpdatingDate
                        });
                    ;
                    break;
                case "eduprograms":
                    ViewData["Target"] = "eduprograms";
                    ViewData["TargetTitle"] = "البرامج الإرشادية";
                    model = _context.ReportsKpisForEduPrograms
                        .Include(x => x.Owner)
                        .Where(x => x.EducationalYearId == year.EducationalYearId)
                        .Select(x => new ReportKpisListItemViewModel
                        {
                            OwnerName = x.Owner.Name,
                            OwnerId = x.OwnerId,
                            KpiId = x.KpiId,
                            KpiDenominator = x.KpiDenominator,
                            KpiNumerator = x.KpiNumerator,
                            KpiUpdatingDate = x.KpiUpdatingDate
                        });
                    ;
                    break;
                default:
                    return View("Error");
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.OwnerName.Contains(searchString));
            }

            switch (order)
            {
                case "desc":
                    model = model.OrderByDescending(x => x.KpiNumerator / x.KpiDenominator);
                    break;
                case "asc":
                    model = model.OrderBy(x => x.KpiNumerator / x.KpiDenominator);
                    break;
            }
            return View(model.AsNoTracking().AsEnumerable().ToPagedList(50, page));
        }

        [HttpGet]
        [Authorize(Roles = Templates.adminRoleUp)]
        public async Task<IActionResult> RatedAndUnratedEduProgramsList(int page = 1)
        {
            // Get current year
            IActionResult getSelectedYear = GetSelectedEducationalYear(out EducationalYear educationalYear, out string educationalYearId);
            if (getSelectedYear != null) return getSelectedYear;

            var model = _context.EduPrograms
                .Include(x => x.Reports)
                .Where(x => x.EducationalYearId == educationalYearId)
                .Select(x => new RatedAndUnratedEduProgramsItemViewModel
                {
                    Name = x.Name,
                    ReportDeadline = x.ReportDeadline,
                    SentReportsCount = x.Reports.Count(r => r.IsSignedByPrinciple),
                    RatedReportsCount = x.Reports.Count(r => r.IsEvaluated),
                    RouteDataForUnratedLink = new Dictionary<string, string>
                    {
                        {"showRatedOrUnratedReports","ShowUnratedReports"}, 
                        {"eduProgramIds", x.EduProgramId}
                    }
                })
                .OrderByDescending(x => x.ReportDeadline)
                .AsNoTracking();

            return View(model.AsEnumerable().ToPagedList(50, page));
        }

        [HttpGet]
        [Authorize(Roles = "principle,mentor")]
        public async Task<IActionResult> SelectSchool()
        {
            var user = await _userManager.GetUserAsync(User);
            var isUserSchoolEmp = await user.IsSchoolEmployee(_userManager);
            var schools = (isUserSchoolEmp) ?
                _schoolRepo.GetAllForUser(User.Identity.Name) : null;// _schoolRepo.GetAll().ToList();

            if (schools == null || schools.Count == 0)
            {
                return View("Error");
            }
            else if (schools.Count == 1)
            {
                user.CurrentSchool = schools[0];
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                var schoolsList = schools
                    .Select(s => new SelectListItem { Text = s.Name, Value = s.SchoolId }).ToList();
                return View(new SelectSchoolViewModel { Schools = schoolsList });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "principle,mentor")]
        public async Task<IActionResult> SelectSchool(SelectSchoolViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            user.CurrentSchool = _schoolRepo.GetById(model.SelectedSchool);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        [Authorize(Roles = Templates.eduemployeeRoleUp)]
        public IActionResult SelectEducationalYear()
        {
            var years = _educationalYearsRepo.GetAll();
            if (years == null || years.Count() < 1)
            {
                _educationalYearsRepo.GetCurrentYearOrCreateIfNotExist();
                years = _educationalYearsRepo.GetAll();
            }
            if (years.Count() == 1 && String.IsNullOrEmpty(HttpContext.Request.Cookies["EducationalYear"]))
            {
                Cookies.CreateCookie(HttpContext, Cookies.EducationalYear, years.ToList()[0].EducationalYearId, Cookies.EducationalYearExpireInDays);
                return RedirectToAction("Index", "Dashboard");
            }
            else if (years.Count() > 1)
            {
                return View(new SelectEducationalYearViewModel
                {
                    Years = years.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.EducationalYearId
                    }).ToList()
                });
            }
            return View("Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Templates.eduemployeeRoleUp)]
        public IActionResult SelectEducationalYear(SelectEducationalYearViewModel model)
        {
            var selectedYear = _educationalYearsRepo.GetById(model.SelectedYear);
            if (selectedYear == null) return View("Error");
            Cookies.CreateCookie(HttpContext, Cookies.EducationalYear, model.SelectedYear, Cookies.EducationalYearExpireInDays);
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        [Authorize(Roles = Templates.principleRoleUp)]
        public async Task<IActionResult> AddPerson()
        {
            var user = await _userManager.GetUserAsync(this.User);
            var userRole = (await _userManager.GetRolesAsync(user)).ToList()[0];
            if (userRole == "mentor")
                return View("Error");

            var model = new AddPersonViewModel();
            if (user.GetCurrentSchool(_schoolRepo) != null)
            {
                model.SchoolMinistryNo = user.GetCurrentSchool(_schoolRepo).MinistryNo;
            }
            else
            {
                var schoolsList = this.GetSchoolsList();
                if (schoolsList.Count() > 0)
                {
                    ViewBag.SchoolsList = schoolsList.ToList();
                }
            }

            var rolesList = this.GetRolesListForRoleUp(userRole);
            model.Roles = rolesList.ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Templates.principleRoleUp)]
        public async Task<IActionResult> AddPerson(AddPersonViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User); // The user who will add a new person.
            var userRole = (await _userManager.GetRolesAsync(user)).ToList()[0];

            if (user.GetCurrentSchool(_schoolRepo) != null)
            {
                var person = _authorizedPeopleRepo.GetByIdNo(model.IdNo);
                if (person != null &&
                    person.SchoolMinistryNo == user.GetCurrentSchool(_schoolRepo).MinistryNo)
                {
                    ModelState.AddModelError(string.Empty, "يوجد شخص مضاف إلى النظام بنفس رقم الهوية المدخل ومعين لنفس مدرسة قائد المدرسة.");
                    var rolesList = this.GetRolesListForRoleUp(userRole);
                    model.Roles = rolesList.ToList();
                    return View(model);
                }

                var personAsUser = await _userManager.FindByNameAsync(model.IdNo);
                if (personAsUser != null)
                {
                    ModelState.AddModelError(string.Empty, "لا يمكن إضافة الشخص مع رقم الهوية المدخل.");
                    var rolesList = this.GetRolesListForRoleUp(userRole);
                    model.Roles = rolesList.ToList();
                    return View(model);
                }
            }
            else
            {
                // Prevent super roles for a school employee
                if (!String.IsNullOrEmpty(model.SchoolMinistryNo) && Templates.eduemployeeRoleUp.Split(',').Any(x => x == model.SelectedRole))
                {
                    ModelState.AddModelError(string.Empty, "لا يمكن منح الصلاحية المختارة لموظف يعمل بمدرسة.");
                    var rolesList = this.GetRolesListForRoleUp(userRole);
                    model.Roles = rolesList.ToList();
                    var schoolsList = this.GetSchoolsList();
                    if (schoolsList.Count() > 0)
                    {
                        ViewBag.SchoolsList = schoolsList.ToList();
                    }
                    model.SchoolMinistryNo = null;
                    return View(model);
                }
                else
                {
                    // TO DO: Validate School Ministry No is correct
                    if (_schoolRepo.GetByMinistryNo(model.SchoolMinistryNo) == null)
                    {
                        ModelState.AddModelError(string.Empty, "الرجاء اختيار المدرسة.");
                        var rolesList = this.GetRolesListForRoleUp(userRole);
                        model.Roles = rolesList.ToList();
                        var schoolsList = this.GetSchoolsList();
                        if (schoolsList.Count() > 0)
                        {
                            ViewBag.SchoolsList = schoolsList.ToList();
                        }
                        model.SchoolMinistryNo = null;
                        return View(model);
                    }
                }
            }

            var authorizedPerson = new AuthorizedPerson
            {
                IdNo = model.IdNo,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                ViaNoor = false,
                CreatedData = DateTime.Now,
                SchoolMinistryNo = (user.GetCurrentSchool(_schoolRepo) == null) ? model.SchoolMinistryNo : user.GetCurrentSchool(_schoolRepo).MinistryNo,
                AddedByUser = user,
                Role = await _roleManager.FindByNameAsync(model.SelectedRole)
            };
            var isAuthorizedPersonAdded = _authorizedPeopleRepo.Add(authorizedPerson) != null ? true : false;
            if (!isAuthorizedPersonAdded)
            {
                ModelState.AddModelError(string.Empty, "لا يمكن إضافة الشخص مع رقم الهوية المدخل. تأكد من عدم تسجيل المستخدم مسبقاً بالنظام أو من عدم حمله لصلاحية مختلفة عن الصلاحية المخرنة مسبقاً للشخص على النظام.");
                var rolesList = this.GetRolesListForRoleUp(userRole);
                model.Roles = rolesList.ToList();
                if (user.GetCurrentSchool(_schoolRepo) == null)
                {
                    var schoolsList = this.GetSchoolsList();
                    if (schoolsList.Count() > 0)
                    {
                        ViewBag.SchoolsList = schoolsList.ToList();
                    }
                    model.SchoolMinistryNo = null;
                }
                return View(model);
            }
            var personAdded = await _authorizedPeopleRepo.SaveChangesAsync();
            if (!personAdded)
            {
                ModelState.AddModelError(string.Empty,
                    "حدث خطأ أثناء حفظ البيانات، يرجى المحاولة مرة أخرى.");
                var rolesList = this.GetRolesListForRoleUp(userRole);
                model.Roles = rolesList.ToList();
                if (user.GetCurrentSchool(_schoolRepo) == null)
                {
                    var schoolsList = this.GetSchoolsList();
                    if (schoolsList.Count() > 0)
                    {
                        ViewBag.SchoolsList = schoolsList.ToList();
                    }
                    model.SchoolMinistryNo = null;
                }
                return View(model);

            }

            ModelState.Clear();
            ViewBag.UserMessage = "تمت الإضافة بنجاح، بإمكان الشخص الآن التسجيل بالنظام بواسطة رقم الهوية.";
            return View();
        }

        #region Helpers
        private IEnumerable<SelectListItem> GetRolesListForRoleUp(string roleName)
        {
            var rolesList = new List<SelectListItem>();
            switch (roleName)
            {
                case "sysowner":
                    rolesList.Add(new SelectListItem { Text = "مالك النظام", Value = "sysowner" });
                    goto case "admin";
                case "admin":
                    rolesList.Add(new SelectListItem { Text = "مدير النظام", Value = "admin" });
                    goto case "eduemployee";
                case "eduemployee":
                    rolesList.Add(new SelectListItem { Text = "موظف بقسم التوجيه والإرشاد", Value = "eduemployee" });
                    rolesList.Add(new SelectListItem { Text = "قائد مدرسة", Value = "principle" });
                    goto case "principle";
                case "principle":
                    rolesList.Add(new SelectListItem { Text = "مرشد طلابي", Value = "mentor" });
                    break;
            }
            return rolesList;
        }

        private IEnumerable<SelectListItem> GetSchoolsList()
        {
            var schools = _schoolRepo.GetAll();
            List<SelectListItem> schoolsList = new List<SelectListItem>();
            foreach (var s in schools)
            {
                schoolsList.Add(new SelectListItem { Text = s.Name, Value = s.MinistryNo });
            }

            return schoolsList;
        }

        /// <summary>
        /// Get the year selected for the current archive by eduEmployee, admin, and system owner.
        /// </summary>
        /// <param name="educationalYear"></param>
        /// <param name="educationalYearId"></param>
        /// <returns></returns>
        private IActionResult GetSelectedEducationalYear(out EducationalYear educationalYear, out string educationalYearId)
        {
            educationalYear = null;
            educationalYearId = HttpContext.Request.Cookies[Cookies.EducationalYear];
            if (!String.IsNullOrEmpty(educationalYearId))
            {
                educationalYear = _educationalYearsRepo.GetById(educationalYearId);
                if (educationalYear == null)
                    return RedirectToAction(nameof(SelectEducationalYear), "Dashboard");
            }
            else
            {
                return RedirectToAction(nameof(SelectEducationalYear), "Dashboard");
            }
            return null;
        }
        #endregion
    }
}