using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class UpdatingEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReportId",
                table: "ReportResponses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadsLink",
                table: "Reports",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportResponses_ReportId",
                table: "ReportResponses",
                column: "ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportResponses_Reports_ReportId",
                table: "ReportResponses",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "ReportId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportResponses_Reports_ReportId",
                table: "ReportResponses");

            migrationBuilder.DropIndex(
                name: "IX_ReportResponses_ReportId",
                table: "ReportResponses");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "ReportResponses");

            migrationBuilder.DropColumn(
                name: "UploadsLink",
                table: "Reports");
        }
    }
}
