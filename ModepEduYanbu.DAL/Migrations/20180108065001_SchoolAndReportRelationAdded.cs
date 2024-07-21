using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class SchoolAndReportRelationAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SchoolId",
                table: "Reports",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SchoolId",
                table: "Reports",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Schools_SchoolId",
                table: "Reports",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Schools_SchoolId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_SchoolId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Reports");
        }
    }
}
