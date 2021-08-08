using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrashMob.Migrations
{
    public partial class AddEventPartners : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventPartnerStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPartnerStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventPartners",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventPartnerStatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPartners", x => new { x.EventId, x.PartnerId, x.PartnerLocationId });
                    table.ForeignKey(
                        name: "FK_EventPartners_EventPartnerStatuses",
                        column: x => x.EventPartnerStatusId,
                        principalTable: "EventPartnerStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventPartners_Events",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventPartners_PartnerLocations",
                        column: x => x.PartnerLocationId,
                        principalTable: "PartnerLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventPartners_Partners",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventPartners_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventPartners_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "EventPartnerStatuses",
                columns: new[] { "Id", "Description", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 0, "Partner has not been contacted", 1, "None" },
                    { 1, "Request is awaiting processing by partner", 2, "Requested" },
                    { 2, "Request has been approved by partner", 3, "Accepted" },
                    { 3, "Request has been declined by partner", 4, "Declined" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventPartners_CreatedByUserId",
                table: "EventPartners",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPartners_EventPartnerStatusId",
                table: "EventPartners",
                column: "EventPartnerStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPartners_LastUpdatedByUserId",
                table: "EventPartners",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPartners_PartnerId",
                table: "EventPartners",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPartners_PartnerLocationId",
                table: "EventPartners",
                column: "PartnerLocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventPartners");

            migrationBuilder.DropTable(
                name: "EventPartnerStatuses");
        }
    }
}
