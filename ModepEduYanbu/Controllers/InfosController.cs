//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Options;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Configuration;
//using ModepEduYanbu.Data;
//using ModepEduYanbu.Repositories;
//using ModepEduYanbu.Models;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using ModepEduYanbu.Models.DashboardViewModels;
//using ModepEduYanbu.Helpers.Extensions;
//using ModepEduYanbu.Repositories.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.EntityFrameworkCore;
//using Sakura.AspNetCore;

//namespace ModepEduYanbu.Controllers
//{
//    public class InfosController : Controller
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly RoleManager<ApplicationRole> _roleManager;
//        private readonly ILogger<AccountController> _logger;
//        private readonly IConfiguration _config;
//        private readonly ApplicationDbContext _context;
//        public readonly ISchoolRepo _schoolRepo;
//        private readonly IEduProgramsRepo _eduProgramsRepo;
//        private readonly IReportsRepo _reportsRepo;

//        public InfosController(UserManager<ApplicationUser> userManager,
//            SignInManager<ApplicationUser> signInManager,
//            RoleManager<ApplicationRole> roleManager,
//            ILoggerFactory loggerFactory,
//            IConfiguration config,
//            ApplicationDbContext context,
//            ISchoolRepo schoolRepo,
//            IEduProgramsRepo eduProgramsRepo,
//            IReportsRepo reportsRepo)
//        {

//        }

//        [HttpGet("User")]
//        public async Task<IActionResult> UserInfo(string id)
//        {
//            var userInfo = _userManager.Users.FirstOrDefault(x => x.Id == id);
//            if (userInfo == null) return View("Error");
//            var userInfoRole = (await _userManager.GetRolesAsync(userInfo))[0];
//            //var userInfoReports = 
//            return View();
//        }

//        public IActionResult School(string id)
//        {
//            return View();
//        }
//    }
//}