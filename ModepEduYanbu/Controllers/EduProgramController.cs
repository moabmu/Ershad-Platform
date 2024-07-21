using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ModepEduYanbu.Models.EduProgramViewModels;
using ModepEduYanbu.Helpers;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using ModepEduYanbu.Models;
using ModepEduYanbu.Repositories;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;
using ModepEduYanbu.Repositories.Interfaces;
using ModepEduYanbu.Data;
using Microsoft.EntityFrameworkCore;
using ModepEduYanbu.DAL.DbContexts;
using Microsoft.AspNetCore.Authorization;
using QuartzWrapper;
using ModepEduYanbu.Jobs;
using Quartz;

namespace ModepEduYanbu.Controllers
{
    public class EduProgramController : Controller
    {
        private readonly IEduProgramsRepo _eduProgramRepo;
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly IAzureContainersRepo _azureContainersRepo;
        private readonly IEducationalYearsRepo _educationalYearsRepo;
        private readonly QuartzHostedService _quartzService;
        private readonly string _triggerKeyPrefix = "ReportKpis_For_NonSentReports_";
        private const string REPORT_DEADLINE_FOR_NEW_EDUPROGRAM_OLD_ERR_MSG = "التاريخ المدخل أقدم من تاريخ اليوم.";

        public EduProgramController(IEduProgramsRepo eduProgramRepo,
            IHostingEnvironment env,
            IConfiguration config,
            ApplicationDbContext context,
            IAzureContainersRepo azureContainersRepo,
            IEducationalYearsRepo educationalYearsRepo,
            QuartzHostedService quartzService)
        {
            _eduProgramRepo = eduProgramRepo;
            _env = env;
            _config = config;
            _context = context;
            _azureContainersRepo = azureContainersRepo;
            _educationalYearsRepo = educationalYearsRepo;
            _quartzService = quartzService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "sysowner,admin,eduemployee")]
        public IActionResult Add()
        {
            var vm = new EduProgramViewModel { SchoolLevels = _context.SchoolLevels
                .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
                { 
                    Text = x.Name,
                    Selected = true,
                    Value = x.SchoollevelId
                })
                .AsNoTracking()
                .ToList() 
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "sysowner,admin,eduemployee")]
        public async Task<IActionResult> Add(EduProgramViewModel model)
        {
            if (model.DescriptionFile != null)
            {
                await UploadedFileHelpers.ValidateFormFile(
                    model.DescriptionFile,
                    ModelState, UploadedFileHelpers.FileType.Document | 
                    UploadedFileHelpers.FileType.Image,
                    (int)UploadedFileHelpers.FileSize.OneMB,
                    model.GetType()
                    );
            }

            if (!ModelState.IsValid)
            {
                goto ResultWhenInvalidModelState;
            }

            var currentYear = _educationalYearsRepo.GetCurrentYearOrCreateIfNotExist();
            var eduProgramNameRepeated = _context.EduPrograms
                .Any(x => x.Name == model.Name && x.EducationalYearId == currentYear.EducationalYearId);
            if (eduProgramNameRepeated)
            {
                ModelState.AddModelError("Name", "يوجد برنامج إرشادي آخر له نفس الاسم. الرجاء اختيار مسمى آخر.");
                goto ResultWhenInvalidModelState;
            }

            var program = Mapper.Map<EduProgram>(model);
            FixEduProgramDates(ref program);

            if (!IsReportDeadlineForNewEduProgramValid(program))
            {
                ModelState.AddModelError(nameof(EduProgramViewModel.ReportDeadline), REPORT_DEADLINE_FOR_NEW_EDUPROGRAM_OLD_ERR_MSG);
                goto ResultWhenInvalidModelState;
            }

            program.EduProgramId = Guid.NewGuid().ToString();
            _eduProgramRepo.Add(program);
            foreach(var sl in model.SelectedSchoolLevels)
            {
                if (!_context.SchoolLevels.Any(x => x.SchoollevelId == sl))
                    return BadRequest("تم إدخال قيمة غير مصرح بها كمعرف لمرحلة دراسية.");
                _context.Add(new EduProgramSchoolLevel { EduProgramId = program.EduProgramId, SchoolLevelId = sl });
            }

            // Upload file
            if (model.DescriptionFile != null && model.DescriptionFile.Length > 0)
            {
                #region Upload Locally
                //var file = model.DescriptionFile;
                //program.DescriptionFileExtension =
                //    file.FileName.Substring(file.FileName.LastIndexOf('.') + 1);


                //var uploads = Path.Combine(_env.WebRootPath, 
                //    _config["Uploads:EduProgramUploads"].ToString());

                //var filePath = Path.Combine(uploads,
                //    $"{program.EduProgramId}." +
                //    $"{file.FileName.Substring(file.FileName.LastIndexOf('.') + 1)}");
                //using (var fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    await file.CopyToAsync(fileStream);
                //}
                #endregion

                #region Upload to Azure Cloud
                try
                {
                    var file = model.DescriptionFile;
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

                    var eduProgramUpload = new EduProgramUploadedFile
                    {
                        EduProgram = program,
                        EduProgramId = program.EduProgramId,
                        Filename = fileName,
                        FileTitle = file.FileName,
                        Extension = fileExtension,
                        Uri = azureUploadResult.Uri,
                        UploadedDate = DateTime.Now,
                        AzureBlobName = azureUploadResult.BlobName,
                        AzureContainer = azureUploadResult.ContainerName
                    };
                    program.DescriptionFile = eduProgramUpload;
                    await _context.SaveChangesAsync();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                #endregion
            }

            await _eduProgramRepo.SaveChangesAsync();

            // Schedule a job
            await ScheduleNewJobForComputingReportKpis(program);

            ModelState.Clear();
            ViewBag.UserMessage = _config["UserMessages:DataSaved"].ToString();
            return View();

            ResultWhenInvalidModelState:
            if (model.SelectedSchoolLevels == null)
            {
                model.SchoolLevels = _context.SchoolLevels
                .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = x.Name,
                    Selected = false,
                    Value = x.SchoollevelId
                })
                .ToList();
            }
            else
            {
                model.SchoolLevels = _context.SchoolLevels
                .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = x.Name,
                    Selected = model.SelectedSchoolLevels.Any(s => x.SchoollevelId == s),
                    Value = x.SchoollevelId
                })
                .ToList();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "sysowner,admin")]
        public async Task<IActionResult> Edit(EduProgramViewModel model, string no)
        {
            if (String.IsNullOrEmpty(no))
            {
                return View("Error");
            }
            var program = _eduProgramRepo.GetById(no);
            if (program == null || _eduProgramRepo.IsDeadlinePassed(program, DateTime.Now))
            {
                return View("Error");
            }
            ViewData["EduProgramNo"] = program.EduProgramId;
            ViewData["EduProgramName"] = program.Name;
            ViewData["DescFile"] = program.DescriptionFile;

            if (model.DescriptionFile != null)
            {
                await UploadedFileHelpers.ValidateFormFile(
                    model.DescriptionFile,
                    ModelState, UploadedFileHelpers.FileType.Document |
                    UploadedFileHelpers.FileType.Image,
                    (int)UploadedFileHelpers.FileSize.OneMB,
                    model.GetType()
                    );
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TO-DO-FUTURE: Editing levels of eduProgram is temporary prohibited. Will be allowed ASAP.
            if (model.SelectedSchoolLevels != null || model.SelectedSchoolLevels.Count() > 0)
                return BadRequest("تمرير قيم المراحل الدراسية للبرنامج الإرشادي غير مصرح به أثناء تعديل بيانات البرنامج الإرشادي.");

            var currentYear = _educationalYearsRepo.GetCurrentYearOrCreateIfNotExist();
            var eduProgramNameRepeated = _context.EduPrograms
                .Any(x => x.Name == model.Name && 
                x.EducationalYearId == currentYear.EducationalYearId &&
                x.EduProgramId != program.EduProgramId);
            if (eduProgramNameRepeated)
            {
                ModelState.AddModelError("Name", "يوجد برنامج إرشادي آخر له نفس الاسم. الرجاء اختيار مسمى آخر.");
                return View(model);
            }
          
            program.Name = model.Name;
            var oldReportDeadline = program.ReportDeadline;

            try{program.BeginDate = DateTime.Parse(model.BeginDate);}
            catch{ModelState.AddModelError("BeginDate", "التاريخ المدخل غير صحيح"); return View(model);}

            try { program.EndDate = DateTime.Parse(model.EndDate); }
            catch { ModelState.AddModelError("EndDate", "التاريخ المدخل غير صحيح"); return View(model); }

            try { program.ReportDeadline = DateTime.Parse(model.ReportDeadline); }
            catch { ModelState.AddModelError("ReportDeadline", "التاريخ المدخل غير صحيح"); return View(model); }

            FixEduProgramDates(ref program);

            if (!IsReportDeadlineForNewEduProgramValid(program))
            {
                ModelState.AddModelError(nameof(EduProgramViewModel.ReportDeadline), REPORT_DEADLINE_FOR_NEW_EDUPROGRAM_OLD_ERR_MSG);
                return View(model);
            }

            //// if there are reports already created for this eduProgram, then the new deadline must be equal or newer than the old deadline.
            //if(_context.Reports.Where(x => x.EduProgramId == program.EduProgramId).Count() > 0 &&
            //    DateTime.Compare(oldReportDeadline, program.ReportDeadline) >= 0 )
            //{
            //    ModelState.AddModelError("ReportDeadline", "لا يمكن إدخال تاريخ أقدم من التاريخ الحالي");
            //    return View(model);
            //}

            // Changing the reportdeadline for finished eduProgram is not because report KPIs are already computed for non-sent (via job)
            var dateTimeNow = DateTime.Now;
            var reportDeadlinePassed = _eduProgramRepo.IsDeadlinePassed(program, dateTimeNow);
            var jobStartsRunning = program.QuartzJob_ComputeReportKpisJobStartsRunning;
            var canReschedule = !reportDeadlinePassed && !jobStartsRunning;
            if(reportDeadlinePassed)  
            {
                ModelState.AddModelError("ReportDeadline", "لا يمكن تعديل التاريخ بسبب انتهاء موعد ارسال التقرير وحساب مؤشرات الأداء لمن لم يرسل.");
                return View(model);
            }

            // Upload file
            if (model.DescriptionFile != null && model.DescriptionFile.Length > 0)
            {

                #region Upload Locally
                //var file = model.DescriptionFile;
                //program.DescriptionFileExtension =
                //    file.FileName.Substring(file.FileName.LastIndexOf('.') + 1);


                //var uploads = Path.Combine(_env.WebRootPath, 
                //    _config["Uploads:EduProgramUploads"].ToString());

                //var filePath = Path.Combine(uploads,
                //    $"{program.EduProgramId}." +
                //    $"{file.FileName.Substring(file.FileName.LastIndexOf('.') + 1)}");
                //using (var fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    await file.CopyToAsync(fileStream);
                //}
                #endregion

                #region Upload to Azure Cloud
                try
                {
                    var file = model.DescriptionFile;
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

                    var eduProgramUpload = new EduProgramUploadedFile
                    {
                        EduProgram = program,
                        EduProgramId = program.EduProgramId,
                        Filename = fileName,
                        FileTitle = file.FileName,
                        Extension = fileExtension,
                        Uri = azureUploadResult.Uri,
                        UploadedDate = DateTime.Now,
                        AzureBlobName = azureUploadResult.BlobName,
                        AzureContainer = azureUploadResult.ContainerName
                    };
                    await _azureContainersRepo.Delete(program.DescriptionFile.AzureContainer, program.DescriptionFile.Filename);
                    _context.EduProgramUploads.Remove(program.DescriptionFile);
                    program.DescriptionFile = eduProgramUpload;
                    await _context.SaveChangesAsync();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                #endregion
            }

            await _eduProgramRepo.SaveChangesAsync();

            // Reschedule job if the report deadline is changed AND job not starting running AND reportdeadline is not passed.
            if(DateTime.Compare(program.ReportDeadline, dateTimeNow) != 0 && canReschedule)
            {
                // Unschedule the old job
                await _quartzService.UnscheduleJob(new TriggerKey(program.QuartzJob_CurrentTriggerKeyName, program.QuartzJob_CurrentTriggerKeyGroup));

                // Schedule a new job
                await ScheduleNewJobForComputingReportKpis(program);
            }

            ModelState.Clear();
            ViewBag.UserMessage = _config["UserMessages:DataSaved"].ToString();
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "sysowner,admin")]
        public async Task<IActionResult> Edit(string no = null)
        {
            if (String.IsNullOrEmpty(no))
            {
                return View("Error");
            }

            var eduProgram = _eduProgramRepo.GetById(no);
            if (eduProgram == null || _eduProgramRepo.IsDeadlinePassed(eduProgram, DateTime.Now))
            {
                return View("Error");
            }
            var model = Mapper.Map<EduProgramViewModel>(eduProgram);
            AdjustDatesForEduProgramViewModel(ref model, eduProgram);
            ViewData["EduProgramNo"] = no;
            ViewData["EduProgramName"] = eduProgram.Name;
            ViewData["DescFile"] = eduProgram.DescriptionFile;
            return View(model);
        }

        [Authorize(Roles = "sysowner")]
        public async Task<IActionResult> Testing(IFormFile file)
        {
            await UploadedFileHelpers.ValidateFormFile(file, 
                ModelState, UploadedFileHelpers.FileType.Document, 
                (int)UploadedFileHelpers.FileSize.OneMB, null);
            return Ok("OKK");
        }

        #region Helpers

        private void FixEduProgramDates(ref EduProgram program)
        {
            program.BeginDate = program.BeginDate.AddDays(1).AddSeconds(-1);
            program.EndDate = program.EndDate.AddDays(1).AddSeconds(-1);
            program.ReportDeadline = program.ReportDeadline.AddDays(1).AddSeconds(-1);
        }

        private void AdjustDatesForEduProgramViewModel(ref EduProgramViewModel model, EduProgram source)
        {
            model.BeginDate = source.BeginDate.AddDays(-1).AddSeconds(1).ToShortDateString();
            model.EndDate = source.EndDate.AddDays(-1).AddSeconds(1).ToShortDateString();
            model.ReportDeadline = source.ReportDeadline.AddDays(-1).AddSeconds(1).ToShortDateString();
        }

        private bool IsReportDeadlineForNewEduProgramValid(EduProgram newEduProgram)
        {
            var result = DateTime.Compare(DateTime.Now, newEduProgram.ReportDeadline) < 0;
            return result;
        }

        private async Task ScheduleNewJobForComputingReportKpis(EduProgram program)
        {
            var reportDeadlineOffset = new DateTimeOffset(program.ReportDeadline,
                  TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time").GetUtcOffset(program.ReportDeadline));
            var triggerUniqueId = $"{_triggerKeyPrefix}{program.EduProgramId}";
            var jobDetails = JobBuilder
                .CreateForAsync<ComputeReportKpisForFinishedEduProgram>()
                .WithIdentity(triggerUniqueId)
                .WithDescription(triggerUniqueId)
                .Build();
            var trigger = TriggerBuilder
                .Create()
                .StartAt(reportDeadlineOffset)
                .WithIdentity(triggerUniqueId)
                .WithDescription(triggerUniqueId)
                .UsingJobData("ReportDeadline", reportDeadlineOffset.ToString()) // Job arg
                .UsingJobData("EduProgramId", program.EduProgramId) // Job arg
                .Build();
            await _quartzService.ScheduleJob<ComputeReportKpisForFinishedEduProgram>(jobDetails, trigger);

            program.QuartzJob_CurrentTriggerKeyGroup = trigger.Key.Group;
            program.QuartzJob_CurrentTriggerKeyName = trigger.Key.Name;
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}