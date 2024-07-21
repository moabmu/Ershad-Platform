using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class EduProgramEntiyAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Program_ProgramId",
                table: "Reports");

            migrationBuilder.DropTable(
                name: "Program");

            migrationBuilder.CreateTable(
                name: "EduPrograms",
                columns: table => new
                {
                    EduProgramId = table.Column<string>(nullable: false),
                    BeginDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ReportDeadline = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EduPrograms", x => x.EduProgramId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_EduPrograms_ProgramId",
                table: "Reports",
                column: "ProgramId",
                principalTable: "EduPrograms",
                principalColumn: "EduProgramId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_EduPrograms_ProgramId",
                table: "Reports");

            migrationBuilder.DropTable(
                name: "EduPrograms");

            migrationBuilder.CreateTable(
                name: "Program",
                columns: table => new
                {
                    ProgramId = table.Column<string>(nullable: false),
                    BeginDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ReportDeadline = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Program", x => x.ProgramId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Program_ProgramId",
                table: "Reports",
                column: "ProgramId",
                principalTable: "Program",
                principalColumn: "ProgramId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
