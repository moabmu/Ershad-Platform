using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class UpdatesOnReportKpi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schools_SchoolLevels_LevelSchoollevelId",
                table: "Schools");

            migrationBuilder.RenameColumn(
                name: "LevelSchoollevelId",
                table: "Schools",
                newName: "LevelId");

            migrationBuilder.RenameIndex(
                name: "IX_Schools_LevelSchoollevelId",
                table: "Schools",
                newName: "IX_Schools_LevelId");

            migrationBuilder.AddColumn<string>(
                name: "MentorId",
                table: "HistoryRecordsForReportKpis",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchoolId",
                table: "HistoryRecordsForReportKpis",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoryRecordsForReportKpis_MentorId",
                table: "HistoryRecordsForReportKpis",
                column: "MentorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryRecordsForReportKpis_SchoolId",
                table: "HistoryRecordsForReportKpis",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoryRecordsForReportKpis_AspNetUsers_MentorId",
                table: "HistoryRecordsForReportKpis",
                column: "MentorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoryRecordsForReportKpis_Schools_SchoolId",
                table: "HistoryRecordsForReportKpis",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schools_SchoolLevels_LevelId",
                table: "Schools",
                column: "LevelId",
                principalTable: "SchoolLevels",
                principalColumn: "SchoollevelId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoryRecordsForReportKpis_AspNetUsers_MentorId",
                table: "HistoryRecordsForReportKpis");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoryRecordsForReportKpis_Schools_SchoolId",
                table: "HistoryRecordsForReportKpis");

            migrationBuilder.DropForeignKey(
                name: "FK_Schools_SchoolLevels_LevelId",
                table: "Schools");

            migrationBuilder.DropIndex(
                name: "IX_HistoryRecordsForReportKpis_MentorId",
                table: "HistoryRecordsForReportKpis");

            migrationBuilder.DropIndex(
                name: "IX_HistoryRecordsForReportKpis_SchoolId",
                table: "HistoryRecordsForReportKpis");

            migrationBuilder.DropColumn(
                name: "MentorId",
                table: "HistoryRecordsForReportKpis");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "HistoryRecordsForReportKpis");

            migrationBuilder.RenameColumn(
                name: "LevelId",
                table: "Schools",
                newName: "LevelSchoollevelId");

            migrationBuilder.RenameIndex(
                name: "IX_Schools_LevelId",
                table: "Schools",
                newName: "IX_Schools_LevelSchoollevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schools_SchoolLevels_LevelSchoollevelId",
                table: "Schools",
                column: "LevelSchoollevelId",
                principalTable: "SchoolLevels",
                principalColumn: "SchoollevelId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
