using Microsoft.EntityFrameworkCore.Migrations;

namespace TrashMob.Migrations
{
    public partial class AddEventDuration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationHours",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DurationHours",
                table: "EventHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "EventHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationHours",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "DurationHours",
                table: "EventHistory");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "EventHistory");
        }
    }
}
