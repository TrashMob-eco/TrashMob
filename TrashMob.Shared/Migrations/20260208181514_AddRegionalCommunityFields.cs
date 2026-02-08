using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddRegionalCommunityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BoundsEast",
                table: "Partners",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoundsNorth",
                table: "Partners",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoundsSouth",
                table: "Partners",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoundsWest",
                table: "Partners",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountyName",
                table: "Partners",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionType",
                table: "Partners",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoundsEast",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "BoundsNorth",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "BoundsSouth",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "BoundsWest",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "CountyName",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "RegionType",
                table: "Partners");
        }
    }
}
