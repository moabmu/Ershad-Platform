﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class EduYearAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EducationalYearId",
                table: "EduPrograms",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EducationalYears",
                columns: table => new
                {
                    EducationalYearId = table.Column<string>(nullable: false),
                    BeginDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ShortName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationalYears", x => x.EducationalYearId);
                });

            migrationBuilder.CreateTable(
                name: "PrivateUserInfos",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AddedByUserFullname = table.Column<string>(nullable: true),
                    AddedByUserId = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateUserInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInfoViewModel",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(nullable: true),
                    PositionTitle = table.Column<string>(nullable: true),
                    PrivateUserInfosId = table.Column<string>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    ReportsEvaluationAverage = table.Column<double>(nullable: true),
                    Role = table.Column<string>(nullable: true),
                    SignedReportsCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfoViewModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInfoViewModel_PrivateUserInfos_PrivateUserInfosId",
                        column: x => x.PrivateUserInfosId,
                        principalTable: "PrivateUserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserInfoReports",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    EduProgramName = table.Column<string>(nullable: true),
                    Evaluation = table.Column<double>(nullable: false),
                    ReportNo = table.Column<string>(nullable: true),
                    UserInfoViewModelId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfoReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInfoReports_UserInfoViewModel_UserInfoViewModelId",
                        column: x => x.UserInfoViewModelId,
                        principalTable: "UserInfoViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserInfoSchools",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UserInfoViewModelId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfoSchools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInfoSchools_UserInfoViewModel_UserInfoViewModelId",
                        column: x => x.UserInfoViewModelId,
                        principalTable: "UserInfoViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EduPrograms_EducationalYearId",
                table: "EduPrograms",
                column: "EducationalYearId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfoReports_UserInfoViewModelId",
                table: "UserInfoReports",
                column: "UserInfoViewModelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfoSchools_UserInfoViewModelId",
                table: "UserInfoSchools",
                column: "UserInfoViewModelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfoViewModel_PrivateUserInfosId",
                table: "UserInfoViewModel",
                column: "PrivateUserInfosId");

            migrationBuilder.AddForeignKey(
                name: "FK_EduPrograms_EducationalYears_EducationalYearId",
                table: "EduPrograms",
                column: "EducationalYearId",
                principalTable: "EducationalYears",
                principalColumn: "EducationalYearId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EduPrograms_EducationalYears_EducationalYearId",
                table: "EduPrograms");

            migrationBuilder.DropTable(
                name: "EducationalYears");

            migrationBuilder.DropTable(
                name: "UserInfoReports");

            migrationBuilder.DropTable(
                name: "UserInfoSchools");

            migrationBuilder.DropTable(
                name: "UserInfoViewModel");

            migrationBuilder.DropTable(
                name: "PrivateUserInfos");

            migrationBuilder.DropIndex(
                name: "IX_EduPrograms_EducationalYearId",
                table: "EduPrograms");

            migrationBuilder.DropColumn(
                name: "EducationalYearId",
                table: "EduPrograms");
        }
    }
}