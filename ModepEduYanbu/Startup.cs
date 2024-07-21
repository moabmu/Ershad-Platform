using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModepEduYanbu.DAL.DbContexts;
using ModepEduYanbu.Data;
using ModepEduYanbu.Helpers;
using ModepEduYanbu.Jobs;
using ModepEduYanbu.Models;
using ModepEduYanbu.Models.AccountViewModels;
using ModepEduYanbu.Models.AdminViewModels;
using ModepEduYanbu.Models.EduProgramViewModels;
using ModepEduYanbu.Models.ReportViewModel;
using ModepEduYanbu.Repositories;
using ModepEduYanbu.Repositories.Interfaces;
using ModepEduYanbu.Services;
using Quartz;
using QuartzWrapper;
using QuartzWrapper.Extensions;
using Sakura.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ModepEduYanbu
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.

            // Angular's default header name for sending the XSRF token.
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            var connStr = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connStr));

            services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
            {
                config.Lockout = new LockoutOptions
                {
                    AllowedForNewUsers = true,
                    DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5),
                    MaxFailedAccessAttempts = 5
                };
                config.SignIn.RequireConfirmedPhoneNumber = false;
                config.User.RequireUniqueEmail = true;

                config.Password.RequiredLength = 6;
                config.Password.RequireDigit = false;
                config.Password.RequireUppercase = false;
                config.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()

            services.AddMvc()
            .AddJsonOptions(o => 
                o.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver()
            )
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddLogging();
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
            });

            // Add application services:
            #region Services
            // General services:
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IPhoneCaller, AuthMessageSender>();
            services.AddTransient<IRequestValidationService, RequestValidationService>();
            services.Configure<SMSoptions>(Configuration);
            services.AddTransient<ContextSeeder>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<ExcelReportsManager>();
            services.AddBootstrapPagerGenerator(options => // Add default bootstrap-styled pager implementation
            {
                options.ConfigureDefault(); // Use default pager options.
            });

            // Repositories
            services.AddScoped<IAuthorizedPeopleRepo, AuthorizedPeopleRepo>();
            services.AddScoped<ISchoolRepo, SchoolRepo>();
            services.AddScoped<IEduProgramsRepo, EduProgramsRepo>();
            services.AddScoped<IReportsRepo, ReportsRepo>();
            services.AddScoped<IUsersRepo, UsersRepo>();
            services.AddScoped<IAzureContainersRepo, AzureContainersRepo>();
            services.AddScoped<IEducationalYearsRepo, EducationalYearsRepo>();
            services.AddScoped<IKpiRepo, KpiRepo>();
            #endregion

            // Quartz Hosted Service
            services.AddQuartzWrapper();

            // Quartz Jobs
            services.AddScoped<ComputeReportKpisForFinishedEduProgram>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            ContextSeeder seeder, 
            ApplicationDbContext context, 
            QuartzHostedService quartzService,
            IEducationalYearsRepo yearsRepo)
        {
            //Register Syncfusion license
            var ej2LicenseKey = "MzkxMDFAMzEzNjJlMzMyZTMwandiajNwbVV1WElnb01oM1FDOXpMTmxlVXRrK0poSURTaWtiVk9XTFI2Yz0=";
            //var ej2UnlockKeyForDevelopment = "@31362e332e30gTpWc+6tqKxuj6Kq85YA7N9GEasS1VJbpYq5STYcsno=";
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(ej2LicenseKey);

            #region Hijri Date
            var defaultSaudiCulture = new List<CultureInfo> { new CultureInfo("ar-SA") };
            var supportedCultures = new List<CultureInfo> { new CultureInfo("ar-SA") };

            var shortDatePattern = "d/M/yyyy هـ";
            var saudiCulture = supportedCultures[0];
            saudiCulture.DateTimeFormat.Calendar = new UmAlQuraCalendar();
            saudiCulture.DateTimeFormat.ShortDatePattern = shortDatePattern;

            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ar-SA"),
                SupportedCultures = defaultSaudiCulture,
                SupportedUICultures = supportedCultures,
            };

            app.UseRequestLocalization(localizationOptions);
            #endregion

            // AutoMapper configurations
            Mapper.Initialize(config =>
            {
                // string => DateTime for HijriDate in AddReportViewModel ...etc
                config.CreateMap<string, DateTime>().ConvertUsing(str => DateTime.Parse(str));
                config.CreateMap<DateTime, string>().ConvertUsing(date => date.ToShortDateString());

                config.CreateMap<School, VerifySchoolViewModel>()
                .ForMember(x => x.SchoolName, opt => opt.MapFrom(src => src.Name))
                .ForMember(x => x.SchoolMinistryNo, opt => opt.Ignore()); // User must enter MinistryNo

                // from VM to Entity
                config.CreateMap<EduProgramViewModel, EduProgram>()
                .ForMember(x => x.DescriptionFileExtension, opt => opt.Ignore())
                .ForMember(x => x.DescriptionFile, opt => opt.Ignore());

                config.CreateMap<EduProgram, EduProgramViewModel>()
                .ForMember(x => x.DescriptionFile, opt => opt.Ignore());

                // From VM to Entity
                config.CreateMap<AddReportViewModel, Report>(memberList: MemberList.Source);

                // From Entity to VM
                config.CreateMap<Report, AddReportViewModel>(memberList: MemberList.Destination);

                //From Entity to VM
                config.CreateMap<Report, ShowReportViewModel>(memberList: MemberList.Source);

                // From VM to Entity
                config.CreateMap<AddSchoolViewModel, School>(memberList: MemberList.Source);
            });

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Prevent Click Jacking attack
            // For more information: https://neelbhatt.com/2018/02/16/secure-net-core-applications-from-click-jacking-net-core-security-part-iii/
            app.UseXfo(options => options.Deny());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            #region Schedule jobs
            var currentYear = yearsRepo.GetCurrentYearOrCreateIfNotExist();
            var upcomingEduPrograms = context.EduPrograms
                .Where(x => DateTime.Compare(DateTime.Now, x.ReportDeadline) < 0
                && !x.QuartzJob_ComputeReportKpisJobStartsRunning);
            foreach (var eduProgram in upcomingEduPrograms)
            {
                var id = $"ComputeReportKpi_LostJob_{Guid.NewGuid().ToString()}";
                var jobDetails = JobBuilder
                    .CreateForAsync<ComputeReportKpisForFinishedEduProgram>()
                    .WithIdentity(id)
                    .WithDescription(id)
                    .Build();

                var trigger = TriggerBuilder
                    .Create()
                    .StartAt(eduProgram.ReportDeadline.AddSeconds(1.0)) //eduProgram.ReportDeadline
                    .WithIdentity(id)
                    .WithDescription(id)
                    .UsingJobData("ReportDeadline", eduProgram.ReportDeadline.ToString())
                    .UsingJobData("EduProgramId", eduProgram.EduProgramId)
                    .Build();

                quartzService.ScheduleJob<ComputeReportKpisForFinishedEduProgram>(jobDetails, trigger).Wait();

                eduProgram.QuartzJob_CurrentTriggerKeyGroup = trigger.Key.Group;
                eduProgram.QuartzJob_CurrentTriggerKeyName = trigger.Key.Name;
            }
            context.SaveChangesAsync().Wait();
            #endregion

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
