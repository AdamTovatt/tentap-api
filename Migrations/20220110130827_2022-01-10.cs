using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TentaPApi.Migrations
{
    public partial class _20220110 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_ExerciseImage_ImageId",
                table: "Exercise");

            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_ExerciseImage_SolutionImageId",
                table: "Exercise");

            migrationBuilder.DropTable(
                name: "ExerciseImage");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_ImageId",
                table: "Exercise");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_SolutionImageId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "SolutionImageId",
                table: "Exercise");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Exercise",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SolutionImageId",
                table: "Exercise",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExerciseImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseImage", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_ImageId",
                table: "Exercise",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_SolutionImageId",
                table: "Exercise",
                column: "SolutionImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_ExerciseImage_ImageId",
                table: "Exercise",
                column: "ImageId",
                principalTable: "ExerciseImage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_ExerciseImage_SolutionImageId",
                table: "Exercise",
                column: "SolutionImageId",
                principalTable: "ExerciseImage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
