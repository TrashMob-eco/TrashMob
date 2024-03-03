using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class EventAttendeeRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventAttendeeRoutes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserPath = table.Column<LineString>(type: "geography", nullable: true),
                    StartTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAttendeeRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventAttendeeRoutes_ApplicationUser",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventAttendeeRoutes_Events",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventAttendeeRoutes_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventAttendeeRoutes_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeRoutes_CreatedByUserId",
                table: "EventAttendeeRoutes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeRoutes_EventId",
                table: "EventAttendeeRoutes",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeRoutes_LastUpdatedByUserId",
                table: "EventAttendeeRoutes",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeRoutes_UserId",
                table: "EventAttendeeRoutes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventAttendeeRoutes");
        }
    }
}
