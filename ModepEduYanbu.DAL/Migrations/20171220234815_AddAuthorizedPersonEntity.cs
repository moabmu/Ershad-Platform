using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class AddAuthorizedPersonEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthorizedPeople",
                columns: table => new
                {
                    AuthorizedPersonId = table.Column<string>(nullable: false),
                    AddedByUserId = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    IdNo = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    SchoolMinistryNo = table.Column<string>(nullable: true),
                    ViaNoor = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizedPeople", x => x.AuthorizedPersonId);
                    table.ForeignKey(
                        name: "FK_AuthorizedPeople_AspNetUsers_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizedPeople_AddedByUserId",
                table: "AuthorizedPeople",
                column: "AddedByUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizedPeople");
        }
    }
}
