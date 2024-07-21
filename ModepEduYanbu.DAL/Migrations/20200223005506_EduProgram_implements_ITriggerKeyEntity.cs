using Microsoft.EntityFrameworkCore.Migrations;

namespace ModepEduYanbu.Data.Migrations
{
    public partial class EduProgram_implements_ITriggerKeyEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuartzJob_CurrentTriggerKeyGroup",
                table: "EduPrograms",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuartzJob_CurrentTriggerKeyName",
                table: "EduPrograms",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuartzJob_CurrentTriggerKeyGroup",
                table: "EduPrograms");

            migrationBuilder.DropColumn(
                name: "QuartzJob_CurrentTriggerKeyName",
                table: "EduPrograms");
        }
    }
}
