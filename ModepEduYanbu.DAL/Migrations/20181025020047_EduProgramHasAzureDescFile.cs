using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class EduProgramHasAzureDescFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionFileId",
                table: "EduPrograms",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EduProgramUploads",
                columns: table => new
                {
                    EduProgramUploadedFileId = table.Column<string>(nullable: false),
                    AzureBlobName = table.Column<string>(nullable: true),
                    AzureContainer = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    FileTitle = table.Column<string>(nullable: true),
                    Filename = table.Column<string>(nullable: true),
                    UploadedDate = table.Column<DateTime>(nullable: false),
                    Uri = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EduProgramUploads", x => x.EduProgramUploadedFileId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EduPrograms_DescriptionFileId",
                table: "EduPrograms",
                column: "DescriptionFileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EduPrograms_EduProgramUploads_DescriptionFileId",
                table: "EduPrograms",
                column: "DescriptionFileId",
                principalTable: "EduProgramUploads",
                principalColumn: "EduProgramUploadedFileId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EduPrograms_EduProgramUploads_DescriptionFileId",
                table: "EduPrograms");

            migrationBuilder.DropTable(
                name: "EduProgramUploads");

            migrationBuilder.DropIndex(
                name: "IX_EduPrograms_DescriptionFileId",
                table: "EduPrograms");

            migrationBuilder.DropColumn(
                name: "DescriptionFileId",
                table: "EduPrograms");
        }
    }
}
