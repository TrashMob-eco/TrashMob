using Microsoft.EntityFrameworkCore.Migrations;

namespace TrashMob.Migrations
{
    public partial class UpdatePartnerLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PartnerLocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PartnerLocations",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "PartnerLocations",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryEmail",
                table: "PartnerLocations",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryPhone",
                table: "PartnerLocations",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryEmail",
                table: "PartnerLocations",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryPhone",
                table: "PartnerLocations",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PartnerLocations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PartnerLocations");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "PartnerLocations");

            migrationBuilder.DropColumn(
                name: "PrimaryEmail",
                table: "PartnerLocations");

            migrationBuilder.DropColumn(
                name: "PrimaryPhone",
                table: "PartnerLocations");

            migrationBuilder.DropColumn(
                name: "SecondaryEmail",
                table: "PartnerLocations");

            migrationBuilder.DropColumn(
                name: "SecondaryPhone",
                table: "PartnerLocations");
        }
    }
}
