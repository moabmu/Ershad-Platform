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
    [Migration("20171227015849_RemoveViewModelEntities")]
    partial class RemoveViewModelEntities
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.4")
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
