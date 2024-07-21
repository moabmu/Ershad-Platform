using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class AlterSearchIndexInReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reports_SchoolMinistryNo_SchoolName_SigningPrincipleFullName_OwnerFullName_EduProgramName",
                table: "Reports");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_IsSignedByPrinciple_SchoolMinistryNo_SchoolName_SigningPrincipleFullName_OwnerFullName_EduProgramName",
                table: "Reports",
                columns: new[] { "IsSignedByPrinciple", "SchoolMinistryNo", "SchoolName", "SigningPrincipleFullName", "OwnerFullName", "EduProgramName" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reports_IsSignedByPrinciple_SchoolMinistryNo_SchoolName_SigningPrincipleFullName_OwnerFullName_EduProgramName",
                table: "Reports");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SchoolMinistryNo_SchoolName_SigningPrincipleFullName_OwnerFullName_EduProgramName",
                table: "Reports",
                columns: new[] { "SchoolMinistryNo", "SchoolName", "SigningPrincipleFullName", "OwnerFullName", "EduProgramName" });
        }
    }
}
