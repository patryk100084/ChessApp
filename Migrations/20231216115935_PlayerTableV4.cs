using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessApp.Migrations
{
    public partial class PlayerTableV4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    wins = table.Column<int>(type: "int", nullable: false),
                    draws = table.Column<int>(type: "int", nullable: false),
                    losses = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_players_username",
                table: "players",
                column: "username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "players");
        }
    }
}
