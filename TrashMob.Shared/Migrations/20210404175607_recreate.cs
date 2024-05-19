#pragma warning disable CS8981
#pragma warning disable CA1861
#pragma warning disable IDE0300

namespace TrashMob.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class recreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    EventDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EventTypeId = table.Column<int>(type: "int", nullable: false),
                    EventStatusId = table.Column<int>(type: "int", nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    City = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    StateProvince = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    GPSCoords = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaxNumberOfParticipants = table.Column<int>(type: "int", nullable: true),
                    ActualNumberOfParticipants = table.Column<int>(type: "int", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "EventStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateAgreedToPrivacyPolicy = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PrivacyPolicyVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateAgreedToTermsOfService = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TermsOfServiceVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MemberSince = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    EventDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EventTypeId = table.Column<int>(type: "int", nullable: false),
                    EventStatusId = table.Column<int>(type: "int", nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    City = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    StateProvince = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    GPSCoords = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaxNumberOfParticipants = table.Column<int>(type: "int", nullable: true),
                    ActualNumberOfParticipants = table.Column<int>(type: "int", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_ApplicationUser_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_ApplicationUser_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_EventStatuses",
                        column: x => x.EventStatusId,
                        principalTable: "EventStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_EventTypes",
                        column: x => x.EventTypeId,
                        principalTable: "EventTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventAttendees",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SignUpDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CanceledDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAttendees", x => new { x.UserId, x.EventId });
                    table.ForeignKey(
                        name: "FK_EventAttendees_ApplicationUser",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventAttendees_Events",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "EventStatuses",
                columns: new[] { "Id", "Description", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, "Event is actively recruiting new members", 1, "Active" },
                    { 2, "Event is full", 2, "Full" },
                    { 3, "Event has been canceled", 3, "Canceled" },
                    { 4, "Event has completed", 4, "Completed" }
                });

            migrationBuilder.InsertData(
                table: "EventTypes",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 12, "Vandalism Cleanup", 12, true, "Vandalism Cleanup" },
                    { 11, "Waterway Cleanup", 11, true, "Waterway Cleanup" },
                    { 10, "Dog Park Cleanup", 10, true, "Dog Park Cleanup" },
                    { 9, "Private Land Cleanup", 9, true, "Private Land Cleanup" },
                    { 8, "Reef Cleanup", 8, true, "Reef Cleanup" },
                    { 7, "Trail Cleanup", 7, true, "Trail Cleanup" },
                    { 5, "Highway Cleanup", 5, true, "Highway Cleanup" },
                    { 13, "Social Event", 13, true, "Social Event" },
                    { 4, "Beach Cleanup", 4, true, "Beach Cleanup" },
                    { 3, "Neighborhood Cleanup", 3, true, "Neighborhood Cleanup" },
                    { 2, "School Cleanup", 2, true, "School Cleanup" },
                    { 1, "Park Cleanup", 1, true, "Park Cleanup" },
                    { 6, "Natural Disaster Cleanup", 6, true, "Natural Disaster Cleanup" },
                    { 14, "Other", 14, true, "Other" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendees_EventId",
                table: "EventAttendees",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreatedByUserId",
                table: "Events",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventStatusId",
                table: "Events",
                column: "EventStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventTypeId",
                table: "Events",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_LastUpdatedByUserId",
                table: "Events",
                column: "LastUpdatedByUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventAttendees");

            migrationBuilder.DropTable(
                name: "EventHistory");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "EventStatuses");

            migrationBuilder.DropTable(
                name: "EventTypes");
        }
    }
}
