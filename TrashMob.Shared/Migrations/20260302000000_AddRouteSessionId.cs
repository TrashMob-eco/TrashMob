using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteSessionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "EventAttendeeRoutes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeRoutes_SessionId",
                table: "EventAttendeeRoutes",
                column: "SessionId",
                filter: "[SessionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventAttendeeRoutes_SessionId",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "EventAttendeeRoutes");
        }
    }
}
