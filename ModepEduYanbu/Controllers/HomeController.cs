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
using ModepEduYanbu.DAL.DbContexts;

namespace ModepEduYanbu.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        public readonly ISchoolRepo _schoolRepo;
        private readonly IAuthorizedPeopleRepo _authorizedPeopleRepo;
        private readonly IReportsRepo _reportsRepo;

        public HomeController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            ILoggerFactory loggerFactory,
            IConfiguration config,
            ApplicationDbContext context,
            ISchoolRepo schoolRepo,
            IAuthorizedPeopleRepo authorizedPeopleRepo,
            IReportsRepo reportsRepo
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _config = config;
            _context = context;
            _schoolRepo = schoolRepo;
            _authorizedPeopleRepo = authorizedPeopleRepo;
            _reportsRepo = reportsRepo;
        }

        public IActionResult Index()
        {
            return View( new ViewModels.HomeViewModels.HomeIndexViewModel
            {
                TopReports = _reportsRepo.GetTopReportsAsEnumerable(10)
                .Select(r => new ViewModels.Shared.ReportSummary
                {
                    ReportNo = r.ReportNo,
                    EduProgramName = r.EduProgramName,
                    OwnerFullName = r.OwnerFullName,
                    SchoolName = r.SchoolName,
                    Evaluation = r.Evaluation,
                    EvaluationDate = r.EvaluationDate,
                    IsEvaluated = r.IsEvaluated,
                    VisitorOverallRating = r.VisitorOverallRating,
                    VisitorRatingCount = r.VisitorRatingCount
                })
            });
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
