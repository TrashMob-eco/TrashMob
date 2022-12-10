using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class AddPartnerServices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartnerNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerRequests_PartnerType",
                table: "PartnerRequests"
                );

            migrationBuilder.DropForeignKey(
                name: "FK_Partners_PartnerType",
                table: "Partners"
                );

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartnerType",
                table: "PartnerType");

            migrationBuilder.RenameTable(
                name: "PartnerType",
                newName: "PartnerTypes");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "PartnerLocations",
                newName: "PublicNotes");

            migrationBuilder.AddColumn<string>(
                name: "PrivateNotes",
                table: "Partners",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicNotes",
                table: "Partners",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrivateNotes",
                table: "PartnerLocations",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartnerTypes",
                table: "PartnerTypes",
                column: "Id");

            migrationBuilder.AddForeignKey("FK_PartnerRequest_PartnerTypes",
                               "PartnerRequests", "PartnerTypeId",
                               "PartnerTypes",
                               principalColumn: "Id",
                               onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey("FK_Partner_PartnerTypes",
                               "Partners", "PartnerTypeId",
                               "PartnerTypes",
                               principalColumn: "Id",
                               onDelete: ReferentialAction.Restrict);

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
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
                    table.PrimaryKey("PK_ServiceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartnerLocationServices",
                columns: table => new
                {
                    PartnerLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerLocationServices", x => new { x.PartnerLocationId, x.ServiceTypeId });
                    table.ForeignKey(
                        name: "FK_PartnerLocationService_ServiceTypes",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerLocationServices_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerLocationServices_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnersLocationService_PartnerLocations",
                        column: x => x.PartnerLocationId,
                        principalTable: "PartnerLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnersLocationServices_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PartnerServices",
                columns: table => new
                {
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
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

            migrationBuilder.InsertData(
                table: "ServiceTypes",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Partner will haul litter away", 1, true, "Hauling" },
                    { 2, "Partner will dispose of litter", 2, true, "Disposal Location" },
                    { 3, "Partner distributes starter kits", 3, true, "Startup Kits" },
                    { 4, "Partner distributes supplies", 4, true, "Supplies" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLocationServices_CreatedByUserId",
                table: "PartnerLocationServices",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLocationServices_LastUpdatedByUserId",
                table: "PartnerLocationServices",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLocationServices_PartnerId",
                table: "PartnerLocationServices",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLocationServices_ServiceTypeId",
                table: "PartnerLocationServices",
                column: "ServiceTypeId");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartnerLocationServices");

            migrationBuilder.DropTable(
                name: "PartnerServices");

            migrationBuilder.DropTable(
                name: "ServiceTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartnerTypes",
                table: "PartnerTypes");

            migrationBuilder.DropColumn(
                name: "PrivateNotes",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "PublicNotes",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "PrivateNotes",
                table: "PartnerLocations");

            migrationBuilder.RenameTable(
                name: "PartnerTypes",
                newName: "PartnerType");

            migrationBuilder.RenameColumn(
                name: "PublicNotes",
                table: "PartnerLocations",
                newName: "Notes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartnerType",
                table: "PartnerType",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PartnerNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerNotes_Partner",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerNotes_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerNotes_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerNotes_CreatedByUserId",
                table: "PartnerNotes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerNotes_LastUpdatedByUserId",
                table: "PartnerNotes",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerNotes_PartnerId",
                table: "PartnerNotes",
                column: "PartnerId");
        }
    }
}
