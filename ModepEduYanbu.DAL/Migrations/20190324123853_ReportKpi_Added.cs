using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class ReportKpi_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserPositionTitle_UserForeignKey",
                table: "UserPositionTitle");

            migrationBuilder.DropIndex(
                name: "IX_Schools_MinistryNo",
                table: "Schools");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReportNo",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_IsSignedByPrinciple_SchoolMinistryNo_SchoolName_SigningPrincipleFullName_OwnerFullName_EduProgramName",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_EduProgramUploads_EduProgramId",
                table: "EduProgramUploads");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.AlterColumn<string>(
                name: "SigningPrincipleFullName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SchoolName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerFullName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EduProgramName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EvaluationBase",
                table: "EducationalYears",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ReportsKpisForEduPrograms",
                columns: table => new
                {
                    KpiId = table.Column<string>(nullable: false),
                    KpiNumerator = table.Column<decimal>(nullable: false),
                    KpiDenominator = table.Column<long>(nullable: false),
                    KpiUpdatingDate = table.Column<DateTimeOffset>(nullable: false),
                    OwnerId = table.Column<string>(nullable: false),
                    EducationalYearId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportsKpisForEduPrograms", x => x.KpiId);
                    table.UniqueConstraint("AK_ReportsKpisForEduPrograms_OwnerId_EducationalYearId", x => new { x.OwnerId, x.EducationalYearId });
                    table.ForeignKey(
                        name: "FK_ReportsKpisForEduPrograms_EducationalYears_EducationalYearId",
                        column: x => x.EducationalYearId,
                        principalTable: "EducationalYears",
                        principalColumn: "EducationalYearId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportsKpisForEduPrograms_EduPrograms_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "EduPrograms",
                        principalColumn: "EduProgramId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportsKpisForMentors",
                columns: table => new
                {
                    KpiId = table.Column<string>(nullable: false),
                    KpiNumerator = table.Column<decimal>(nullable: false),
                    KpiDenominator = table.Column<long>(nullable: false),
                    KpiUpdatingDate = table.Column<DateTimeOffset>(nullable: false),
                    OwnerId = table.Column<string>(nullable: false),
                    EducationalYearId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportsKpisForMentors", x => x.KpiId);
                    table.UniqueConstraint("AK_ReportsKpisForMentors_OwnerId_EducationalYearId", x => new { x.OwnerId, x.EducationalYearId });
                    table.ForeignKey(
                        name: "FK_ReportsKpisForMentors_EducationalYears_EducationalYearId",
                        column: x => x.EducationalYearId,
                        principalTable: "EducationalYears",
                        principalColumn: "EducationalYearId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportsKpisForMentors_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportsKpisForSchools",
                columns: table => new
                {
                    KpiId = table.Column<string>(nullable: false),
                    KpiNumerator = table.Column<decimal>(nullable: false),
                    KpiDenominator = table.Column<long>(nullable: false),
                    KpiUpdatingDate = table.Column<DateTimeOffset>(nullable: false),
                    OwnerId = table.Column<string>(nullable: false),
                    EducationalYearId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportsKpisForSchools", x => x.KpiId);
                    table.UniqueConstraint("AK_ReportsKpisForSchools_OwnerId_EducationalYearId", x => new { x.OwnerId, x.EducationalYearId });
                    table.ForeignKey(
                        name: "FK_ReportsKpisForSchools_EducationalYears_EducationalYearId",
                        column: x => x.EducationalYearId,
                        principalTable: "EducationalYears",
                        principalColumn: "EducationalYearId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportsKpisForSchools_Schools_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Schools",
                        principalColumn: "SchoolId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoryRecordsForReportKpis",
                columns: table => new
                {
                    KpiRecordId = table.Column<string>(nullable: false),
                    RecordValue = table.Column<decimal>(type: "decimal(11,10)", nullable: false),
                    RecordDate = table.Column<DateTimeOffset>(nullable: false),
                    ReportId = table.Column<string>(nullable: true),
                    EduProgramId = table.Column<string>(nullable: true),
                    RecordCreatorId = table.Column<string>(nullable: true),
                    PreviousRecordGetResetByThisRecordId = table.Column<string>(nullable: true),
                    ReportKpiForMentor_OwnsThisRecordKpiId = table.Column<string>(nullable: true),
                    ReportKpiForSchool_OwnsThisRecordKpiId = table.Column<string>(nullable: true),
                    ReportKpiForEduProgram_OwnsThisRecordKpiId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryRecordsForReportKpis", x => x.KpiRecordId);
                    table.ForeignKey(
                        name: "FK_HistoryRecordsForReportKpis_EduPrograms_EduProgramId",
                        column: x => x.EduProgramId,
                        principalTable: "EduPrograms",
                        principalColumn: "EduProgramId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoryRecordsForReportKpis_HistoryRecordsForReportKpis_PreviousRecordGetResetByThisRecordId",
                        column: x => x.PreviousRecordGetResetByThisRecordId,
                        principalTable: "HistoryRecordsForReportKpis",
                        principalColumn: "KpiRecordId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoryRecordsForReportKpis_AspNetUsers_RecordCreatorId",
                        column: x => x.RecordCreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoryRecordsForReportKpis_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "ReportId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoryRecordsForReportKpis_ReportsKpisForEduPrograms_ReportKpiForEduProgram_OwnsThisRecordKpiId",
                        column: x => x.ReportKpiForEduProgram_OwnsThisRecordKpiId,
                        principalTable: "ReportsKpisForEduPrograms",
                        principalColumn: "KpiId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoryRecordsForReportKpis_ReportsKpisForMentors_ReportKpiForMentor_OwnsThisRecordKpiId",
                        column: x => x.ReportKpiForMentor_OwnsThisRecordKpiId,
                        principalTable: "ReportsKpisForMentors",
                        principalColumn: "KpiId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoryRecordsForReportKpis_ReportsKpisForSchools_ReportKpiForSchool_OwnsThisRecordKpiId",
                        column: x => x.ReportKpiForSchool_OwnsThisRecordKpiId,
                        principalTable: "ReportsKpisForSchools",
                        principalColumn: "KpiId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPositionTitle_UserForeignKey",
                table: "UserPositionTitle",
                column: "UserForeignKey",
                unique: true,
                filter: "[UserForeignKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_MinistryNo",
                table: "Schools",
                column: "MinistryNo",
                unique: true,
                filter: "[MinistryNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_IsSignedByPrinciple",
                table: "Reports",
                column: "IsSignedByPrinciple");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportNo",
                table: "Reports",
                column: "ReportNo",
                unique: true,
                filter: "[ReportNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EduProgramUploads_EduProgramId",
                table: "EduProgramUploads",
                column: "EduProgramId",
                unique: true,
                filter: "[EduProgramId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryRecordsForReportKpis_EduProgramId",
                table: "HistoryRecordsForReportKpis",
                column: "EduProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryRecordsForReportKpis_PreviousRecordGetResetByThisRecordId",
                table: "HistoryRecordsForReportKpis",
                column: "PreviousRecordGetResetByThisRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryRecordsForReportKpis_RecordCreatorId",
                table: "HistoryRecordsForReportKpis",
                column: "RecordCreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryRecordsForReportKpis_ReportId",
                table: "HistoryRecordsForReportKpis",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryRecordsForReportKpis_ReportKpiForEduProgram_OwnsThisRecordKpiId",
                table: "HistoryRecordsForReportKpis",
                column: "ReportKpiForEduProgram_OwnsThisRecordKpiId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryRecordsForReportKpis_ReportKpiForMentor_OwnsThisRecordKpiId",
                table: "HistoryRecordsForReportKpis",
                column: "ReportKpiForMentor_OwnsThisRecordKpiId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryRecordsForReportKpis_ReportKpiForSchool_OwnsThisRecordKpiId",
                table: "HistoryRecordsForReportKpis",
                column: "ReportKpiForSchool_OwnsThisRecordKpiId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsKpisForEduPrograms_EducationalYearId",
                table: "ReportsKpisForEduPrograms",
                column: "EducationalYearId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsKpisForMentors_EducationalYearId",
                table: "ReportsKpisForMentors",
                column: "EducationalYearId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsKpisForSchools_EducationalYearId",
                table: "ReportsKpisForSchools",
                column: "EducationalYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "HistoryRecordsForReportKpis");

            migrationBuilder.DropTable(
                name: "ReportsKpisForEduPrograms");

            migrationBuilder.DropTable(
                name: "ReportsKpisForMentors");

            migrationBuilder.DropTable(
                name: "ReportsKpisForSchools");

            migrationBuilder.DropIndex(
                name: "IX_UserPositionTitle_UserForeignKey",
                table: "UserPositionTitle");

            migrationBuilder.DropIndex(
                name: "IX_Schools_MinistryNo",
                table: "Schools");

            migrationBuilder.DropIndex(
                name: "IX_Reports_IsSignedByPrinciple",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReportNo",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_EduProgramUploads_EduProgramId",
                table: "EduProgramUploads");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "EvaluationBase",
                table: "EducationalYears");

            migrationBuilder.AlterColumn<string>(
                name: "SigningPrincipleFullName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SchoolName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerFullName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EduProgramName",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPositionTitle_UserForeignKey",
                table: "UserPositionTitle",
                column: "UserForeignKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schools_MinistryNo",
                table: "Schools",
                column: "MinistryNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportNo",
                table: "Reports",
                column: "ReportNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_IsSignedByPrinciple_SchoolMinistryNo_SchoolName_SigningPrincipleFullName_OwnerFullName_EduProgramName",
                table: "Reports",
                columns: new[] { "IsSignedByPrinciple", "SchoolMinistryNo", "SchoolName", "SigningPrincipleFullName", "OwnerFullName", "EduProgramName" });

            migrationBuilder.CreateIndex(
                name: "IX_EduProgramUploads_EduProgramId",
                table: "EduProgramUploads",
                column: "EduProgramId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);
        }
    }
}
