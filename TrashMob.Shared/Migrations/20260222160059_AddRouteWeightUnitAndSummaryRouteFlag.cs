using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteWeightUnitAndSummaryRouteFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFromRouteData",
                table: "EventSummaries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WeightUnitId",
                table: "EventAttendeeRoutes",
                type: "int",
                nullable: true);

            // Backfill existing routes that have weight data as pounds (WeightUnitId = 1)
            migrationBuilder.Sql(
                "UPDATE EventAttendeeRoutes SET WeightUnitId = 1 WHERE WeightCollected IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeRoutes_WeightUnitId",
                table: "EventAttendeeRoutes",
                column: "WeightUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendeeRoutes_WeightUnits",
                table: "EventAttendeeRoutes",
                column: "WeightUnitId",
                principalTable: "WeightUnits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendeeRoutes_WeightUnits",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropIndex(
                name: "IX_EventAttendeeRoutes_WeightUnitId",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropColumn(
                name: "IsFromRouteData",
                table: "EventSummaries");

            migrationBuilder.DropColumn(
                name: "WeightUnitId",
                table: "EventAttendeeRoutes");
        }
    }
}
