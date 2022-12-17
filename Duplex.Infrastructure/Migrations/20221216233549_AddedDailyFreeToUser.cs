using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Duplex.Data.Migrations
{
    public partial class AddedDailyFreeToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DailyAvailable",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DailyClaimedOnUtc",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyAvailable",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DailyClaimedOnUtc",
                table: "AspNetUsers");
        }
    }
}
