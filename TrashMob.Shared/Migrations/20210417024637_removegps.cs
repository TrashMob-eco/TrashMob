#pragma warning disable CS8981
#pragma warning disable IDE1006

namespace TrashMob.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class removegps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GPSCoords",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "GPSCoords",
                table: "EventHistory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GPSCoords",
                table: "Events",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GPSCoords",
                table: "EventHistory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
