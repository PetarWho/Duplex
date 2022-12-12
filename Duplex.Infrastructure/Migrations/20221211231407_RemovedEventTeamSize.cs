using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Duplex.Data.Migrations
{
    public partial class RemovedEventTeamSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamSize",
                table: "Events");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamSize",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
