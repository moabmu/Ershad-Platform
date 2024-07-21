using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class relationship_eduprogram_has_schoollevel_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EduProgramSchoolLevel",
                columns: table => new
                {
                    EduProgramId = table.Column<string>(nullable: false),
                    SchoolLevelId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EduProgramSchoolLevel", x => new { x.EduProgramId, x.SchoolLevelId });
                    table.ForeignKey(
                        name: "FK_EduProgramSchoolLevel_EduPrograms_EduProgramId",
                        column: x => x.EduProgramId,
                        principalTable: "EduPrograms",
                        principalColumn: "EduProgramId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EduProgramSchoolLevel_SchoolLevels_SchoolLevelId",
                        column: x => x.SchoolLevelId,
                        principalTable: "SchoolLevels",
                        principalColumn: "SchoollevelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EduProgramSchoolLevel_SchoolLevelId",
                table: "EduProgramSchoolLevel",
                column: "SchoolLevelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EduProgramSchoolLevel");
        }
    }
}
