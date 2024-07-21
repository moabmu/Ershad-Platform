using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class RelationshipReportsAndProgramsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_EduPrograms_ProgramId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ProgramId",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "ProgramName",
                table: "Reports",
                newName: "SchoolMinistryNo");

            migrationBuilder.RenameColumn(
                name: "ProgramId",
                table: "Reports",
                newName: "EduProgramName");

            migrationBuilder.AlterColumn<string>(
                name: "EduProgramName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EduProgramId",
                table: "Reports",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_EduProgramId",
                table: "Reports",
                column: "EduProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_EduPrograms_EduProgramId",
                table: "Reports",
                column: "EduProgramId",
                principalTable: "EduPrograms",
                principalColumn: "EduProgramId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_EduPrograms_EduProgramId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_EduProgramId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "EduProgramId",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "SchoolMinistryNo",
                table: "Reports",
                newName: "ProgramName");

            migrationBuilder.RenameColumn(
                name: "EduProgramName",
                table: "Reports",
                newName: "ProgramId");

            migrationBuilder.AlterColumn<string>(
                name: "ProgramId",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ProgramId",
                table: "Reports",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_EduPrograms_ProgramId",
                table: "Reports",
                column: "ProgramId",
                principalTable: "EduPrograms",
                principalColumn: "EduProgramId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
