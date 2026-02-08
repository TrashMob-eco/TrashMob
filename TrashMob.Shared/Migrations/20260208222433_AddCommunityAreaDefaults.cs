using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunityAreaDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DefaultAllowCoAdoption",
                table: "Partners",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultCleanupFrequencyDays",
                table: "Partners",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultMinEventsPerYear",
                table: "Partners",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultSafetyRequirements",
                table: "Partners",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultAllowCoAdoption",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "DefaultCleanupFrequencyDays",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "DefaultMinEventsPerYear",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "DefaultSafetyRequirements",
                table: "Partners");
        }
    }
}
