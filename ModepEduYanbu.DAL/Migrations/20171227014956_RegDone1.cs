using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class RegDone1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "AuthorizedPeople",
                nullable: true);

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

            migrationBuilder.CreateTable(
                name: "SchoolLevels",
                columns: table => new
                {
                    SchoollevelId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolLevels", x => x.SchoollevelId);
                });

            migrationBuilder.CreateTable(
                name: "SchoolTypes",
                columns: table => new
                {
                    SchoolTypeId = table.Column<string>(nullable: false),
                    ClassificationName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolTypes", x => x.SchoolTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Schools",
                columns: table => new
                {
                    SchoolId = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    LevelSchoollevelId = table.Column<string>(nullable: true),
                    MinistryNo = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PrincipleIdNo = table.Column<string>(nullable: true),
                    TypeSchoolTypeId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schools", x => x.SchoolId);
                    table.ForeignKey(
                        name: "FK_Schools_SchoolLevels_LevelSchoollevelId",
                        column: x => x.LevelSchoollevelId,
                        principalTable: "SchoolLevels",
                        principalColumn: "SchoollevelId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schools_SchoolTypes_TypeSchoolTypeId",
                        column: x => x.TypeSchoolTypeId,
                        principalTable: "SchoolTypes",
                        principalColumn: "SchoolTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSchool",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    SchooId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSchool", x => new { x.UserId, x.SchooId });
                    table.ForeignKey(
                        name: "FK_UserSchool_Schools_SchooId",
                        column: x => x.SchooId,
                        principalTable: "Schools",
                        principalColumn: "SchoolId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSchool_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizedPeople_RoleId",
                table: "AuthorizedPeople",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_LevelSchoollevelId",
                table: "Schools",
                column: "LevelSchoollevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_MinistryNo",
                table: "Schools",
                column: "MinistryNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schools_TypeSchoolTypeId",
                table: "Schools",
                column: "TypeSchoolTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSchool_SchooId",
                table: "UserSchool",
                column: "SchooId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizedPeople_AspNetRoles_RoleId",
                table: "AuthorizedPeople",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizedPeople_AspNetRoles_RoleId",
                table: "AuthorizedPeople");

            migrationBuilder.DropTable(
                name: "TestingViewModel");

            migrationBuilder.DropTable(
                name: "VerifySchoolViewModel");

            migrationBuilder.DropTable(
                name: "UserSchool");

            migrationBuilder.DropTable(
                name: "Schools");

            migrationBuilder.DropTable(
                name: "SchoolLevels");

            migrationBuilder.DropTable(
                name: "SchoolTypes");

            migrationBuilder.DropIndex(
                name: "IX_AuthorizedPeople_RoleId",
                table: "AuthorizedPeople");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "AuthorizedPeople");
        }
    }
}
