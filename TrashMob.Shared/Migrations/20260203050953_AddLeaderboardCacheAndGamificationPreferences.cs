using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaderboardCacheAndGamificationPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AchievementNotificationsEnabled",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnLeaderboards",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "LeaderboardCaches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LeaderboardType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TimeRange = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LocationScope = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LocationValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    ComputedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardCaches", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000000"),
                columns: new[] { "AchievementNotificationsEnabled", "ShowOnLeaderboards" },
                values: new object[] { true, true });

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardCache_EntityId",
                table: "LeaderboardCaches",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardCache_Lookup",
                table: "LeaderboardCaches",
                columns: new[] { "EntityType", "LeaderboardType", "TimeRange", "LocationScope", "LocationValue", "Rank" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaderboardCaches");

            migrationBuilder.DropColumn(
                name: "AchievementNotificationsEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ShowOnLeaderboards",
                table: "Users");
        }
    }
}
