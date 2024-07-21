using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class UpdateReportEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProceduresCount",
                table: "Reports",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProceduresSuggestions",
                table: "Reports",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProceduresCount",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ProceduresSuggestions",
                table: "Reports");
        }
    }
}
