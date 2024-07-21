using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ModepEduYanbu.Data;
using ModepEduYanbu.DAL.DbContexts;
namespace ModepEduYanbu.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20180102003338_EduProgramEntiyAdded")]
    partial class EduProgramEntiyAdded
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.4")
                .HasAnnotation("Relational:Sequence:shared.ReportNoSequence", "'ReportNoSequence', 'shared', '2002', '3', '', '', 'Int32', 'False'")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description");

                    b.Property<string>("DescriptionAsTitle");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("AddedByUserId");

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<int>("FailedAttemptsToVerfiyPhoneNumber");

                    b.Property<string>("FullName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<DateTime>("RegDate");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.Property<bool>("ViaNoor");

                    b.HasKey("Id");

                    b.HasIndex("AddedByUserId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.AuthorizedPerson", b =>
                {
                    b.Property<string>("AuthorizedPersonId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddedByUserId");

                    b.Property<string>("Email");

                    b.Property<string>("FullName");

                    b.Property<string>("IdNo")
                        .IsRequired();

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("RoleId");

                    b.Property<string>("SchoolMinistryNo");

                    b.Property<bool>("ViaNoor");

                    b.HasKey("AuthorizedPersonId");

                    b.HasIndex("AddedByUserId");

                    b.HasIndex("RoleId");

                    b.ToTable("AuthorizedPeople");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.EduProgram", b =>
                {
                    b.Property<string>("EduProgramId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("BeginDate");

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("Name");

                    b.Property<DateTime>("ReportDeadline");

                    b.HasKey("EduProgramId");

                    b.ToTable("EduPrograms");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.Report", b =>
                {
                    b.Property<string>("ReportId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AllowEdit");

                    b.Property<string>("ChallengesSolus");

                    b.Property<double>("Evaluation");

                    b.Property<DateTime?>("EvaluationDate");

                    b.Property<string>("EvaluatorFullName");

                    b.Property<string>("EvaluatorId");

                    b.Property<string>("EvaluatorIdNo");

                    b.Property<string>("ExecutionData");

                    b.Property<DateTime?>("ExecutionDate");

                    b.Property<int>("ExecutionPeriod");

                    b.Property<string>("Field");

                    b.Property<string>("OwnerFullName");

                    b.Property<string>("OwnerId");

                    b.Property<string>("OwnerIdNo");

                    b.Property<double>("ParticipantsRatio");

                    b.Property<string>("PictureName");

                    b.Property<string>("ProgramId");

                    b.Property<string>("ProgramName");

                    b.Property<string>("ReportNo")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("NEXT VALUE FOR shared.ReportNoSequence");

                    b.Property<string>("SchoolName");

                    b.Property<DateTime?>("SentDateTime");

                    b.Property<DateTime?>("SigningDateTime");

                    b.Property<string>("SigningPrincipleFullName");

                    b.Property<string>("SigningPrincipleId");

                    b.Property<string>("SigningPrincipleIdNo");

                    b.Property<int>("TargetedCount");

                    b.Property<string>("TargetedSlice");

                    b.HasKey("ReportId");

                    b.HasIndex("EvaluatorId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ProgramId");

                    b.HasIndex("ReportNo")
                        .IsUnique();

                    b.HasIndex("SigningPrincipleId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.ReportResponse", b =>
                {
                    b.Property<string>("ReportResponseId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("OwnerFullName");

                    b.Property<string>("OwnerId");

                    b.Property<string>("OwnerIdNo");

                    b.HasKey("ReportResponseId");

                    b.HasIndex("OwnerId");

                    b.ToTable("ReportResponses");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.ReportUploadedFile", b =>
                {
                    b.Property<string>("ReportUploadedFileId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Extension");

                    b.Property<string>("Filename");

                    b.Property<string>("ReportId");

                    b.Property<DateTime>("UploadedDate");

                    b.HasKey("ReportUploadedFileId");

                    b.HasIndex("ReportId");

                    b.ToTable("ReportUploads");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.School", b =>
                {
                    b.Property<string>("SchoolId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("City");

                    b.Property<string>("LevelSchoollevelId");

                    b.Property<string>("MinistryNo");

                    b.Property<string>("Name");

                    b.Property<string>("PrincipleIdNo");

                    b.Property<string>("TypeSchoolTypeId");

                    b.HasKey("SchoolId");

                    b.HasIndex("LevelSchoollevelId");

                    b.HasIndex("MinistryNo")
                        .IsUnique();

                    b.HasIndex("TypeSchoolTypeId");

                    b.ToTable("Schools");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.SchoolLevel", b =>
                {
                    b.Property<string>("SchoollevelId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("SchoollevelId");

                    b.ToTable("SchoolLevels");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.SchoolType", b =>
                {
                    b.Property<string>("SchoolTypeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClassificationName");

                    b.HasKey("SchoolTypeId");

                    b.ToTable("SchoolTypes");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.UserPositionTitle", b =>
                {
                    b.Property<string>("UserPositionTitleId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PositionTitle");

                    b.Property<string>("UserForeignKey");

                    b.HasKey("UserPositionTitleId");

                    b.HasIndex("UserForeignKey")
                        .IsUnique();

                    b.ToTable("UserPositionTitle");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.UserSchool", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("SchooId");

                    b.HasKey("UserId", "SchooId");

                    b.HasIndex("SchooId");

                    b.ToTable("UserSchool");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.ApplicationRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.ApplicationRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ModepEduYanbu.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ModepEduYanbu.Models.ApplicationUser", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.ApplicationUser", "AddedByUser")
                        .WithMany()
                        .HasForeignKey("AddedByUserId");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.AuthorizedPerson", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.ApplicationUser", "AddedByUser")
                        .WithMany()
                        .HasForeignKey("AddedByUserId");

                    b.HasOne("ModepEduYanbu.Models.ApplicationRole", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.Report", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.ApplicationUser", "Evaluator")
                        .WithMany()
                        .HasForeignKey("EvaluatorId");

                    b.HasOne("ModepEduYanbu.Models.ApplicationUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");

                    b.HasOne("ModepEduYanbu.Models.EduProgram", "Program")
                        .WithMany()
                        .HasForeignKey("ProgramId");

                    b.HasOne("ModepEduYanbu.Models.ApplicationUser", "SigningPrinciple")
                        .WithMany()
                        .HasForeignKey("SigningPrincipleId");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.ReportResponse", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.ApplicationUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.ReportUploadedFile", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.Report", "Report")
                        .WithMany("Uploads")
                        .HasForeignKey("ReportId");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.School", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.SchoolLevel", "Level")
                        .WithMany("Schools")
                        .HasForeignKey("LevelSchoollevelId");

                    b.HasOne("ModepEduYanbu.Models.SchoolType", "Type")
                        .WithMany("Schools")
                        .HasForeignKey("TypeSchoolTypeId");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.UserPositionTitle", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.ApplicationUser", "User")
                        .WithOne("PositionTitle")
                        .HasForeignKey("ModepEduYanbu.Models.UserPositionTitle", "UserForeignKey");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.UserSchool", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.School", "School")
                        .WithMany("UsersForSchool")
                        .HasForeignKey("SchooId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ModepEduYanbu.Models.ApplicationUser", "User")
                        .WithMany("UserSchools")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
