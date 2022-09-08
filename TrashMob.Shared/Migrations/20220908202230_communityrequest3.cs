using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class communityrequest3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommunityContactTypes",
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
                    table.PrimaryKey("PK_CommunityContactTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommunityRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    StreetAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityRequests_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommunityStatuses",
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
                    table.PrimaryKey("PK_CommunityStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Community",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    City = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Region = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    CommunityStatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Community", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Communities_ApplicationUser_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Communities_ApplicationUser_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Communities_CommunityStatuses",
                        column: x => x.CommunityStatusId,
                        principalTable: "CommunityStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunityAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityAttachments_ApplicationUser_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityAttachments_ApplicationUser_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityAttachments_Community",
                        column: x => x.CommunityId,
                        principalTable: "Community",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommunityContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityContactTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityContacts_ApplicationUser_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityContacts_ApplicationUser_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityContacts_Community",
                        column: x => x.CommunityId,
                        principalTable: "Community",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityContacts_CommunityContactTypes",
                        column: x => x.CommunityContactTypeId,
                        principalTable: "CommunityContactTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommunityNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityNotes_ApplicationUser_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityNotes_ApplicationUser_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityNotes_Community",
                        column: x => x.CommunityId,
                        principalTable: "Community",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommunityUsers",
                columns: table => new
                {
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityUsers", x => new { x.CommunityId, x.UserId });
                    table.ForeignKey(
                        name: "FK_CommunityUser_Communities",
                        column: x => x.CommunityId,
                        principalTable: "Community",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityUser_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityUsers_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityUsers_LastUpdatedByUser_Id",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "CommunityContactTypes",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 0, "Contact is not available", 1, null, "None" },
                    { 1, "Contact is an official within the Community", 2, null, "Official" },
                    { 2, "Contact is TrashMobHeadquarters", 3, null, "TrashMobHQ" },
                    { 3, "Contact is a TrashMob Volunteer in the community", 4, null, "TrashMob Volunteer" },
                    { 4, "Contact is a TrashMob Partner in the community", 5, null, "TrashMob Partner" }
                });

            migrationBuilder.InsertData(
                table: "CommunityStatuses",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 0, "Community is not currently an active TrashMob community", 2, null, "Inactive" },
                    { 1, "Community is an active TrashMob community", 1, null, "Active" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Community_CommunityStatusId",
                table: "Community",
                column: "CommunityStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Community_CreatedByUserId",
                table: "Community",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Community_LastUpdatedByUserId",
                table: "Community",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityAttachments_CommunityId",
                table: "CommunityAttachments",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityAttachments_CreatedByUserId",
                table: "CommunityAttachments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityAttachments_LastUpdatedByUserId",
                table: "CommunityAttachments",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContacts_CommunityContactTypeId",
                table: "CommunityContacts",
                column: "CommunityContactTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContacts_CommunityId",
                table: "CommunityContacts",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContacts_CreatedByUserId",
                table: "CommunityContacts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContacts_LastUpdatedByUserId",
                table: "CommunityContacts",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityNotes_CommunityId",
                table: "CommunityNotes",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityNotes_CreatedByUserId",
                table: "CommunityNotes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityNotes_LastUpdatedByUserId",
                table: "CommunityNotes",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityRequest_CreatedByUserId",
                table: "CommunityRequest",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityUsers_CreatedByUserId",
                table: "CommunityUsers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityUsers_LastUpdatedByUserId",
                table: "CommunityUsers",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityUsers_UserId",
                table: "CommunityUsers",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunityAttachments");

            migrationBuilder.DropTable(
                name: "CommunityContacts");

            migrationBuilder.DropTable(
                name: "CommunityNotes");

            migrationBuilder.DropTable(
                name: "CommunityRequest");

            migrationBuilder.DropTable(
                name: "CommunityUsers");

            migrationBuilder.DropTable(
                name: "CommunityContactTypes");

            migrationBuilder.DropTable(
                name: "Community");

            migrationBuilder.DropTable(
                name: "CommunityStatuses");
        }
    }
}
