using Microsoft.EntityFrameworkCore.Migrations;

namespace TentaPApi.Migrations
{
    public partial class _202201101512 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExerciseImageId",
                table: "Exercise",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SolutionImageId",
                table: "Exercise",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExerciseImageId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "SolutionImageId",
                table: "Exercise");
        }
    }
}
