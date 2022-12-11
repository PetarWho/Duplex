using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Duplex.Data.Migrations
{
    public partial class AddedPUUIDToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PUUID",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PUUID",
                table: "AspNetUsers");
        }
    }
}
