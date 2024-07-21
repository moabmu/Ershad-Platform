using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class DecimalDomainChangedForKpiBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "KpiNumerator",
                table: "ReportsKpisForSchools",
                type: "decimal(28,21)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "KpiNumerator",
                table: "ReportsKpisForMentors",
                type: "decimal(28,21)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "KpiNumerator",
                table: "ReportsKpisForEduPrograms",
                type: "decimal(28,21)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.CreateIndex(
                name: "IX_HistoryRecordsForReportKpis_RecordDate",
                table: "HistoryRecordsForReportKpis",
                column: "RecordDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HistoryRecordsForReportKpis_RecordDate",
                table: "HistoryRecordsForReportKpis");

            migrationBuilder.AlterColumn<decimal>(
                name: "KpiNumerator",
                table: "ReportsKpisForSchools",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,21)");

            migrationBuilder.AlterColumn<decimal>(
                name: "KpiNumerator",
                table: "ReportsKpisForMentors",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,21)");

            migrationBuilder.AlterColumn<decimal>(
                name: "KpiNumerator",
                table: "ReportsKpisForEduPrograms",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,21)");
        }
    }
}
