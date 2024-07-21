using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class EduProgramAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureName",
                table: "Reports");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionFileExtension",
                table: "EduPrograms",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionFileExtension",
                table: "EduPrograms");

            migrationBuilder.AddColumn<string>(
                name: "PictureName",
                table: "Reports",
                nullable: true);
        }
    }
}
