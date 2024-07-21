using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModepEduYanbu.Models;
using ModepEduYanbu.Models.AccountViewModels;
using ModepEduYanbu.Services;
using Microsoft.Extensions.Configuration;
using ModepEduYanbu.Data;
using ModepEduYanbu.Repositories;
using AutoMapper;
using Newtonsoft.Json;
using ModepEduYanbu.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using ModepEduYanbu.Helpers;
using ModepEduYanbu.Models.AdminViewModels;
using Microsoft.EntityFrameworkCore;
using ModepEduYanbu.DAL.DbContexts;

namespace ModepEduYanbu.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizedPeopleRepo _authorizedPeopleRepo;
        private readonly ISchoolRepo _schoolRepo;
        private readonly ExcelReportsManager _excelReportsManager;

        public AdminController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            IConfiguration config,
            ApplicationDbContext context,
            IAuthorizedPeopleRepo authorizedPeopleRepo,
            ISchoolRepo schoolRepo,
            ExcelReportsManager excelReportsManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _config = config;
            _context = context;
            _authorizedPeopleRepo = authorizedPeopleRepo;
            _schoolRepo = schoolRepo;
            _excelReportsManager = excelReportsManager;
        }

        [HttpGet]
        [Authorize(Roles = Templates.adminRoleUp)]
        public IActionResult AddSchool()
        {
            // School List
            List<SelectListItem> levels = _context.SchoolLevels
                .Select(x => new SelectListItem { Text = x.Name, Value = x.SchoollevelId}).ToList();

            // Program List
            var types = _context.SchoolTypes
                .Select(x => new SelectListItem { Text = x.ClassificationName, Value = x.SchoolTypeId }).ToList();

            return View(new AddSchoolViewModel { Levels = levels, Types = types});
        }

        [HttpPost]
        [Authorize(Roles = Templates.adminRoleUp)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSchool(AddSchoolViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var school = Mapper.Map<School>(model);
            school.LevelId = model.SelectedLevel;
            _context.Schools.Add(school);
            await _context.SaveChangesAsync();

            ViewBag.UserMessage = "تمت إضافة بيانات المدرسة إلى النظام بنجاح.";
            ModelState.Clear();
            return View();
        }        

        [HttpGet]
        [Authorize(Roles = "sysowner")]
        public IActionResult UploadSchoolsPrinciplesData()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "sysowner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadSchoolsPrinciplesData(List<IFormFile> files)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _excelReportsManager.UploadSchoolsAndPrinciples(files, user);

                ModelState.Clear();
                ViewBag.UserMessage = $"تم رفع البيانات بنجاح\nعدد المدارس المضافة للنظام:{result.SchoolsCount}\nعدد قادة المدارس المضافين للنظام:{result.PrinciplesCount}";
                return View();
            }
            catch(Exception ex)
            {
                ViewBag.UserMessage = "حدث خطأ أثناء رفع البيانات، يرجى المحاولة مرة أخرى.";
                return View();
            }
        }

        [HttpGet]
        [Authorize(Roles = Templates.adminRoleUp)]
        public async Task<IActionResult> ChangeUserSchools(string username = null)
        {
            if (!String.IsNullOrEmpty(username))
            {
                if (!_userManager.Users.Any(u => u.UserName == username))
                    return View("Error");
            }

            var principles = await _userManager.GetUsersInRoleAsync(Templates.Roles[3].Name);
            var mentors = await _userManager.GetUsersInRoleAsync(Templates.Roles[4].Name);
            //await Task.WhenAll(principles, mentors);
            var schoolUsers = principles.Concat(mentors);
            IEnumerable<SelectListItem> schoolUsersList = schoolUsers.Select(u => new SelectListItem {
                Text = $"{u.FullName} - {u.UserName}",
                Value = u.UserName
            });
            ViewBag.SchoolUsersList = schoolUsersList;
            ViewBag.Id = username;

            var model = _userManager.Users.Where(u => u.UserName == username)
                .Select(u => new ChangeUserSchoolsViewModel {
                    UserFullName = u.FullName,
                    UserIdNo = u.UserName
                }).FirstOrDefault();
            if(model != null)
            {
                var user = _userManager.Users.FirstOrDefault(x => x.UserName == username);
                model.RoleName = (await _roleManager.FindByNameAsync((await _userManager.GetRolesAsync(user))[0])).Description;
                model.UserSchools = _schoolRepo.GetAllForUser(user.UserName)
                    .Select(s => new SchoolInfoViewModel { SchoolId = s.SchoolId, SchoolName = s.Name });
                ViewBag.SchoolsList = _context.Schools.Select(s => new SelectListItem { Text = s.Name, Value = s.SchoolId});
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = Templates.adminRoleUp)]
        public async Task<IActionResult> DeleteSchoolForUser(string username = null, string schoolId = null)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(schoolId)) return View("Error");
            var user = _userManager.Users.Include(x => x.UserSchools).FirstOrDefault(x => x.UserName == username);
            if (user == null) return View("Error");
            var school = _schoolRepo.GetAllForUser(user.UserName).FirstOrDefault(x => x.SchoolId == schoolId);
            if (school == null) return View("Error");
            var role = (await _userManager.GetRolesAsync(user))[0];
            if(role == "principle")
            {
                school.PrincipleIdNo = null;
            }
            if(user.CurrentSchoolId == school.SchoolId)
            {
                user.CurrentSchool = null;
                user.CurrentSchoolId = null;
            }
            var userSchoolRelation = user.UserSchools.FirstOrDefault(x => x.SchooId == schoolId);
            user.UserSchools.Remove(userSchoolRelation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(this.ChangeUserSchools), new { username = user.UserName});
        }

        [HttpGet]
        [Authorize(Roles = Templates.adminRoleUp)]
        public async Task<IActionResult> AddSchoolForUser(string username = null, string schoolId = null)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(schoolId)) return View("Error");
            var user = _userManager.Users.Include(x => x.UserSchools).FirstOrDefault(x => x.UserName == username);
            if (user == null) return View("Error");
            var school = _schoolRepo.GetById(schoolId);
            if ( school == null) return View("Error");
            if (_schoolRepo.GetAllForUser(user.UserName).FirstOrDefault(x => x.SchoolId == schoolId) != null)
                return View("Error"); // User already in the school
            var role = (await _userManager.GetRolesAsync(user))[0];
            if(role == "mentor" && user.UserSchools.Count > 0)
            {
                return BadRequest("لا يمكن للمرشد الطلابي الانضمام إلى أكثر من مدرسة في نفس الوقت. الرجاء حذف المرشد من مدرسته الحالية ثم إعادة محاولة إضافته إلى مدرسة أخرى.");
            }
            if (role == "principle")
            {
                if(school.PrincipleIdNo != null)
                {
                    return BadRequest($"لا يمكن تعيين المستخدم المختار كقائد للمدرسة بسبب تعيين رقم هوية قائد المدرسة مسبقا في بيانات المدرسة (رقم الهوية للقائد الحالي {school.PrincipleIdNo} ) الرجاء إزالة القائد الحالي للمدرسة من مدرسته ثم تعيين القائد الجديد.");
                }
                school.PrincipleIdNo = user.UserName;
            }
            user.UserSchools.Add(new UserSchool { School = school, User = user, SchooId = school.SchoolId, UserId = user.Id });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(this.ChangeUserSchools), new { username = user.UserName });
        }
    }
}