using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class RelationshipBetweenReportAndEduYear : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EducationalYearId",
                table: "Reports",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationalYearName",
                table: "Reports",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationalYearShortName",
                table: "Reports",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_EducationalYearId",
                table: "Reports",
                column: "EducationalYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_EducationalYears_EducationalYearId",
                table: "Reports",
                column: "EducationalYearId",
                principalTable: "EducationalYears",
                principalColumn: "EducationalYearId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_EducationalYears_EducationalYearId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_EducationalYearId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "EducationalYearId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "EducationalYearName",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "EducationalYearShortName",
                table: "Reports");
        }
    }
}
