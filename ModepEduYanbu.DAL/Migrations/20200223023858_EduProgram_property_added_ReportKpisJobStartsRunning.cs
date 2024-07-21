using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class EduProgram_property_added_ReportKpisJobStartsRunning : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "QuartzJob_ComputeReportKpisJobStartsRunning",
                table: "EduPrograms",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuartzJob_ComputeReportKpisJobStartsRunning",
                table: "EduPrograms");
        }
    }
}
