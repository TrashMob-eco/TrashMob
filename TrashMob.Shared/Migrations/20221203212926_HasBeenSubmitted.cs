using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class HasBeenSubmitted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasBeenSubmitted",
                table: "PickupLocations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasBeenSubmitted",
                table: "PickupLocations");
        }
    }
}
