using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class partnerLocationService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventPartnerLocations");

            migrationBuilder.DropTable(
                name: "EventPartnerLocationStatuses");

            migrationBuilder.CreateTable(
                name: "EventPartnerLocationServiceStatuses",
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
                    table.PrimaryKey("PK_EventPartnerLocationServiceStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventPartnerLocationServices",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    EventPartnerLocationServiceStatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPartnerLocationServices", x => new { x.EventId, x.PartnerLocationId });
                    table.ForeignKey(
                        name: "FK_EventPartnerLocationServices_EventPartnerLocationStatuses",
                        column: x => x.EventPartnerLocationServiceStatusId,
                        principalTable: "EventPartnerLocationServiceStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventPartnerLocationServices_Events",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventPartnerLocationServices_PartnerLocations",
                        column: x => x.PartnerLocationId,
                        principalTable: "PartnerLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventPartnerLocationServices_ServiceTypes",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventPartnerLocationServices_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventPartnerLocationServices_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "EventPartnerLocationServiceStatuses",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 0, "Partner has not been contacted", 1, true, "None" },
                    { 1, "Request is awaiting processing by partner", 2, true, "Requested" },
                    { 2, "Request has been approved by partner", 3, true, "Accepted" },
                    { 3, "Request has been declined by partner", 4, true, "Declined" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventPartnerLocationServices_CreatedByUserId",
                table: "EventPartnerLocationServices",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPartnerLocationServices_EventPartnerLocationServiceStatusId",
                table: "EventPartnerLocationServices",
                column: "EventPartnerLocationServiceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPartnerLocationServices_LastUpdatedByUserId",
                table: "EventPartnerLocationServices",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPartnerLocationServices_PartnerLocationId",
                table: "EventPartnerLocationServices",
                column: "PartnerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPartnerLocationServices_ServiceTypeId",
                table: "EventPartnerLocationServices",
                column: "ServiceTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventPartnerLocationServices");

            migrationBuilder.DropTable(
                name: "EventPartnerLocationServiceStatuses");

            migrationBuilder.CreateTable(
                name: "EventPartnerLocationStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPartnerLocationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventPartnerLocations",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventPartnerLocationStatusId = table.Column<int>(type: "int", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPartnerLocations", x => new { x.EventId, x.PartnerLocationId });
                    table.ForeignKey(
                        name: "FK_EventPartnerLocations_EventPartnerLocationStatuses",
                        column: x => x.EventPartnerLocationStatusId,
                        principalTable: "EventPartnerLocationStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventPartnerLocations_Events",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventPartnerLocations_PartnerLocations",
                        column: x => x.PartnerLocationId,
                        principalTable: "PartnerLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventPartnerLocations_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventPartnerLocations_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "EventPartnerLocationStatuses",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 0, "Partner has not been contacted", 1, true, "None" },
                    { 1, "Request is awaiting processing by partner", 2, true, "Requested" },
                    { 2, "Request has been approved by partner", 3, true, "Accepted" },
                    { 3, "Request has been declined by partner", 4, true, "Declined" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventPartnerLocations_CreatedByUserId",
                table: "EventPartnerLocations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPartnerLocations_EventPartnerLocationStatusId",
                table: "EventPartnerLocations",
                column: "EventPartnerLocationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPartnerLocations_LastUpdatedByUserId",
                table: "EventPartnerLocations",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPartnerLocations_PartnerLocationId",
                table: "EventPartnerLocations",
                column: "PartnerLocationId");
        }
    }
}
