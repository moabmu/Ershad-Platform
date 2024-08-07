﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ModepEduYanbu.Data;
using ModepEduYanbu.DAL.DbContexts;
namespace ModepEduYanbu.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20181102124441_EduYearAdded")]
    partial class EduYearAdded
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.6")
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

                    b.Property<string>("CurrentSchoolId");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<int>("FailedAttemptsToVerfiyPhoneNumber");

                    b.Property<string>("FullName");

                    b.Property<DateTime?>("LastManualSmsSendRequest");

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

                    b.HasIndex("CurrentSchoolId");

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

                    b.Property<DateTime>("CreatedData");

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

            modelBuilder.Entity("ModepEduYanbu.Models.AzureContainer", b =>
                {
                    b.Property<string>("AzureContainerId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<long>("Length");

                    b.Property<string>("Name");

                    b.Property<string>("Uri");

                    b.HasKey("AzureContainerId");

                    b.ToTable("AzureContainers");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.EducationalYear", b =>
                {
                    b.Property<string>("EducationalYearId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("BeginDate");

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("Name");

                    b.Property<string>("ShortName");

                    b.HasKey("EducationalYearId");

                    b.ToTable("EducationalYears");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.EduProgram", b =>
                {
                    b.Property<string>("EduProgramId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("BeginDate");

                    b.Property<string>("DescriptionFileExtension");

                    b.Property<string>("EducationalYearId");

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("Name");

                    b.Property<DateTime>("ReportDeadline");

                    b.HasKey("EduProgramId");

                    b.HasIndex("EducationalYearId");

                    b.ToTable("EduPrograms");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.EduProgramUploadedFile", b =>
                {
                    b.Property<string>("EduProgramUploadedFileId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AzureBlobName");

                    b.Property<string>("AzureContainer");

                    b.Property<string>("EduProgramId");

                    b.Property<string>("Extension");

                    b.Property<string>("FileTitle");

                    b.Property<string>("Filename");

                    b.Property<DateTime>("UploadedDate");

                    b.Property<string>("Uri");

                    b.HasKey("EduProgramUploadedFileId");

                    b.HasIndex("EduProgramId")
                        .IsUnique();

                    b.ToTable("EduProgramUploads");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.InfosViewModels.PrivateUserInfos", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddedByUserFullname");

                    b.Property<string>("AddedByUserId");

                    b.Property<string>("Email");

                    b.Property<string>("PhoneNumber");

                    b.HasKey("Id");

                    b.ToTable("PrivateUserInfos");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.InfosViewModels.UserInfoReports", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("EduProgramName");

                    b.Property<double>("Evaluation");

                    b.Property<string>("ReportNo");

                    b.Property<string>("UserInfoViewModelId");

                    b.HasKey("Id");

                    b.HasIndex("UserInfoViewModelId");

                    b.ToTable("UserInfoReports");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.InfosViewModels.UserInfoSchools", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("UserInfoViewModelId");

                    b.HasKey("Id");

                    b.HasIndex("UserInfoViewModelId");

                    b.ToTable("UserInfoSchools");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.InfosViewModels.UserInfoViewModel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FullName");

                    b.Property<string>("PositionTitle");

                    b.Property<string>("PrivateUserInfosId");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<double?>("ReportsEvaluationAverage");

                    b.Property<string>("Role");

                    b.Property<int>("SignedReportsCount");

                    b.HasKey("Id");

                    b.HasIndex("PrivateUserInfosId");

                    b.ToTable("UserInfoViewModel");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.Report", b =>
                {
                    b.Property<string>("ReportId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AllowEdit");

                    b.Property<string>("ChallengesSolus");

                    b.Property<string>("EduProgramId");

                    b.Property<string>("EduProgramName");

                    b.Property<double>("Evaluation");

                    b.Property<DateTime?>("EvaluationDate");

                    b.Property<string>("EvaluatorFullName");

                    b.Property<string>("EvaluatorId");

                    b.Property<string>("EvaluatorIdNo");

                    b.Property<string>("ExecutionData");

                    b.Property<DateTime?>("ExecutionDate");

                    b.Property<int>("ExecutionPeriod");

                    b.Property<string>("Field");

                    b.Property<bool>("IsEvaluated");

                    b.Property<bool>("IsSignedByPrinciple");

                    b.Property<string>("OwnerFullName");

                    b.Property<string>("OwnerId");

                    b.Property<string>("OwnerIdNo");

                    b.Property<double>("ParticipantsRatio");

                    b.Property<int>("ProceduresCount");

                    b.Property<string>("ProceduresSuggestions");

                    b.Property<string>("ReportNo")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("NEXT VALUE FOR shared.ReportNoSequence");

                    b.Property<string>("SchoolId");

                    b.Property<string>("SchoolMinistryNo");

                    b.Property<string>("SchoolName");

                    b.Property<DateTime?>("SentDateTime");

                    b.Property<DateTime?>("SigningDateTime");

                    b.Property<string>("SigningPrincipleFullName");

                    b.Property<string>("SigningPrincipleId");

                    b.Property<string>("SigningPrincipleIdNo");

                    b.Property<int>("TargetedCount");

                    b.Property<string>("TargetedSlice");

                    b.Property<string>("UploadsLink");

                    b.Property<int>("VisitorOverallRating");

                    b.Property<int>("VisitorRatingCount");

                    b.HasKey("ReportId");

                    b.HasIndex("EduProgramId");

                    b.HasIndex("EvaluatorId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ReportNo")
                        .IsUnique();

                    b.HasIndex("SchoolId");

                    b.HasIndex("SchoolMinistryNo");

                    b.HasIndex("SigningPrincipleId");

                    b.HasIndex("SchoolMinistryNo", "SchoolName", "SigningPrincipleFullName", "OwnerFullName", "EduProgramName");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.ReportActivity", b =>
                {
                    b.Property<string>("ReportActivityId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Email");

                    b.Property<string>("FullName");

                    b.Property<string>("IpAddress");

                    b.Property<int>("Rating");

                    b.Property<string>("ReportId");

                    b.HasKey("ReportActivityId");

                    b.HasIndex("ReportId");

                    b.ToTable("ReportActivities");
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

                    b.Property<string>("ReportId");

                    b.HasKey("ReportResponseId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ReportId");

                    b.ToTable("ReportResponses");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.ReportUploadedFile", b =>
                {
                    b.Property<string>("ReportUploadedFileId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AzureBlobName");

                    b.Property<string>("AzureContainer");

                    b.Property<string>("Extension");

                    b.Property<string>("FileTitle");

                    b.Property<string>("Filename");

                    b.Property<string>("ReportId");

                    b.Property<DateTime>("UploadedDate");

                    b.Property<string>("Uri");

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

                    b.Property<string>("PhoneNumber");

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

                    b.HasOne("ModepEduYanbu.Models.School", "CurrentSchool")
                        .WithMany("CurrentUsers")
                        .HasForeignKey("CurrentSchoolId");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.AuthorizedPerson", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.ApplicationUser", "AddedByUser")
                        .WithMany("PeopleWhomAddedByUser")
                        .HasForeignKey("AddedByUserId");

                    b.HasOne("ModepEduYanbu.Models.ApplicationRole", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.EduProgram", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.EducationalYear", "EducationalYear")
                        .WithMany("EduPrograms")
                        .HasForeignKey("EducationalYearId");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.EduProgramUploadedFile", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.EduProgram", "EduProgram")
                        .WithOne("DescriptionFile")
                        .HasForeignKey("ModepEduYanbu.Models.EduProgramUploadedFile", "EduProgramId");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.InfosViewModels.UserInfoReports", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.InfosViewModels.UserInfoViewModel")
                        .WithMany("Reports")
                        .HasForeignKey("UserInfoViewModelId");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.InfosViewModels.UserInfoSchools", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.InfosViewModels.UserInfoViewModel")
                        .WithMany("Schools")
                        .HasForeignKey("UserInfoViewModelId");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.InfosViewModels.UserInfoViewModel", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.InfosViewModels.PrivateUserInfos", "PrivateUserInfos")
                        .WithMany()
                        .HasForeignKey("PrivateUserInfosId");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.Report", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.EduProgram", "EduProgram")
                        .WithMany("Reports")
                        .HasForeignKey("EduProgramId");

                    b.HasOne("ModepEduYanbu.Models.ApplicationUser", "Evaluator")
                        .WithMany()
                        .HasForeignKey("EvaluatorId");

                    b.HasOne("ModepEduYanbu.Models.ApplicationUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");

                    b.HasOne("ModepEduYanbu.Models.School", "School")
                        .WithMany("Reports")
                        .HasForeignKey("SchoolId");

                    b.HasOne("ModepEduYanbu.Models.ApplicationUser", "SigningPrinciple")
                        .WithMany()
                        .HasForeignKey("SigningPrincipleId");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.ReportActivity", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.Report", "Report")
                        .WithMany("Activities")
                        .HasForeignKey("ReportId");
                });

            modelBuilder.Entity("ModepEduYanbu.Models.ReportResponse", b =>
                {
                    b.HasOne("ModepEduYanbu.Models.ApplicationUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");

                    b.HasOne("ModepEduYanbu.Models.Report", "Report")
                        .WithMany("Responses")
                        .HasForeignKey("ReportId");
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
