using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class IndexesAddedToReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SigningPrincipleFullName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SchoolName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SchoolMinistryNo",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerFullName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EduProgramName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SchoolMinistryNo",
                table: "Reports",
                column: "SchoolMinistryNo");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SchoolMinistryNo_SchoolName_SigningPrincipleFullName_OwnerFullName_EduProgramName",
                table: "Reports",
                columns: new[] { "SchoolMinistryNo", "SchoolName", "SigningPrincipleFullName", "OwnerFullName", "EduProgramName" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reports_SchoolMinistryNo",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_SchoolMinistryNo_SchoolName_SigningPrincipleFullName_OwnerFullName_EduProgramName",
                table: "Reports");

            migrationBuilder.AlterColumn<string>(
                name: "SigningPrincipleFullName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SchoolName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SchoolMinistryNo",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerFullName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EduProgramName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
