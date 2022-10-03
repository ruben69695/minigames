using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minigames.Infrastructure.Migrations
{
    public partial class UserGameDataMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserGameData",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TotalPoints = table.Column<int>(type: "integer", nullable: false),
                    Record = table.Column<int>(type: "integer", nullable: false),
                    LastGameDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGameData", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserGameData_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGameData");
        }
    }
}
