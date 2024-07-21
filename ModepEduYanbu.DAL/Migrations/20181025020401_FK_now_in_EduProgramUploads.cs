using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class FK_now_in_EduProgramUploads : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EduPrograms_EduProgramUploads_DescriptionFileId",
                table: "EduPrograms");

            migrationBuilder.DropIndex(
                name: "IX_EduPrograms_DescriptionFileId",
                table: "EduPrograms");

            migrationBuilder.DropColumn(
                name: "DescriptionFileId",
                table: "EduPrograms");

            migrationBuilder.AddColumn<string>(
                name: "EduProgramId",
                table: "EduProgramUploads",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EduProgramUploads_EduProgramId",
                table: "EduProgramUploads",
                column: "EduProgramId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EduProgramUploads_EduPrograms_EduProgramId",
                table: "EduProgramUploads",
                column: "EduProgramId",
                principalTable: "EduPrograms",
                principalColumn: "EduProgramId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EduProgramUploads_EduPrograms_EduProgramId",
                table: "EduProgramUploads");

            migrationBuilder.DropIndex(
                name: "IX_EduProgramUploads_EduProgramId",
                table: "EduProgramUploads");

            migrationBuilder.DropColumn(
                name: "EduProgramId",
                table: "EduProgramUploads");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionFileId",
                table: "EduPrograms",
                nullable: true);

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
    }
}
