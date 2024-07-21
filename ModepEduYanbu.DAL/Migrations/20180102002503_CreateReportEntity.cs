using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class CreateReportEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.CreateSequence<int>(
                name: "ReportNoSequence",
                schema: "shared",
                startValue: 2002L,
                incrementBy: 3);

            migrationBuilder.CreateTable(
                name: "Program",
                columns: table => new
                {
                    ProgramId = table.Column<string>(nullable: false),
                    BeginDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ReportDeadline = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Program", x => x.ProgramId);
                });

            migrationBuilder.CreateTable(
                name: "ReportResponses",
                columns: table => new
                {
                    ReportResponseId = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    OwnerFullName = table.Column<string>(nullable: true),
                    OwnerId = table.Column<string>(nullable: true),
                    OwnerIdNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportResponses", x => x.ReportResponseId);
                    table.ForeignKey(
                        name: "FK_ReportResponses_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ReportId = table.Column<string>(nullable: false),
                    AllowEdit = table.Column<bool>(nullable: false),
                    ChallengesSolus = table.Column<string>(nullable: true),
                    Evaluation = table.Column<double>(nullable: false),
                    EvaluationDate = table.Column<DateTime>(nullable: true),
                    EvaluatorFullName = table.Column<string>(nullable: true),
                    EvaluatorId = table.Column<string>(nullable: true),
                    EvaluatorIdNo = table.Column<string>(nullable: true),
                    ExecutionData = table.Column<string>(nullable: true),
                    ExecutionDate = table.Column<DateTime>(nullable: true),
                    ExecutionPeriod = table.Column<int>(nullable: false),
                    Field = table.Column<string>(nullable: true),
                    OwnerFullName = table.Column<string>(nullable: true),
                    OwnerId = table.Column<string>(nullable: true),
                    OwnerIdNo = table.Column<string>(nullable: true),
                    ParticipantsRatio = table.Column<double>(nullable: false),
                    PictureName = table.Column<string>(nullable: true),
                    ProgramId = table.Column<string>(nullable: true),
                    ProgramName = table.Column<string>(nullable: true),
                    ReportNo = table.Column<string>(nullable: true, defaultValueSql: "NEXT VALUE FOR shared.ReportNoSequence"),
                    SchoolName = table.Column<string>(nullable: true),
                    SentDateTime = table.Column<DateTime>(nullable: true),
                    SigningDateTime = table.Column<DateTime>(nullable: true),
                    SigningPrincipleFullName = table.Column<string>(nullable: true),
                    SigningPrincipleId = table.Column<string>(nullable: true),
                    SigningPrincipleIdNo = table.Column<string>(nullable: true),
                    TargetedCount = table.Column<int>(nullable: false),
                    TargetedSlice = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_Reports_AspNetUsers_EvaluatorId",
                        column: x => x.EvaluatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_Program_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Program",
                        principalColumn: "ProgramId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_AspNetUsers_SigningPrincipleId",
                        column: x => x.SigningPrincipleId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportUploads",
                columns: table => new
                {
                    ReportUploadedFileId = table.Column<string>(nullable: false),
                    Extension = table.Column<string>(nullable: true),
                    Filename = table.Column<string>(nullable: true),
                    ReportId = table.Column<string>(nullable: true),
                    UploadedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportUploads", x => x.ReportUploadedFileId);
                    table.ForeignKey(
                        name: "FK_ReportUploads_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "ReportId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_EvaluatorId",
                table: "Reports",
                column: "EvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_OwnerId",
                table: "Reports",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ProgramId",
                table: "Reports",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportNo",
                table: "Reports",
                column: "ReportNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SigningPrincipleId",
                table: "Reports",
                column: "SigningPrincipleId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportResponses_OwnerId",
                table: "ReportResponses",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportUploads_ReportId",
                table: "ReportUploads",
                column: "ReportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportResponses");

            migrationBuilder.DropTable(
                name: "ReportUploads");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Program");

            migrationBuilder.DropSequence(
                name: "ReportNoSequence",
                schema: "shared");
        }
    }
}
