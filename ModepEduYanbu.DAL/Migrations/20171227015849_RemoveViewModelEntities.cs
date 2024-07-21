using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class RemoveViewModelEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestingViewModel");

            migrationBuilder.DropTable(
                name: "VerifySchoolViewModel");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestingViewModel",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    birthDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestingViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VerifySchoolViewModel",
                columns: table => new
                {
                    SchoolId = table.Column<string>(nullable: false),
                    SchoolMinistryNo = table.Column<string>(nullable: false),
                    SchoolName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerifySchoolViewModel", x => x.SchoolId);
                });
        }
    }
}
