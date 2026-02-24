using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteTimeTrimming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTimeTrimmed",
                table: "EventAttendeeRoutes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OriginalDurationMinutes",
                table: "EventAttendeeRoutes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "OriginalEndTime",
                table: "EventAttendeeRoutes",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginalTotalDistanceMeters",
                table: "EventAttendeeRoutes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTimeTrimmed",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropColumn(
                name: "OriginalDurationMinutes",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropColumn(
                name: "OriginalEndTime",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropColumn(
                name: "OriginalTotalDistanceMeters",
                table: "EventAttendeeRoutes");
        }
    }
}
