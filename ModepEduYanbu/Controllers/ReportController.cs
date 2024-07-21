using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ModepEduYanbu.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using ModepEduYanbu.Data;
using ModepEduYanbu.Repositories.Interfaces;
using ModepEduYanbu.Models.ReportViewModel;
using AutoMapper;
using ModepEduYanbu.Models;
using Microsoft.AspNetCore.Identity;
using ModepEduYanbu.Helpers.Extensions;
using System.Collections;
using System.IO;
using Microsoft.AspNetCore.Http;
using ModepEduYanbu.Helpers.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModepEduYanbu.Helpers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using ModepEduYanbu.Services;
using System.Text;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using ModepEduYanbu.DAL.DbContexts;

namespace ModepEduYanbu.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportsRepo _reportsRepo;
        private readonly IEduProgramsRepo _eduProgramRepo;
        private readonly ISchoolRepo _schoolsRepo;
        private readonly IUsersRepo _usersRepo;
        private readonly IAzureContainersRepo _azureContainersRepo;
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ILogger<ReportController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ISmsSender _smsSender;
        private readonly IEmailSender _emailSender;
        private readonly IEducationalYearsRepo _educationalYearsRepo;
        private readonly IKpiRepo _kpiRepo;

        private const string EDU_PROGRAM_MISMATCHES_SCHOOL_LEVEL_ERR_MSG = "لا يمكن إضافة تقرير بسبب عدم تطابق مرحلة المدرسة الخاصة بالمرشد مع المراحل الدراسية المخصصة لهذا البرنامج الإرشادي";
        private const string MENTOR_ALREADY_ADDED_REPORT_ERR_MSG = "يوجد تقرير قام المستخدم الحالي برفعه مسبقا لهذا البرنامج الإرشادي.";

        // Get the default form options so that we can use them to set the default limits for
        // request body data
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public ReportController(IReportsRepo reportsRepo,
            IEduProgramsRepo eduProgramsRepo,
            ISchoolRepo schoolsRepo,
            IUsersRepo usersRepo,
            IAzureContainersRepo azureContainersRepo,
            IHostingEnvironment env,
            IConfiguration config,
            ILogger<ReportController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            ISmsSender smsSender,
            IEmailSender emailSender,
            IEducationalYearsRepo educationalYearsRepo,
            IKpiRepo kpiRepo)
        {
            _reportsRepo = reportsRepo;
            _eduProgramRepo = eduProgramsRepo;
            _schoolsRepo = schoolsRepo;
            _usersRepo = usersRepo;
            _azureContainersRepo = azureContainersRepo;
            _env = env;
            _config = config;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _smsSender = smsSender;
            _emailSender = emailSender;
            _educationalYearsRepo = educationalYearsRepo;
            _kpiRepo = kpiRepo;
        }

        public enum ReportMessageId
        {
            SignByPrincipleSuccess,
            EvaluateSuccess,
            AddResponseSuccess,
            AddActivitySuccess,
            DeleteActivitySuccess,
            DeleteFileSuccess
        }

        [HttpGet]
        [Authorize(Roles = "mentor")]
        public async Task<IActionResult> Add(string programId = null)
        {
            var eduProgram = _eduProgramRepo.GetById(programId);
            if (eduProgram == null)
            {
                return View("Error");
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            
            // Prevent mentor from adding a report when eduProgram.Level doesn't match his school level
            if (!IsMentorSchoolMatchesEduProgramLevel(user, eduProgram))
            {
                return BadRequest(EDU_PROGRAM_MISMATCHES_SCHOOL_LEVEL_ERR_MSG);
            }

            var userHasReport = _context.Reports.Any(x => x.OwnerIdNo == user.UserName && x.EduProgramId == programId);
            if (userHasReport)
            {
                return BadRequest(MENTOR_ALREADY_ADDED_REPORT_ERR_MSG);
            }

            ViewData["ProgramId"] = programId;
            ViewData["EduProgram"] = eduProgram;
            ViewData["UploadedDescFilesURL"] = _config["Uploads:EduProgramUploads"]
                .ToString().Replace("\\", "/");

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "eduemployee,sysowner")]
        public async Task<IActionResult> DeleteActivity(string id = null)
        {
            var errMsg = "معرف رأي الزائر غير صحيح.";
            if (String.IsNullOrEmpty(id))
                return BadRequest(errMsg);

            var activity = _context.ReportActivities.FirstOrDefault(x => x.ReportActivityId == id);
            if (activity == null)
                return BadRequest(errMsg);

            var report = _context.Reports.FirstOrDefault(x => x.ReportId == activity.ReportId);
            _context.ReportActivities.Remove(activity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Show), "Report", new { reportNo = report.ReportNo, message = ReportMessageId.DeleteActivitySuccess });
        }

        [HttpGet]
        [Authorize(Roles = "mentor")]
        public async Task<IActionResult> DeleteFile(string id = null)
        {
            var errMsg = "معرف الملف المطلوب حذفه غير صحيح.";
            if (String.IsNullOrEmpty(id))
                return BadRequest(errMsg);

            var file = _context.ReportUploads.Include(x => x.Report).FirstOrDefault(x => x.ReportUploadedFileId == id);
            if (file == null || file.Report == null)
                return BadRequest(errMsg);
            var rNo = file.Report.ReportNo;

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (file.Report.OwnerId != currentUser.Id)
            {
                // User is not the author of the report
                await _smsSender.SendSmsAsync("+966567894439", $"محاولة حذف شاهد تنفيذ من قبل مستخدم لا يملك ذلك الملف، اسمه: {currentUser.FullName}, الهوية: {currentUser.UserName}, جوال: {currentUser.PhoneNumber}. الملف {file.ReportUploadedFileId} {file.Report.SchoolName}|{file.Report.ReportNo}");
                return View("Account/AccessDenied");
            }

            _context.ReportUploads.Remove(file);
            await _context.SaveChangesAsync();
            await _azureContainersRepo.Delete(file.AzureContainer, file.Filename);
            return RedirectToAction(nameof(Show), "Report", new { reportNo = rNo, message = ReportMessageId.DeleteFileSuccess });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "mentor")]
        public async Task<IActionResult> Add(AddReportViewModel model, string programId)
        {
            var eduProgram = _eduProgramRepo.GetById(id: programId, includeEducationalYear: true);
            if (eduProgram == null)
            {
                return View("Error");
            }
            ViewData["ProgramId"] = programId;
            ViewData["EduProgram"] = eduProgram;
            ViewData["UploadedDescFilesURL"] = _config["Uploads:EduProgramUploads"]
                .ToString().Replace("\\", "/");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);

            // Prevent mentor from adding a report when eduProgram.Level doesn't match his school level
            if (!IsMentorSchoolMatchesEduProgramLevel(user, eduProgram))
            {
                return BadRequest(EDU_PROGRAM_MISMATCHES_SCHOOL_LEVEL_ERR_MSG);
            }

            var userHasReport = _context.Reports.Any(x => x.OwnerIdNo == user.UserName && x.EduProgramId == programId);
            if (userHasReport)
            {
                return BadRequest(MENTOR_ALREADY_ADDED_REPORT_ERR_MSG);
            }
            var school = user.GetCurrentSchool(_schoolsRepo);
            var report = Mapper.Map<Report>(model);

            report.EduProgram = eduProgram;
            report.EduProgramName = eduProgram.Name;
            report.EducationalYear = eduProgram.EducationalYear;
            report.EducationalYearId = eduProgram.EducationalYear.EducationalYearId;
            report.EducationalYearName = eduProgram.EducationalYear.Name;
            report.EducationalYearShortName = eduProgram.EducationalYear.ShortName;

            report.School = school;
            report.SchoolMinistryNo = school.MinistryNo;
            report.SchoolName = school.Name;

            report.Owner = user;
            report.OwnerFullName = user.FullName;
            report.OwnerIdNo = user.UserName;

            report.SentDateTime = DateTime.Now;

            _reportsRepo.Add(report);
            var isReportSaved = await _reportsRepo.SaveChangesAsync();
            if (!isReportSaved)
            {
                ViewBag.UserMessage = "حدث خطأ، يرجى المحاولة مرة أخرى.";
                return View(model);
            }

            #region Send SMS notifications for owner.
            //var smsMsg = $"تمت إضافة التقرير رقم {report.ReportNo} للبرنامج الإرشادي ({report.EduProgramName}). الرابط للتقرير https://moeduyanbu.com/report/show?reportNo={report.ReportNo}";
            //_smsSender.SendSmsAsync(user.PhoneNumber, smsMsg);

            ////var groupSmsRoles = Templates.eduemployeeRoleUpExceptSysowner;
            ////var eduUsersPhones = await _usersRepo.GetPhoneNumbersForUsersInRoles(groupSmsRoles);
            ////_smsSender.SendSmsAsync(eduUsersPhones, smsMsg);
            #endregion
            #region Send SMS to principle
            var ownerFirstLastNameArr = report.OwnerFullName.Split(' ');
            string ownerName;
            try { ownerName = $"{ownerFirstLastNameArr[0]} {ownerFirstLastNameArr[ownerFirstLastNameArr.Length - 1]}"; } catch { ownerName = report.OwnerFullName; };
            var smsMsg = $"تمت إضافة التقرير الإرشادي رقم {report.ReportNo} بواسطة {ownerName}.";
            try
            {
                var principle = await _userManager.FindByNameAsync(school.PrincipleIdNo);
                if (principle != null)
                {
                    _smsSender.SendSmsAsync(principle.PhoneNumber, smsMsg);
                }
            }
            catch { }
            #endregion
            //ViewBag.UserMessage = "تمت إضافة التقرير لنجاح.";
            ViewData["ReportNo"] = report.ReportNo;
            //return View(model);
            return RedirectToAction(nameof(UploadFiles), "report", new { no = report.ReportNo });
        }

        public async Task<IActionResult> AddResponse(AddResponseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var report = _reportsRepo.GetByReportNo(model.ReportNo, includeResponses: true);
            if (report == null)
            {
                return View("Error");
            }

            var response = new ReportResponse { Report = report, Content = model.Content, Owner = user, OwnerFullName = user.FullName, OwnerIdNo = user.UserName, CreatedDate = DateTime.Now };
            _reportsRepo.AddReponse(response, report);

            var result = await _reportsRepo.SaveChangesAsync();
            if (!result)
            {
                return View("Error");
            }

            try
            {
                #region Prepare for SMS
                var smsMsg =
                    $"رد جديد على التقرير رقم {report.ReportNo} من {response.OwnerFullName.GetFirstNameAr()}. رابط التقرير https://moeduyanbu.com/report/show?reportNo={report.ReportNo}";
                var phones = new List<string>();

                var mentor = await _userManager.FindByNameAsync(report.OwnerIdNo);
                if (mentor != null) { phones.Add(mentor.PhoneNumber); }

                var mentorSchool = mentor.GetCurrentSchool(_schoolsRepo);
                var principleIdNo = mentorSchool.PrincipleIdNo;
                var principle = await _userManager.FindByNameAsync(principleIdNo);
                if (principle != null) { phones.Add(principle.PhoneNumber); }

                //var responseOwnersIdNos = report.Responses.Select(x => x.OwnerIdNo).Distinct();
                //foreach (var u in responseOwnersIdNos)
                //{
                //    var userHavingResponse = await _userManager.FindByNameAsync(u);
                //    if (userHavingResponse == null 
                //        || String.IsNullOrEmpty(userHavingResponse.PhoneNumber))
                //        continue;
                //    if (!phones.Any(x => x == userHavingResponse.PhoneNumber))
                //    {
                //        phones.Add(userHavingResponse.PhoneNumber);
                //    }
                //}
                #endregion

                #region Prepare for Email
                var emailMsg = $"رد جديد على التقرير رقم {report.ReportNo} من {response.OwnerFullName.GetFirstNameAr()}. رابط التقرير <a=\"https://moeduyanbu.com/report/show?reportNo={report.ReportNo}\">https://moeduyanbu.com/report/show?reportNo={report.ReportNo}</a>.<br/>الرد:<br/><b>{response.Content}</b>";
                var emails = new List<string>() { mentor.Email, principle.Email };
                var responseOwnersIdNos = report.Responses.Select(x => x.OwnerIdNo).Distinct();
                foreach (var u in responseOwnersIdNos)
                {
                    var userHavingResponse = await _userManager.FindByNameAsync(u);
                    if (userHavingResponse == null
                        || String.IsNullOrEmpty(userHavingResponse.PhoneNumber))
                        continue;
                    if (!emails.Any(x => x == userHavingResponse.Email))
                    {
                        emails.Add(userHavingResponse.Email);
                    }
                }
                #endregion

                await Task.WhenAll(_smsSender.SendSmsAsync(phones, smsMsg), new Task(() => { emails.ForEach(x => _emailSender.SendEmailAsync(x, "", emailMsg)); }));
            }
            catch { }
            return RedirectToAction(nameof(Show), "Report", new { reportNo = model.ReportNo, message = ReportMessageId.AddResponseSuccess });
        }

        //var groupSmsRoles = Templates.eduemployeeRoleUpExceptSysowner;
        //var eduUsersPhones = await _usersRepo.GetPhoneNumbersForUsersInRoles(groupSmsRoles);
        //_smsSender.SendSmsAsync(eduUsersPhones, smsMsg);

        [HttpGet]
        [Authorize(Roles = "mentor")]
        public async Task<IActionResult> Edit(string no = null)
        {
            if (String.IsNullOrEmpty(no))
            {
                return View("Error");
            }

            var report = _reportsRepo.GetByReportNo(no);
            if (report == null)
            {
                return View("Error");
            }

            var eduProgram = _eduProgramRepo.GetById(report.EduProgramId);
            if (eduProgram == null)
            {
                return View("Error");
            }

            var user = await _userManager.GetUserAsync(User);
            var school = user.GetCurrentSchool(_schoolsRepo);
            if (report.OwnerIdNo != user.UserName)
            {
                // User is not the author of the report
                return View("Error");
            }
            if (report.SchoolMinistryNo != school.MinistryNo)
            {
                // User moved to another school
                return View("Error");
            }

            if (_eduProgramRepo.IsDeadlinePassed(eduProgram, DateTime.Now))
            {
                return View("Error");
            }

            if (report.IsSignedByPrinciple)
            {
                return BadRequest("لا يمكن تعديل التقرير بعد اعتماده بواسطة قائد المدرسة.");
            }


            ViewData["ProgramId"] = eduProgram.EduProgramId;
            ViewData["EduProgram"] = eduProgram;
            ViewData["ReportNo"] = report.ReportNo;
            ViewData["UploadedDescFilesURL"] = _config["Uploads:EduProgramUploads"]
                .ToString().Replace("\\", "/");

            var model = Mapper.Map<AddReportViewModel>(report);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "mentor")]
        public async Task<IActionResult> Edit(AddReportViewModel model, string no)
        {
            if (String.IsNullOrEmpty(no))
            {
                return View("Error");
            }

            var report = _reportsRepo.GetByReportNo(no);
            if (report == null || report.IsSignedByPrinciple)
            {
                return View("Error");
            }

            var eduProgram = _eduProgramRepo.GetById(report.EduProgramId);
            if (eduProgram == null)
            {
                return View("Error");
            }

            if (_eduProgramRepo.IsDeadlinePassed(eduProgram, DateTime.Now))
            {
                return View("Error");
            }

            ViewData["ProgramId"] = eduProgram.EduProgramId; ;
            ViewData["EduProgram"] = eduProgram;
            ViewData["ReportNo"] = report.ReportNo;
            ViewData["UploadedDescFilesURL"] = _config["Uploads:EduProgramUploads"]
                .ToString().Replace("\\", "/");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var school = user.GetCurrentSchool(_schoolsRepo);

            if (report.OwnerIdNo != user.UserName)
            {
                // User is not the author of the report
                return View("Error");
            }

            if (report.SchoolMinistryNo != school.MinistryNo)
            {
                // User moved to another school
                return View("Error");
            }


            report.ChallengesSolus = model.ChallengesSolus;
            report.ExecutionData = model.ExecutionData;
            report.ExecutionDate = DateTime.Parse(model.ExecutionDate);
            report.ExecutionPeriod = model.ExecutionPeriod;
            report.Field = model.Field;
            report.ParticipantsRatio = model.ParticipantsRatio;
            report.SentDateTime = DateTime.Now;
            report.TargetedCount = model.TargetedCount;
            report.TargetedSlice = model.TargetedSlice;
            report.UploadsLink = model.UploadsLink;

            var isReportSaved = await _reportsRepo.SaveChangesAsync();
            if (!isReportSaved)
            {
                ViewBag.UserMessage = "حدث خطأ، يرجى المحاولة مرة أخرى.";
                return View(model);
            }

            ViewBag.UserMessage = "تم تعديل التقرير لنجاح.";
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Show(string reportNo = null, ReportMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ReportMessageId.EvaluateSuccess ? "تمت إضافة التقييم للتقرير بنجاح."
                : message == ReportMessageId.SignByPrincipleSuccess ? "تم اعتماد التقرير بواسطة قائد المدرسة بنجاح."
                : message == ReportMessageId.AddResponseSuccess ? "تمت إضافة الرد بنجاح."
                : message == ReportMessageId.AddActivitySuccess ? "تمت إضافة رأي الزائر بنجاح."
                : message == ReportMessageId.DeleteActivitySuccess ? "تم حذف رأي الزائر بنجاح."
                : message == ReportMessageId.DeleteFileSuccess ? "تم حذف شاهد التنفيذ بنجاح."
                : "";

            #region Sample Model for designing the view
            //var model = new ShowReportViewModel
            //{
            //    ChallengesSolus = "جلب أطباء للتحدث عن مخاطر التدخين بحاجة لموافقة الشؤون الصحية.",
            //    EduProgramName = "مخاطر التدخين",
            //    ExecutionData = "تم التحدث في الطابور الصباحي عن مخاطر التدخين،إذا كانت إجابتك لا، فإليك المفاجأة.. فقد أثبتت دراسات عدة أن الجسم يتفاعل بشكل مختلف مع كل نوع من أنواع الطعام مما يكون له تأثير مباشر على الحالة المزاجية للشخص.",
            //    ExecutionPeriod = 10,
            //    ExecutionDate = DateTime.Now,
            //    Field = "تربوي",
            //    OwnerFullName = "بندر محمد الجهني",
            //    ParticipantsRatio = 85.5,
            //    ReportNo = "2002",
            //    SchoolName = "مدارس الحديثة الأهلية المتوسطة",
            //    SentDateTime = "2018/1/5",
            //    TargetedCount = 54,
            //    TargetedSlice = "طلاب المرحلة المتوسطة",
            //    UploadsLink = "http://google.com",
            //    Evaluation = 9.75,
            //    EvaluationDate = DateTime.Now,
            //    Responses = new List<ReportResponse> {
            //        new ReportResponse {
            //            OwnerFullName = "عواض الشهري",
            //            Content = "يرجى تضمين فقرة الأخيرة من النشط (مسرحية مخاطر التدخين) إلى بيانات التنفيذ.",
            //            CreatedDate = DateTime.Now
            //        },
            //        new ReportResponse {
            //            OwnerFullName = "حمدان الصيدلاني",
            //            Content = "يرجى التحقق من صحة تاريخ التنفيذ.",
            //            CreatedDate = DateTime.Now
            //        }
            //    }
            //};
            #endregion

            bool isUserSignedIn = _signInManager.IsSignedIn(this.User);
            bool includeResponses = isUserSignedIn; // includeResponses is a variable for view
            var report = _reportsRepo.GetByReportNo(reportNo, includeResponses, true, true);
            if (report == null)
                return View("Error");

            // Show archived reports only for EduEmployees
            var currentYear = _educationalYearsRepo.GetCurrentYearOrCreateIfNotExist();
            if (report.EducationalYearId != currentYear.EducationalYearId)
            {
                if (!isUserSignedIn)
                    return View("Error");

                var user = await _userManager.GetUserAsync(this.User);
                var role = (await _userManager.GetRolesAsync(user))[0];
                if (!Templates.EduEmployeeRolesUpToStringObjectsAsEnumerable().Any(x => x == role))
                {
                    return View("Error");
                }
            }

            var model = Mapper.Map<ShowReportViewModel>(report);

            ViewData["Images"] = model.Uploads.Where(x => Helpers.UploadedFileHelpers.GetFileType(x.Extension) == UploadedFileHelpers.FileType.Image).ToList();
            ViewData["Videos"] = model.Uploads.Where(x => Helpers.UploadedFileHelpers.GetFileType(x.Extension) == UploadedFileHelpers.FileType.Video).ToList();
            ViewData["Documents"] = model.Uploads.Where(x => Helpers.UploadedFileHelpers.GetFileType(x.Extension) == UploadedFileHelpers.FileType.Document).ToList();

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "mentor")]
        public async Task<IActionResult> UploadFiles(string no = null)
        {
            var report = _reportsRepo.GetByReportNo(no);
            #region Validations
            if (no == null || no == "" || report == null || report.IsSignedByPrinciple)
            {
                return View("Error");
            }


            var user = await _userManager.GetUserAsync(User);
            var school = user.GetCurrentSchool(_schoolsRepo);

            if (report.OwnerIdNo != user.UserName)
            {
                // User is not the author of the report
                return View("Account/AccessDenied");
            }

            if (report.SchoolMinistryNo != school.MinistryNo)
            {
                // User moved to another school
                return View("Error");
            }
            #endregion

            ViewData["ReportNo"] = report.ReportNo;
            return View();
        }

        #region UploadFiles HttpPost
        //[HttpPost]
        ////[Authorize(Roles = "mentor")]
        //public async Task<IActionResult> UploadFiles(IList<IFormFile> files, string no = null)
        //{
        //    //return Json(new { state = 0, message = string.Empty, count = files.Count });

        //    var report = _reportsRepo.GetByReportNo(no);
        //    #region Validations
        //    if (report == null)
        //    {
        //        return Ok(new { code = 1, status = BadRequest().StatusCode, message = "التقرير المطلوب غير متوفر." });
        //    }

        //    //var user = await _userManager.GetUserAsync(User);
        //    //var school = user.GetCurrentSchool(_schoolsRepo);

        //    //if (report.OwnerIdNo != user.UserName)
        //    //{
        //    //    // User is not the author of the report
        //    //    return View("Error");
        //    //}

        //    //if (report.SchoolMinistryNo != school.MinistryNo)
        //    //{
        //    //    // User moved to another school
        //    //    return View("Error");
        //    //}


        //    if (files == null || files.Count < 1)
        //    {
        //        return Ok(new { code = 1, status = BadRequest().StatusCode, message = "لا توجد ملفات لرفعها." });
        //    }
        //    #endregion

        //    ViewData["ReportNo"] = report.ReportNo;

        //    List<UploadFileResult> result = new List<UploadFileResult>();
        //    int uploadedFilesCount = 0;
        //    foreach (var file in files)
        //    {
        //        if (file != null)
        //        {
        //            await UploadedFileHelpers.ValidateFormFile(
        //                file,
        //                ModelState, UploadedFileHelpers.FileType.Document |
        //                UploadedFileHelpers.FileType.Image |
        //                UploadedFileHelpers.FileType.Video,
        //                (int)UploadedFileHelpers.FileSize.TwentyFiveMB,
        //                null
        //                );
        //        }
        //        else
        //        {
        //            continue;
        //        }

        //        if (!ModelState.IsValid)
        //        {
        //            continue;
        //        }

        //        string filePath = string.Empty;
        //        if (file.Length > 0)
        //        {
        //            try
        //            {
        //                var guidFileName = Guid.NewGuid().ToString();
        //                var fileExtension = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1);
        //                var fileName = $"{guidFileName}.{fileExtension}";

        //                #region Upload locally
        //                //var uploads = Path.Combine(_env.WebRootPath, _config["Uploads:ReportUploads"].ToString());
        //                //filePath = Path.Combine(uploads, $"{guidFileName}.{fileExtension}");
        //                //using (var stream = new FileStream(filePath, FileMode.Create))
        //                //{
        //                //    await file.CopyToAsync(stream);
        //                //}
        //                #endregion

        //                #region Upload to Cloud
        //                var azureUploadResult = await _azureContainersRepo.Upload(file, fileName);
        //                if (azureUploadResult == null) throw new Exception();

        //                var reportUpload = new ReportUploadedFile
        //                {
        //                    Report = report,
        //                    ReportId = report.ReportId,
        //                    Filename = fileName,
        //                    FileTitle = file.FileName,
        //                    Uri = azureUploadResult.Uri,
        //                    AzureBlobName = azureUploadResult.BlobName,
        //                    AzureContainer = azureUploadResult.ContainerName
        //                };
        //                report.Uploads.Add(reportUpload);
        //                await _context.SaveChangesAsync();
        //                #endregion

        //                uploadedFilesCount++;
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }
        //    }

        //    if (uploadedFilesCount < 1)
        //    {
        //        return Ok(new { code = 1, status = BadRequest().StatusCode, message = "حدث خطأ أثناء رفع الملفات، الرجاء التأكد من صحة أنواع البيانات وحجمها." });
        //    }

        //    var successMsg =
        //        $"تم إرفاق شواهد التنفيذ بنجاح. إجمالي الملفات المحملة {uploadedFilesCount} من أصل {files.Count} ملف. لمشاهدة التقرير <a href=\"https://moeduyanbu.com/report/show?reportNo=" + no + "\">انقر هنا.<a>";
        //    return Ok(new { code = 2, status = Ok().StatusCode, message = successMsg });
        //}
        #endregion

        [HttpPost]
        [Authorize(Roles = "mentor")]
        public async Task<IActionResult> UploadFile(List<IFormFile> files, string no = null)
        {
            var report = _reportsRepo.GetByReportNo(no, includeUploads: true);
            if (no == null || no == string.Empty ||
                files == null || files.Count != 1 || files[0].Length < 1
                || report == null || report.IsSignedByPrinciple)
            {
                return Ok(new { code = 1, status = Ok().StatusCode, message = "gfy" });
            }

            var eduProgram = _eduProgramRepo.GetById(report.EduProgramId);
            if (eduProgram == null)
            {
                return View("Error");
            }

            if (_eduProgramRepo.IsDeadlinePassed(eduProgram, DateTime.Now))
            {
                return View("Error");
            }

            var user = await _userManager.GetUserAsync(User);
            var school = user.GetCurrentSchool(_schoolsRepo);
            if (report.OwnerIdNo != user.UserName)
            {
                //// User is not the author of the report
                //return View("Account/AccessDenied");
                return Ok(new { code = 1, status = Ok().StatusCode, message = "Access Denied" });
            }

            if (report.SchoolMinistryNo != school.MinistryNo)
            {
                //// User moved to another school
                //return View("Error");
                return Ok(new { code = 1, status = Ok().StatusCode, message = "gfy" });
            }


            var file = files[0];
            if (file != null)
            {
                await UploadedFileHelpers.ValidateFormFile(
                    file,
                    ModelState, UploadedFileHelpers.FileType.Document |
                    UploadedFileHelpers.FileType.Image |
                    UploadedFileHelpers.FileType.Video,
                    (int)UploadedFileHelpers.FileSize.TwentyFiveMB,
                    null
                    );
            }
            else
            {
                //continue;
            }

            if (!ModelState.IsValid)
            {
                return Ok(new { code = 1, status = Ok().StatusCode, message = "gfy" });
            }

            string filePath = string.Empty;
            if (file.Length > 0)
            {
                try
                {
                    var guidFileName = Guid.NewGuid().ToString();
                    var fileExtension = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1).ToLower();
                    var fileName = $"{guidFileName}.{fileExtension}";

                    #region Upload locally
                    //var uploads = Path.Combine(_env.WebRootPath, _config["Uploads:ReportUploads"].ToString());
                    //filePath = Path.Combine(uploads, $"{guidFileName}.{fileExtension}");
                    //using (var stream = new FileStream(filePath, FileMode.Create))
                    //{
                    //    await file.CopyToAsync(stream);
                    //}
                    #endregion

                    #region Upload to Cloud
                    var azureUploadResult = await _azureContainersRepo.Upload(file, fileName);
                    if (azureUploadResult == null) throw new Exception();

                    var reportUpload = new ReportUploadedFile
                    {
                        Report = report,
                        ReportId = report.ReportId,
                        Filename = fileName,
                        FileTitle = file.FileName,
                        Extension = fileExtension,
                        Uri = azureUploadResult.Uri,
                        UploadedDate = DateTime.Now,
                        AzureBlobName = azureUploadResult.BlobName,
                        AzureContainer = azureUploadResult.ContainerName
                    };
                    report.Uploads.Add(reportUpload);
                    await _context.SaveChangesAsync();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return Ok(new { code = 2, status = Ok().StatusCode, message = "Ok" });
        }


        [HttpGet]
        [Authorize(Roles = "principle")]
        public async Task<IActionResult> SignByPrinciple(string no = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var report = _reportsRepo.GetByReportNo(no, includeUploads: true);

            if (report == null)
                return View("Error");

            if (user.GetCurrentSchool(_schoolsRepo).MinistryNo != report.SchoolMinistryNo)
                return View("Account/AccessDenied");

            if (report.IsSignedByPrinciple)
                return View("Error");

            if (report.Uploads == null || report.Uploads.Count < 1)
                return BadRequest("لا يمكن اعتماد التقرير بسبب عدم إرفاق شواهد التنفيذ. الرجاء تعديل التقرير.");

            var program = _eduProgramRepo.GetById(report.EduProgramId);
            if (_eduProgramRepo.IsDeadlinePassed(program, DateTime.Now))
                return BadRequest("لا يمكن اعتماد التقرير لتجاوز الموعد النهائي لإرسال التقرير.");

            report.IsSignedByPrinciple = true;
            report.SigningDateTime = DateTime.Now;
            report.SigningPrinciple = user;
            report.SigningPrincipleFullName = user.FullName;
            report.SigningPrincipleIdNo = user.UserName;
            report.AllowEdit = false;
            var result = await _reportsRepo.SaveChangesAsync();
            if (result)
            {
                var smsMsg = $"تم اعتماد التقرير رقم {report.ReportNo} من مدرسة {report.SchoolName}."; //$"تم اعتماد تقرير لبرنامج {report.EduProgramName} من مدرسة {report.SchoolName}. رابط التقرير https://moeduyanbu.com/report/show?reportNo={report.ReportNo}";
                var phones = new List<string>();
                phones.Add(user.PhoneNumber);
                var mentor = await _userManager.FindByNameAsync(report.OwnerIdNo);
                if (mentor != null)
                    phones.Add(mentor.PhoneNumber);

                //var groupSmsRoles = Templates.Roles[2].Name;
                //var eduEmpPhones = 
                //    await _usersRepo.GetPhoneNumbersForUsersInRolesAsync(groupSmsRoles);
                //if (eduEmpPhones != null)
                //    phones.AddRange(eduEmpPhones);

                await _smsSender.SendSmsAsync(phones, smsMsg);

                return RedirectToAction(nameof(Show), new { reportNo = no, message = ReportMessageId.SignByPrincipleSuccess });
            }
            //// SMS for mentor
            //var smsMsg = $"قائد المدرسة اعتمد التقرير رقم {report.ReportNo}. رابط التقرير https://moeduyanbu.com/report/show?reportNo={report.ReportNo}";
            //var mentor = await _userManager.FindByNameAsync(report.OwnerIdNo);
            //if (mentor != null)
            //{
            //    await _smsSender.SendSmsAsync(mentor.PhoneNumber, smsMsg);
            //}

            //// SMS for principle
            //smsMsg = $"تم اعتماد تقرير البرنامج الإرشادي بنجاح. رقم التقرير {report.ReportNo}";
            //await _smsSender.SendSmsAsync(user.PhoneNumber, smsMsg);

            //smsMsg = $"تم اعتماد تقرير لبرنامج {report.EduProgramName} من مدرسة {report.SchoolName}. رابط التقرير https://moeduyanbu.com/report/show?reportNo={report.ReportNo}";
            //var groupSmsRoles = Templates.Roles[2].Name;
            //var eduUsersPhones = await _usersRepo.GetPhoneNumbersForUsersInRolesAsync(groupSmsRoles);
            //await _smsSender.SendSmsAsync(eduUsersPhones, smsMsg);

            return View("Error");
        }

        [HttpGet]
        [Authorize(Roles = "sysowner,admin,eduemployee")]
        public async Task<IActionResult> Evaluate(string no = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var report = _reportsRepo.GetByReportNo(no);

            if (report == null)
                return View("Error");

            return View(new EvaluateViewModel { ReportNo = no, Evaluation = report.Evaluation });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "sysowner,admin,eduemployee")]
        public async Task<IActionResult> Evaluate(EvaluateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var report = _reportsRepo.GetByReportNo(model.ReportNo);

            if (report == null)
                return View("Error");

            //if (!report.IsEvaluated)
            //{
            //    report.IsEvaluated = true;
            //    report.Evaluator = user;
            //    report.EvaluationDate = DateTime.Now;
            //    report.EvaluatorFullName = user.FullName;
            //    report.EvaluatorIdNo = user.UserName;
            //    report.Evaluation = model.Evaluation;
            //}
            //else
            //{

            //}

            report.IsEvaluated = true;
            report.Evaluator = user;
            report.EvaluatorId = user.Id;
            report.EvaluationDate = DateTime.Now;
            report.EvaluatorFullName = user.FullName;
            report.EvaluatorIdNo = user.UserName;
            report.Evaluation = model.Evaluation;

            var evalRecord = new ReportKpiRecord
            {
                KpiRecordId = Guid.NewGuid().ToString(),
                RecordValue = (decimal)(report.Evaluation / 10.0),
                RecordCreator = report.Evaluator,
                RecordCreatorId = report.EvaluatorId,
                RecordDate = new DateTimeOffset(report.EvaluationDate.Value,
TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time").GetUtcOffset(report.EvaluationDate.Value)),
                Report = report,
                ReportId = report.ReportId,
            };
            await _kpiRepo.AdjustReportKpisAsync(report, evalRecord);

            var result = await _reportsRepo.SaveChangesAsync();
            await _context.SaveChangesAsync();
            if (result)
            {
                #region Send SMS notifications for report owner and principle.
                var smsMsg = $"تقييم قسم التوجيه والإرشاد بينبع للتقرير رقم {report.ReportNo} هو {report.Evaluation}/10"; //$"تقييم قسم التوجيه والإرشاد بينبع للتقرير رقم {report.ReportNo} هو {report.Evaluation} من 10. رابط التقرير https://moeduyanbu.com/report/show?reportNo={report.ReportNo}";
                var mentor = await _userManager.FindByNameAsync(report.OwnerIdNo);
                if (mentor != null)
                {
                    _smsSender.SendSmsAsync(mentor.PhoneNumber, smsMsg);
                }

                var principle = await _userManager.FindByNameAsync(report.SigningPrincipleIdNo);
                if (principle != null)
                {
                    _smsSender.SendSmsAsync(principle.PhoneNumber, smsMsg);
                }

                //var groupSmsRoles = Templates.eduemployeeRoleUpExceptSysowner;
                //var eduUsersPhones = await _usersRepo.GetPhoneNumbersForUsersInRoles(groupSmsRoles);
                //_smsSender.SendSmsAsync(eduUsersPhones, smsMsg);
                #endregion
                return RedirectToAction(nameof(Show), new { reportNo = model.ReportNo, message = ReportMessageId.EvaluateSuccess });
            }

            return View("Error");
        }

        [HttpGet]
        [Authorize(Roles = Templates.mentorRoleUp)]
        public async Task<IActionResult> AddResponse(string no = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var report = _reportsRepo.GetByReportNo(no);

            if (report == null)
                return View("Error");

            if ((await user.IsSchoolEmployee(_userManager))
                && user.GetCurrentSchool(_schoolsRepo).MinistryNo != report.SchoolMinistryNo)
            {
                return View("Error");
            }

            return View(new AddResponseViewModel { ReportNo = no, Content = "" });
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AddActivity(string no = null)
        {
            if (String.IsNullOrEmpty(no))
            {
                return View("Error");
            }

            var report = _reportsRepo.GetByReportNo(no);
            if (report == null)
            {
                return View("Error");
            }

            var user = await _userManager.GetUserAsync(this.User);
            if (user != null)
            {
                return BadRequest("تفاعلات الجمهور متاحة للزوار فقط.");
            }

            ViewData["ReCaptchaKey"] = _config.GetSection("GoogleReCaptcha:key").Value;

            return View(new AddActivityViewModel
            {
                ReportNo = report.ReportNo
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActivity(AddActivityViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                return View("Error"); // Visitors only can add activities.
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // A good practice is to attach a validation attribute to the model and not to validate in the controller
            #region CustomValidation
            string[] badWords =
                { "أنيك", "انيك", "منيوك", "المنيوك", "منيوكة", "المنيوكة",
                "منيوكه" ,"المنيوكه" , "كس" ,"زب" ,"كسمك" ,"زبك" ,"زبي" ,"قحبة","قحبه" ,
                "القحبة", "القحبه" ,"شرموط" ,"الشرموط" ,"شرموطة" ,"الشرموطة", "شرموطه" ,
                "الشرموطه","شراميط" ,"الشراميط" ,"مخنوث" ,"المخنوث", "خنيث", "الخنيث",
                "مخانيث","المخانيث" ,"طرطور" ,"الطرطور", "طراطير", "الطراطير", "مبسبس",
                "المبسبس","مبسبسين", "المبسبسين", "أزباب", "ازباب", "زبيبي", "زبيبتي",
                "زبزوب", "الزبزوب", "زبزوبي", "الزبزوبي","حمار","الحمار","حيوان","الحيوان","زق","الزق"
            };
            Type modelType = model.GetType();
            var modelProperties = modelType.GetProperties();
            bool isModelContainsBadWords = false;
            foreach (var property in modelProperties)
            {
                foreach (var badWord in badWords)
                {
                    var val = property.GetValue(model).ToString();
                    if (val.Contains(badWord + " ")
                        || val.Contains(" " + badWord)
                        || val.Contains(" " + badWord + " ")
                        || val.Contains(badWord + ".")
                        || val.Contains(badWord + ",")
                        || (val.Contains(badWord) && val.Length == badWord.Length))
                    {
                        //this.ModelState.AddModelError(property.Name, $"The word '{badWord}' is a bad word");
                        isModelContainsBadWords = true;
                    }
                }
            }
            if (isModelContainsBadWords)
            {
                this.ModelState.AddModelError("", "حدث خطأ أثناء إرسال البيانات، يرجى المحاولة لاحقاً."); // A Model error, not a property error and it will show up in the validation summary in the view if the summary is set 'ModelOnly', 'ModelOnly will show up model errors in the summary, not property errors.
                return View(model);
            }

            #endregion

            if (!GoogleReCaptchaHelper.ReCaptchaPassed(
                Request.Form["g-recaptcha-response"], // that's how you get it from the Request object
                _config.GetSection("GoogleReCaptcha:secret").Value,
                _logger)
                )
            {
                ViewData["ReCaptchaKey"] = _config.GetSection("GoogleReCaptcha:key").Value;
                ModelState.AddModelError(string.Empty, "فشل اختبار التحقق من صحة الإدخال البشري.");
                return View(model);
            }

            var report = _reportsRepo.GetByReportNo(model.ReportNo);
            if (report == null)
                return View("Error");

            report.VisitorRatingCount++;
            report.VisitorOverallRating = (int)Math.Ceiling(
                    (double)((double)report.VisitorOverallRating + (double)model.Rating)
                    / (double)report.VisitorRatingCount);

            var activity = new ReportActivity
            {
                Report = report,
                Content = model.Content,
                FullName = model.FullName,
                Rating = model.Rating,
                Email = model.Email,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                CreatedDate = DateTime.Now
            };

            _reportsRepo.AddActivity(activity, report);

            var result = await _reportsRepo.SaveChangesAsync();
            if (result)
                return RedirectToAction(nameof(Show), new { reportNo = model.ReportNo, message = ReportMessageId.AddActivitySuccess });
            return View("Error");
        }

        #region Helpers
        private bool IsMentorSchoolMatchesEduProgramLevel(ApplicationUser mentor, EduProgram eduProgram)
        {
            var userSchools = _schoolsRepo.GetAllForUser(mentor.UserName);
            _context.Entry(eduProgram).Collection(x => x.SchoolLevels).Load();
            return eduProgram.SchoolLevels.Any(x => userSchools.Any(us => us.LevelId == x.SchoolLevelId));
        }
        #endregion

        //[HttpGet]
        //[GenerateAntiforgeryTokenCookieForAjax]
        //public async Task<IActionResult> ManageUploads(string reportId)
        //{
        //    var report = _reportsRepo.GetById(reportId);
        //    if (report == null)
        //        return View("Error");

        //    var user = await _userManager.GetUserAsync(this.User);

        //    if (user != report.Owner)
        //        return View("Account/AccessDenied");

        //    var uploads = _reportsRepo.GetUploads(report).ToList();
        //    ViewBag.Uploads = uploads;
        //    ViewData["FilesCount"] = uploads.Count;
        //    return View();
        //}

        #region Uploading with streaming (Commented)
        //[HttpGet]
        //[GenerateAntiforgeryTokenCookieForAjax]
        //public IActionResult Index()
        //{
        //    return View();
        //}

        //// 1. Disable the form value model binding here to take control of handling 
        ////    potentially large files.
        //// 2. Typically antiforgery tokens are sent in request body, but since we 
        ////    do not want to read the request body early, the tokens are made to be 
        ////    sent via headers. The antiforgery token filter first looks for tokens
        ////    in the request header and then falls back to reading the body.
        //[HttpPost]
        //[DisableFormValueModelBinding]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Upload()
        //{
        //    #region Commented
        //    if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
        //    {
        //        return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
        //    }

        //    // Used to accumulate all the form url encoded key value pairs in the 
        //    // request.
        //    var formAccumulator = new KeyValueAccumulator();
        //    string targetFilePath = null;

        //    string targetFileName = Guid.NewGuid().ToString();
        //    string folderPath = null;
        //    string fileName = null;
        //    string fileExtension = null;

        //    var boundary = MultipartRequestHelper.GetBoundary(
        //        MediaTypeHeaderValue.Parse(Request.ContentType),
        //        300); //_defaultFormOptions.MultipartBoundaryLengthLimit
        //    var reader = new MultipartReader(boundary, HttpContext.Request.Body);

        //    var section = await reader.ReadNextSectionAsync();
        //    while (section != null)
        //    {
        //        ContentDispositionHeaderValue contentDisposition;
        //        var hasContentDispositionHeader = ContentDispositionHeaderValue
        //            .TryParse(section.ContentDisposition, out contentDisposition);



        //        if (hasContentDispositionHeader)
        //        {
        //            if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
        //            {
        //                //Debug.WriteLine($"************************** contentDisposition HasFile: {contentDisposition.FileName} | {contentDisposition.DispositionType}");
        //                #region My touches! Commented!
        //                //    fileName = Guid.NewGuid().ToString();
        //                //    targetFolderPath = Path.Combine(_env.WebRootPath,
        //                //_config["Uploads:ReportUploads"].ToString());
        //                //    targetFilePath = Path.Combine(targetFolderPath, fileName + "." + );
        //                #endregion
        //                //// Azure blob for temp files: https://azure.microsoft.com/en-in/resources/samples/storage-blob-dotnet-store-temp-files/
        //                //folderPath = Path.GetTempPath();
        //                fileName = contentDisposition.FileName;
        //                fileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1);
        //                //try { fileName = fileName.Remove(0, fileName.LastIndexOf("\\") + 1); } catch { }

        //                //targetFilePath = Path.Combine(folderPath, fileName + "." + fileExtension);
        //                targetFilePath = "C:\\temp\\" + targetFileName + ".tmp";//Path.GetTempFileName();
        //                Debug.WriteLine($"**************** {targetFilePath}");
        //                Debug.WriteLine("------------------- " + targetFilePath + "\n\n\n\n\n\n");
        //                using (var targetStream = System.IO.File.Create(targetFilePath))
        //                {
        //                    await section.Body.CopyToAsync(targetStream);

        //                    _logger.LogInformation($"Copied the uploaded file '{targetFilePath}'");
        //                }
        //            }
        //            else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
        //            {
        //                //Debug.WriteLine($"************************** contentDisposition NOT HasFile: {contentDisposition.FileName} | {contentDisposition.DispositionType}");
        //                // Content-Disposition: form-data; name="key"
        //                //
        //                // value

        //                // Do not limit the key name length here because the 
        //                // multipart headers length limit is already in effect.
        //                var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
        //                var encoding = GetEncoding(section);
        //                using (var streamReader = new StreamReader(
        //                    section.Body,
        //                    encoding,
        //                    detectEncodingFromByteOrderMarks: true,
        //                    bufferSize: 1024,
        //                    leaveOpen: true))
        //                {
        //                    // The value length limit is enforced by MultipartBodyLengthLimit
        //                    var value = await streamReader.ReadToEndAsync();
        //                    if (String.Equals(value, "undefined",
        //                        StringComparison.OrdinalIgnoreCase))
        //                    {
        //                        value = String.Empty;
        //                    }
        //                    formAccumulator.Append(key, value);
        //                    //Debug.WriteLine($" ------------- formAccumulator: {key} | {value}");

        //                    if (formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
        //                    {
        //                        throw new InvalidDataException($"Form key count limit {_defaultFormOptions.ValueCountLimit} exceeded.");
        //                    }
        //                }
        //            }
        //        }

        //        // Drains any remaining section body that has not been consumed and
        //        // reads the headers for the next section.
        //        section = await reader.ReadNextSectionAsync();
        //    }

        //    //System.IO.File.Move(targetFilePath, "c:\\temp\\" + targetFileName + "." + fileExtension);

        //    // Bind form data to a model
        //    var vm = new ReportUploadViewModel();
        //    var formValueProvider = new FormValueProvider(
        //        BindingSource.Form,
        //        new FormCollection(formAccumulator.GetResults()),
        //        CultureInfo.CurrentCulture);

        //    var bindingSuccessful = await TryUpdateModelAsync(vm, prefix: "",
        //        valueProvider: formValueProvider);
        //    if (!bindingSuccessful)
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //    }


        //    var uploadedData = new UploadedData()
        //    {
        //        ReportNo = vm.ReportNo,
        //        FilePath = targetFilePath
        //    };
        //    return Json(uploadedData);
        //    #endregion

        //    #region with FileStreamingHelper class
        //    //FormValueProvider formModel;
        //    //using (var stream = System.IO.File.Create("C:\\temp\\myfile.temp"))
        //    //{
        //    //    formModel = await Request.StreamFile(stream);
        //    //}

        //    //var viewModel = new ReportUploadViewModel();

        //    //var bindingSuccessful = await TryUpdateModelAsync(viewModel, prefix: "",
        //    //   valueProvider: formModel);

        //    //if (!bindingSuccessful)
        //    //{
        //    //    if (!ModelState.IsValid)
        //    //    {
        //    //        return BadRequest(ModelState);
        //    //    }
        //    //}
        //    #endregion
        //}

        //private static Encoding GetEncoding(MultipartSection section)
        //{
        //    MediaTypeHeaderValue mediaType;
        //    var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
        //    // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
        //    // most cases.
        //    if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
        //    {
        //        return Encoding.UTF8;
        //    }
        //    return mediaType.Encoding;
        //}
        #endregion
        #region Commented
        //    // Get the default form options so that we can use them to set the default limits for
        //    // request body data
        //    private static readonly FormOptions _defaultFormOptions = new FormOptions();

        //    #region snippet1
        //    // 1. Disable the form value model binding here to take control of handling 
        //    //    potentially large files.
        //    // 2. Typically antiforgery tokens are sent in request body, but since we 
        //    //    do not want to read the request body early, the tokens are made to be 
        //    //    sent via headers. The antiforgery token filter first looks for tokens
        //    //    in the request header and then falls back to reading the body.
        //    [HttpPost]
        //    [DisableFormValueModelBinding]
        //    [ValidateAntiForgeryToken]
        //    [Route("/report/{reportNo}/upload")]
        //    public async Task<IActionResult> Upload(string reportNo)
        //    {
        //        if(String.IsNullOrEmpty(reportNo))
        //        {
        //            return View("Error");
        //        }
        //        if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
        //        {
        //            return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
        //        }

        //        // Create a folder for the report with its number
        //        var uploadsPath = Path.Combine(_env.WebRootPath,
        //_config["Uploads:ReportUploads"].ToString());
        //        var reportFolderPath = Path.Combine(uploadsPath, reportNo);
        //        if(!Directory.Exists(reportFolderPath))
        //        {
        //            Directory.CreateDirectory(reportFolderPath);
        //        }

        //        // Used to accumulate all the form url encoded key value pairs in the 
        //        // request.
        //        var formAccumulator = new KeyValueAccumulator();
        //        string targetFilePath = null;

        //        var boundary = MultipartRequestHelper.GetBoundary(
        //            MediaTypeHeaderValue.Parse(Request.ContentType),
        //            _defaultFormOptions.MultipartBoundaryLengthLimit);
        //        var reader = new MultipartReader(boundary, HttpContext.Request.Body);

        //        var section = await reader.ReadNextSectionAsync();
        //        while (section != null)
        //        {
        //            ContentDispositionHeaderValue contentDisposition;
        //            var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

        //            if (hasContentDispositionHeader)
        //            {
        //                if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
        //                {
        //                    targetFilePath = Path.GetTempFileName();
        //                    using (var targetStream = System.IO.File.Create(targetFilePath))
        //                    {
        //                        await section.Body.CopyToAsync(targetStream);

        //                        _logger.LogInformation($"Copied the uploaded file '{targetFilePath}'");
        //                    }
        //                }
        //                else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
        //                {
        //                    // Content-Disposition: form-data; name="key"
        //                    //
        //                    // value

        //                    // Do not limit the key name length here because the 
        //                    // multipart headers length limit is already in effect.
        //                    var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
        //                    var encoding = GetEncoding(section);
        //                    using (var streamReader = new StreamReader(
        //                        section.Body,
        //                        encoding,
        //                        detectEncodingFromByteOrderMarks: true,
        //                        bufferSize: 1024,
        //                        leaveOpen: true))
        //                    {
        //                        // The value length limit is enforced by MultipartBodyLengthLimit
        //                        var value = await streamReader.ReadToEndAsync();
        //                        if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            value = String.Empty;
        //                        }
        //                        formAccumulator.Append(key, value);

        //                        if (formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
        //                        {
        //                            throw new InvalidDataException($"Form key count limit {_defaultFormOptions.ValueCountLimit} exceeded.");
        //                        }
        //                    }
        //                }
        //            }

        //            // Drains any remaining section body that has not been consumed and
        //            // reads the headers for the next section.
        //            section = await reader.ReadNextSectionAsync();
        //        }

        //        // Bind form data to a model
        //        var user = new User();
        //        var formValueProvider = new FormValueProvider(
        //            BindingSource.Form,
        //            new FormCollection(formAccumulator.GetResults()),
        //            CultureInfo.CurrentCulture);

        //        var bindingSuccessful = await TryUpdateModelAsync(user, prefix: "",
        //            valueProvider: formValueProvider);
        //        if (!bindingSuccessful)
        //        {
        //            if (!ModelState.IsValid)
        //            {
        //                return BadRequest(ModelState);
        //            }
        //        }

        //        var uploadedData = new UploadedData()
        //        {
        //            Name = user.Name,
        //            Age = user.Age,
        //            Zipcode = user.Zipcode,
        //            FilePath = targetFilePath
        //        };
        //        return Json(uploadedData);
        //    }
        //    #endregion

        //    private static Encoding GetEncoding(MultipartSection section)
        //    {
        //        MediaTypeHeaderValue mediaType;
        //        var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
        //        // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
        //        // most cases.
        //        if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
        //        {
        //            return Encoding.UTF8;
        //        }
        //        return mediaType.Encoding;
        //    }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public void FileUpload(ICollection<IFormFile> files, string reportNo = null)
        //{
        //    if (files != null)
        //    {
        //        foreach (var file in files)
        //        {
        //            // Verify that the user selected a file
        //            if (file != null && file.Length > 0)
        //            {
        //                // extract only the fielname
        //                var fileName = Path.GetFileName(file.FileName);
        //                // TODO: need to define destination
        //                var path = Path.Combine(Microsoft.AspNetCore.Hosting.Server.MapPath("~/Upload"), fileName);
        //                file.SaveAs(path);
        //            }
        //        }
        //    }

        //    // Upload file
        //    if (model.DescriptionFile != null)
        //    {
        //        var file = model.DescriptionFile;
        //        program.DescriptionFileExtension =
        //            file.FileName.Substring(file.FileName.LastIndexOf('.') + 1);


        //        var uploads = Path.Combine(_env.WebRootPath,
        //            _config["Uploads:EduProgramUploads"].ToString());

        //        var filePath = Path.Combine(uploads,
        //            $"{program.EduProgramId}." +
        //            $"{file.FileName.Substring(file.FileName.LastIndexOf('.') + 1)}");
        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(fileStream);
        //        }
        //    }
        //}
        #endregion
    }
}