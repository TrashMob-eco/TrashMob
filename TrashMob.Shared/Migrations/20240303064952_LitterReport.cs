using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class LitterReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LitterReportStatuses",
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
                    table.PrimaryKey("PK_LitterReportStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LitterReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    LitterReportStatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LitterReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LitterReport_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LitterReport_LitterReportStatuses",
                        column: x => x.LitterReportStatusId,
                        principalTable: "LitterReportStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LitterReport_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventLitterReports",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitterReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLitterReports", x => new { x.EventId, x.LitterReportId });
                    table.ForeignKey(
                        name: "FK_EventLitterReport_Event",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventLitterReport_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventLitterReport_LitterReport",
                        column: x => x.LitterReportId,
                        principalTable: "LitterReports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventLitterReport_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitterImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitterReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AzureBlobURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LitterImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LitterImage_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LitterImage_LitterReports",
                        column: x => x.LitterReportId,
                        principalTable: "LitterReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LitterImage_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "LitterReportStatuses",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "New created", 1, true, "New" },
                    { 2, "Assigned To Event", 2, true, "Assigned" },
                    { 3, "Litter Cleaned", 3, true, "Cleaned" },
                    { 4, "Report Cancelled", 4, true, "Cancelled" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventLitterReports_CreatedByUserId",
                table: "EventLitterReports",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventLitterReports_LastUpdatedByUserId",
                table: "EventLitterReports",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventLitterReports_LitterReportId",
                table: "EventLitterReports",
                column: "LitterReportId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterImages_CreatedByUserId",
                table: "LitterImages",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterImages_LastUpdatedByUserId",
                table: "LitterImages",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterImages_LitterReportId",
                table: "LitterImages",
                column: "LitterReportId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterReports_CreatedByUserId",
                table: "LitterReports",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterReports_LastUpdatedByUserId",
                table: "LitterReports",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterReports_LitterReportStatusId",
                table: "LitterReports",
                column: "LitterReportStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventLitterReports");

            migrationBuilder.DropTable(
                name: "LitterImages");

            migrationBuilder.DropTable(
                name: "LitterReports");

            migrationBuilder.DropTable(
                name: "LitterReportStatuses");
        }
    }
}
