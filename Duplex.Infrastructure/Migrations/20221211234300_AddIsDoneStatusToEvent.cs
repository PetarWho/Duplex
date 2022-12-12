using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Duplex.Data.Migrations
{
    public partial class AddIsDoneStatusToEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "EventsUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDone",
                table: "EventsUsers");
        }
    }
}
