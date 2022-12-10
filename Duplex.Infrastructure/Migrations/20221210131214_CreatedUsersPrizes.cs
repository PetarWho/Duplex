using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Duplex.Data.Migrations
{
    public partial class CreatedUsersPrizes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prizes_AspNetUsers_ApplicationUserId",
                table: "Prizes");

            migrationBuilder.DropIndex(
                name: "IX_Prizes_ApplicationUserId",
                table: "Prizes");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Prizes");

            migrationBuilder.CreateTable(
                name: "UsersPrizes",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PrizeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersPrizes", x => new { x.UserId, x.PrizeId });
                    table.ForeignKey(
                        name: "FK_UsersPrizes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersPrizes_Prizes_PrizeId",
                        column: x => x.PrizeId,
                        principalTable: "Prizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersPrizes_PrizeId",
                table: "UsersPrizes",
                column: "PrizeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersPrizes");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Prizes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prizes_ApplicationUserId",
                table: "Prizes",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prizes_AspNetUsers_ApplicationUserId",
                table: "Prizes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
