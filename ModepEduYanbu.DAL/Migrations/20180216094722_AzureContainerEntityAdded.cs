using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class AzureContainerEntityAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AzureBlobName",
                table: "ReportUploads",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AzureContainer",
                table: "ReportUploads",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileTitle",
                table: "ReportUploads",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uri",
                table: "ReportUploads",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AzureContainers",
                columns: table => new
                {
                    AzureContainerId = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Length = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AzureContainers", x => x.AzureContainerId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AzureContainers");

            migrationBuilder.DropColumn(
                name: "AzureBlobName",
                table: "ReportUploads");

            migrationBuilder.DropColumn(
                name: "AzureContainer",
                table: "ReportUploads");

            migrationBuilder.DropColumn(
                name: "FileTitle",
                table: "ReportUploads");

            migrationBuilder.DropColumn(
                name: "Uri",
                table: "ReportUploads");
        }
    }
}
