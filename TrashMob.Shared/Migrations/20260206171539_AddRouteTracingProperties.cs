using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteTracingProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BagsCollected",
                table: "EventAttendeeRoutes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "EventAttendeeRoutes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpiresDate",
                table: "EventAttendeeRoutes",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTrimmed",
                table: "EventAttendeeRoutes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "EventAttendeeRoutes",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrivacyLevel",
                table: "EventAttendeeRoutes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValue: "EventOnly");

            migrationBuilder.AddColumn<int>(
                name: "TotalDistanceMeters",
                table: "EventAttendeeRoutes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrimEndMeters",
                table: "EventAttendeeRoutes",
                type: "int",
                nullable: false,
                defaultValue: 100);

            migrationBuilder.AddColumn<int>(
                name: "TrimStartMeters",
                table: "EventAttendeeRoutes",
                type: "int",
                nullable: false,
                defaultValue: 100);

            migrationBuilder.AddColumn<decimal>(
                name: "WeightCollected",
                table: "EventAttendeeRoutes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoutePoints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Altitude = table.Column<double>(type: "float", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Accuracy = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutePoints_EventAttendeeRoutes",
                        column: x => x.RouteId,
                        principalTable: "EventAttendeeRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoutePoints_RouteId",
                table: "RoutePoints",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutePoints_RouteId_Timestamp",
                table: "RoutePoints",
                columns: new[] { "RouteId", "Timestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoutePoints");

            migrationBuilder.DropColumn(
                name: "BagsCollected",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropColumn(
                name: "ExpiresDate",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropColumn(
                name: "IsTrimmed",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropColumn(
                name: "PrivacyLevel",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropColumn(
                name: "TotalDistanceMeters",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropColumn(
                name: "TrimEndMeters",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropColumn(
                name: "TrimStartMeters",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropColumn(
                name: "WeightCollected",
                table: "EventAttendeeRoutes");
        }
    }
}
