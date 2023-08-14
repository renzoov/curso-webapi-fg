using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeliculasAPI.Migrations
{
    /// <inheritdoc />
    public partial class Entitys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActorId",
                table: "PeliculasGeneros",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PeliculasGeneros_ActorId",
                table: "PeliculasGeneros",
                column: "ActorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PeliculasGeneros_Actores_ActorId",
                table: "PeliculasGeneros",
                column: "ActorId",
                principalTable: "Actores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeliculasGeneros_Actores_ActorId",
                table: "PeliculasGeneros");

            migrationBuilder.DropIndex(
                name: "IX_PeliculasGeneros_ActorId",
                table: "PeliculasGeneros");

            migrationBuilder.DropColumn(
                name: "ActorId",
                table: "PeliculasGeneros");
        }
    }
}
