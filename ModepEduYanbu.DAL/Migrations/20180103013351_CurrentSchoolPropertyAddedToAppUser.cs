using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class CurrentSchoolPropertyAddedToAppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentSchoolId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CurrentSchoolId",
                table: "AspNetUsers",
                column: "CurrentSchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Schools_CurrentSchoolId",
                table: "AspNetUsers",
                column: "CurrentSchoolId",
                principalTable: "Schools",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Schools_CurrentSchoolId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CurrentSchoolId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CurrentSchoolId",
                table: "AspNetUsers");
        }
    }
}
