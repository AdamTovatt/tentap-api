using Microsoft.EntityFrameworkCore.Migrations;

namespace TentaPApi.Migrations
{
    public partial class _202201101438 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExerciseImageUrl",
                table: "Exercise",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SolutionImageUrl",
                table: "Exercise",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExerciseImageUrl",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "SolutionImageUrl",
                table: "Exercise");
        }
    }
}
