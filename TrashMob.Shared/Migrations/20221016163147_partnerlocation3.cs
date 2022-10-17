using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class partnerlocation3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerLocationServices_Partners_PartnerId",
                table: "PartnerLocationServices");

            migrationBuilder.DropTable(
                name: "EventPartners");

            migrationBuilder.DropTable(
                name: "PartnerServices");

            migrationBuilder.DropTable(
                name: "EventPartnerStatuses");

            migrationBuilder.DropIndex(
                name: "IX_PartnerLocationServices_PartnerId",
                table: "PartnerLocationServices");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "StreetAddress",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "PartnerLocationServices");

            migrationBuilder.DropColumn(
                name: "PrimaryEmail",
                table: "PartnerLocations");

            migrationBuilder.DropColumn(
                name: "PrimaryPhone",
                table: "PartnerLocations");

            migrationBuilder.DropColumn(
                name: "SecondaryEmail",
                table: "PartnerLocations");

            migrationBuilder.DropColumn(
                name: "SecondaryPhone",
                table: "PartnerLocations");

            migrationBuilder.DropColumn(
                name: "PartnerContactTypeId",
                table: "PartnerContacts");

            migrationBuilder.CreateTable(
                name: "EventPartnerLocationStatuses",
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
                    table.PrimaryKey("PK_EventPartnerLocationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartnerLocationContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerLocationContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerLocationContacts_PartnerLocation",
                        column: x => x.PartnerLocationId,
                        principalTable: "PartnerLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerLocationContacts_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerLocationContacts_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventPartnerLocations",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventPartnerLocationStatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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

            migrationBuilder.UpdateData(
                table: "PartnerRequestStatus",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Invitiation has been sent", "Invitation Sent" });

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

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLocationContacts_CreatedByUserId",
                table: "PartnerLocationContacts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLocationContacts_LastUpdatedByUserId",
                table: "PartnerLocationContacts",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLocationContacts_PartnerLocationId",
                table: "PartnerLocationContacts",
                column: "PartnerLocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventPartnerLocations");

            migrationBuilder.DropTable(
                name: "PartnerLocationContacts");

            migrationBuilder.DropTable(
                name: "EventPartnerLocationStatuses");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Partners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Partners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Partners",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Partners",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Partners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Partners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetAddress",
                table: "Partners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId",
                table: "PartnerLocationServices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryEmail",
                table: "PartnerLocations",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryPhone",
                table: "PartnerLocations",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryEmail",
                table: "PartnerLocations",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryPhone",
                table: "PartnerLocations",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartnerContactTypeId",
                table: "PartnerContacts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EventPartnerStatuses",
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
                    table.PrimaryKey("PK_EventPartnerStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartnerServices",
                columns: table => new
                {
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerServices", x => new { x.PartnerId, x.ServiceTypeId });
                    table.ForeignKey(
                        name: "FK_PartnerServices_Partners",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerServices_ServiceTypes",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerServices_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerServices_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventPartners",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventPartnerStatusId = table.Column<int>(type: "int", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventPartners_PartnerLocations",
                        column: x => x.PartnerLocationId,
                        principalTable: "PartnerLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventPartners_Partners",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventPartners_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventPartners_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "EventPartnerStatuses",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 0, "Partner has not been contacted", 1, true, "None" },
                    { 1, "Request is awaiting processing by partner", 2, true, "Requested" },
                    { 2, "Request has been approved by partner", 3, true, "Accepted" },
                    { 3, "Request has been declined by partner", 4, true, "Declined" }
                });

            migrationBuilder.UpdateData(
                table: "PartnerRequestStatus",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Request has been sent", "Sent" });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLocationServices_PartnerId",
                table: "PartnerLocationServices",
                column: "PartnerId");

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

            migrationBuilder.CreateIndex(
                name: "IX_PartnerServices_CreatedByUserId",
                table: "PartnerServices",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerServices_LastUpdatedByUserId",
                table: "PartnerServices",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerServices_ServiceTypeId",
                table: "PartnerServices",
                column: "ServiceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerLocationServices_Partners_PartnerId",
                table: "PartnerLocationServices",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id");
        }
    }
}
