using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class Prevent_Identical_EduProgramNames_Index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EduPrograms",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "Prevent_Identical_EduProgramNames",
                table: "EduPrograms",
                columns: new[] { "Name", "EducationalYearId" },
                unique: true,
                filter: "[Name] IS NOT NULL AND [EducationalYearId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Prevent_Identical_EduProgramNames",
                table: "EduPrograms");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EduPrograms",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
