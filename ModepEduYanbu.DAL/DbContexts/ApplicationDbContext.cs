using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModepEduYanbu.Models;

namespace ModepEduYanbu.DAL.DbContexts
{
    public class ApplicationDbContextFactory
    {
        public static DbContextOptionsBuilder<ApplicationDbContext> GetOptionsBuilder(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(
                connectionString, 
                opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(45).TotalSeconds)
                );
            return optionsBuilder;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        /// <summary>
        /// Use it for the Web app. The command timeout has the default value.
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        /// <summary>
        /// Use it for Console app or long data processing operations.
        /// The command timeout has duration of 45 minutes.
        /// Do not forget to instantiate the context with 'using' clause.
        /// https://www.mikesdotnetting.com/article/317/extending-the-command-timeout-in-entity-framework-core-migrations
        /// </summary>
        /// <param name="connectionString"></param>
        public ApplicationDbContext(string connectionString) 
            : base(ApplicationDbContextFactory.GetOptionsBuilder(connectionString).Options)
        {

        }

        public DbSet<AuthorizedPerson> AuthorizedPeople { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<SchoolLevel> SchoolLevels { get; set; }
        public DbSet<SchoolType> SchoolTypes { get; set; }
        public DbSet<EduProgram> EduPrograms { get; set; }
        public DbSet<Report> Reports { get; set;}
        public DbSet<ReportResponse> ReportResponses { get; set; }
        public DbSet<ReportActivity> ReportActivities { get; set; }
        public DbSet<ReportUploadedFile> ReportUploads { get; set; }
        public DbSet<AzureContainer> AzureContainers { get; set; }
        public DbSet<EduProgramUploadedFile> EduProgramUploads { get; set; }
        public DbSet<EducationalYear> EducationalYears { get; set; }
        public DbSet<ReportKpiForMentor> ReportsKpisForMentors { get; set; }
        public DbSet<ReportKpiForSchool> ReportsKpisForSchools { get; set; }
        public DbSet<ReportKpiForEduProgram> ReportsKpisForEduPrograms { get; set; }
        public DbSet<ReportKpiRecord> HistoryRecordsForReportKpis { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EduProgram>()
                .HasIndex(x => new { x.Name, x.EducationalYearId })
                .HasName("Prevent_Identical_EduProgramNames")
                .IsUnique();

            builder.Entity<School>()
                .HasIndex(s => s.MinistryNo)
                .IsUnique();

            builder.Entity<Report>()
                .HasIndex(r => r.ReportNo)
                .IsUnique();

            builder.Entity<Report>()
                .HasIndex(r => r.SchoolMinistryNo);

            builder.Entity<Report>()
                .HasIndex(r => r.IsSignedByPrinciple);

            builder.Entity<ReportKpiRecord>()
                .HasIndex(r => r.RecordDate);

            // Report entity: Create a sequence
            builder.HasSequence<int>("ReportNoSequence", schema: "shared")
                .StartsAt(2002)
                .IncrementsBy(3);

            // Report entity: Use sequence for ReportNo in 
            builder.Entity<Report>()
                .Property(r => r.ReportNo)
                .HasDefaultValueSql("NEXT VALUE FOR shared.ReportNoSequence");

            // Relationship: one-to-one 
            // User and his title
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.PositionTitle)
                .WithOne(i => i.User)
                .HasForeignKey<UserPositionTitle>(p => p.UserForeignKey);

            // FIXED RELATIONSHIP, it should be one-to-many.
            //// Relationship: one-to-one
            //// School and its Level
            //builder.Entity<School>()
            //    .HasOne(s => s.Level)
            //    .WithOne(i => i.School)
            //    .HasForeignKey<SchoolLevel>(x => x.SchoolForeignKey);

            // Relationship: many-to-many
            // User can work for many schools, and a school contains many users
            #region User and School
            builder.Entity<UserSchool>()
                .HasKey(x => new { x.UserId, x.SchooId });

            builder.Entity<UserSchool>()
                .HasOne(x => x.User)
                .WithMany(user => user.UserSchools)
                .HasForeignKey(x => x.UserId);

            builder.Entity<UserSchool>()
                .HasOne(x => x.School)
                .WithMany(school => school.UsersForSchool)
                .HasForeignKey(x => x.SchooId);
            #endregion

            // Relationship: many-to-many
            // EduProgram has many school levels, and a school level has many eduPrograms.
            #region EduProgram and SchoolLevel
            builder.Entity<EduProgramSchoolLevel>()
                .HasKey(x => new { x.EduProgramId, x.SchoolLevelId });

            builder.Entity<EduProgramSchoolLevel>()
                .HasOne(x => x.EduProgram)
                .WithMany(eduProgram => eduProgram.SchoolLevels)
                .HasForeignKey(x => x.EduProgramId);

            builder.Entity<EduProgramSchoolLevel>()
                .HasOne(x => x.SchoolLevel)
                .WithMany(schoolLevel => schoolLevel.EduPrograms)
                .HasForeignKey(x => x.SchoolLevelId);
            #endregion

            builder.Entity<AuthorizedPerson>()
                .HasOne(x => x.AddedByUser)
                .WithMany(x => x.PeopleWhomAddedByUser); //.IsRequired(false);

            builder.Entity<EduProgram>()
                .HasOne(x => x.DescriptionFile)
                .WithOne(x => x.EduProgram)
                .HasForeignKey<EduProgramUploadedFile>(x => x.EduProgramId);

            // Relationship: many to many
            #region ReportsKpiForMentor relationship: Mentor related to many edu years, and edu year related to many mentors
            builder.Entity<ReportKpiForMentor>()
                .HasKey(x => x.KpiId);

            builder.Entity<ReportKpiForMentor>()
                .HasAlternateKey(x => new { x.OwnerId, x.EducationalYearId });

            builder.Entity<ReportKpiForMentor>()
                .HasOne(x => x.Owner) //x.Mentor
                .WithMany(mentor => mentor.ReportsKpis_IfUserIsMentor)
                .HasForeignKey(x => x.OwnerId); //x.MentorId

            builder.Entity<ReportKpiForMentor>()
                .HasOne(x => x.EducationalYear)
                .WithMany(year => year.ReportsKpisForMentors)
                .HasForeignKey(x => x.EducationalYearId);
            #endregion

            // Relationship: many to many
            #region ReportsKpiForSchool: School related to many edu years, and edu year related to many schools
            builder.Entity<ReportKpiForSchool>()
                .HasKey(x => x.KpiId);

            builder.Entity<ReportKpiForSchool>()
                .HasAlternateKey(x => new { x.OwnerId, x.EducationalYearId });

            builder.Entity<ReportKpiForSchool>()
                .HasOne(x => x.Owner)
                .WithMany(school => school.ReportsKpisForSchool)
                .HasForeignKey(x => x.OwnerId);

            builder.Entity<ReportKpiForSchool>()
                .HasOne(x => x.EducationalYear)
                .WithMany(year => year.ReportsKpisForSchools)
                .HasForeignKey(x => x.EducationalYearId);
            #endregion

            // Relationship: many to many
            #region ReportsKpiForEduProgram: EduProgram related to many edu years, and edu year related to many EduPrograms
            builder.Entity<ReportKpiForEduProgram>()
                .HasKey(x => x.KpiId);

            builder.Entity<ReportKpiForEduProgram>()
                .HasAlternateKey(x => new { x.OwnerId, x.EducationalYearId });

            builder.Entity<ReportKpiForEduProgram>()
                .HasOne(x => x.Owner)
                .WithMany(program => program.ReportsKpisForEduPrograms)
                .HasForeignKey(x => x.OwnerId);

            builder.Entity<ReportKpiForEduProgram>()
                .HasOne(x => x.EducationalYear)
                .WithMany(year => year.ReportKpisForEduPrograms)
                .HasForeignKey(x => x.EducationalYearId);
            #endregion

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (optionsBuilder.IsConfigured == false)
            //{
            //    optionsBuilder.UseSqlServer(
            //   @"Server=(localdb)\\mssqllocaldb;Database=aspnet-ModepEduYanbu-0879D7A0-52B5-4D99-9E06-C88E849F1E5A;Trusted_Connection=True;MultipleActiveResultSets=true");
            //}
            base.OnConfiguring(optionsBuilder);
        }
    }
}
