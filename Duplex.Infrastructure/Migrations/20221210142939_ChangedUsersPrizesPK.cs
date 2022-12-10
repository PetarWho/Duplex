using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Duplex.Data.Migrations
{
    public partial class ChangedUsersPrizesPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersPrizes",
                table: "UsersPrizes");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UsersPrizes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersPrizes",
                table: "UsersPrizes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UsersPrizes_UserId",
                table: "UsersPrizes",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersPrizes",
                table: "UsersPrizes");

            migrationBuilder.DropIndex(
                name: "IX_UsersPrizes_UserId",
                table: "UsersPrizes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UsersPrizes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersPrizes",
                table: "UsersPrizes",
                columns: new[] { "UserId", "PrizeId" });
        }
    }
}
