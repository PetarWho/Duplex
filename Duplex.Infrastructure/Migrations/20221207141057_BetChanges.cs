using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Duplex.Data.Migrations
{
    public partial class BetChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bets_AspNetUsers_ApplicationUserId",
                table: "Bets");

            migrationBuilder.DropIndex(
                name: "IX_Bets_ApplicationUserId",
                table: "Bets");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Bets");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Bets");

            migrationBuilder.RenameColumn(
                name: "TotalPrize",
                table: "Bets",
                newName: "PrizePool");

            migrationBuilder.CreateTable(
                name: "UsersBets",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersBets", x => new { x.UserId, x.BetId });
                    table.ForeignKey(
                        name: "FK_UsersBets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersBets_Bets_BetId",
                        column: x => x.BetId,
                        principalTable: "Bets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersBets_BetId",
                table: "UsersBets",
                column: "BetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersBets");

            migrationBuilder.RenameColumn(
                name: "PrizePool",
                table: "Bets",
                newName: "TotalPrize");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Bets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Bets",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Bets_ApplicationUserId",
                table: "Bets",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bets_AspNetUsers_ApplicationUserId",
                table: "Bets",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
