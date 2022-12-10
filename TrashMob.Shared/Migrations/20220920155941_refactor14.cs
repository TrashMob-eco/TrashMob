using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class refactor14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_PartnerRequestStatus",
                table: "Partners");

            migrationBuilder.DropTable(
                name: "CommunityAttachments");

            migrationBuilder.DropTable(
                name: "CommunityContacts");

            migrationBuilder.DropTable(
                name: "CommunityNotes");

            migrationBuilder.DropTable(
                name: "CommunityPartners");

            migrationBuilder.DropTable(
                name: "CommunityRequests");

            migrationBuilder.DropTable(
                name: "CommunitySocialMediaAccounts");

            migrationBuilder.DropTable(
                name: "CommunityUsers");

            migrationBuilder.DropTable(
                name: "CommunityContactTypes");

            migrationBuilder.DropTable(
                name: "SocialMediaAccounts");

            migrationBuilder.DropTable(
                name: "Communities");

            migrationBuilder.DropTable(
                name: "CommunityStatuses");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "PrimaryEmail",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "PrimaryPhone",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "SecondaryEmail",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "SecondaryPhone",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "PrimaryEmail",
                table: "PartnerRequests");

            migrationBuilder.DropColumn(
                name: "PrimaryPhone",
                table: "PartnerRequests");

            migrationBuilder.RenameColumn(
                name: "SecondaryPhone",
                table: "PartnerRequests",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "SecondaryEmail",
                table: "PartnerRequests",
                newName: "Email");

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId1",
                table: "PartnerUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartnerTypeId",
                table: "Partners",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "PartnerRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "PartnerRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "PartnerRequests",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "PartnerRequests",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "PartnerRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "PartnerRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "PartnerRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId1",
                table: "PartnerLocations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PartnerContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerContactTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    PartnerId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerContacts_Partner",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerContacts_Partners_PartnerId1",
                        column: x => x.PartnerId1,
                        principalTable: "Partners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerContacts_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerContacts_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PartnerDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    PartnerId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerDocuments_Partner",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerDocuments_Partners_PartnerId1",
                        column: x => x.PartnerId1,
                        principalTable: "Partners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerDocuments_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerDocuments_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PartnerNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    PartnerId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
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
                        name: "FK_PartnerNotes_Partners_PartnerId1",
                        column: x => x.PartnerId1,
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

            migrationBuilder.CreateTable(
                name: "PartnerSocialMediaAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountIdentifier = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    SocialMediaAccountTypeId = table.Column<int>(type: "int", nullable: false),
                    PartnerId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerSocialMediaAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerSocialMediaAccount_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerSocialMediaAccount_LastUpdatedByUser_Id",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerSocialMediaAccount_Partner",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerSocialMediaAccounts_Partners_PartnerId1",
                        column: x => x.PartnerId1,
                        principalTable: "Partners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerSocialMediaAccounts_SocialMediaAccountType",
                        column: x => x.SocialMediaAccountTypeId,
                        principalTable: "SocialMediaAccountTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PartnerType",
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
                    table.PrimaryKey("PK_PartnerType", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PartnerType",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[] { 1, "Partner is a Government or Government Agency", 1, true, "Government" });

            migrationBuilder.InsertData(
                table: "PartnerType",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[] { 2, "Partner is Business", 2, true, "Business" });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerUsers_PartnerId1",
                table: "PartnerUsers",
                column: "PartnerId1");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_PartnerTypeId",
                table: "Partners",
                column: "PartnerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLocations_PartnerId1",
                table: "PartnerLocations",
                column: "PartnerId1");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerContacts_CreatedByUserId",
                table: "PartnerContacts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerContacts_LastUpdatedByUserId",
                table: "PartnerContacts",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerContacts_PartnerId",
                table: "PartnerContacts",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerContacts_PartnerId1",
                table: "PartnerContacts",
                column: "PartnerId1");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerDocuments_CreatedByUserId",
                table: "PartnerDocuments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerDocuments_LastUpdatedByUserId",
                table: "PartnerDocuments",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerDocuments_PartnerId",
                table: "PartnerDocuments",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerDocuments_PartnerId1",
                table: "PartnerDocuments",
                column: "PartnerId1");

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

            migrationBuilder.CreateIndex(
                name: "IX_PartnerNotes_PartnerId1",
                table: "PartnerNotes",
                column: "PartnerId1");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerSocialMediaAccounts_CreatedByUserId",
                table: "PartnerSocialMediaAccounts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerSocialMediaAccounts_LastUpdatedByUserId",
                table: "PartnerSocialMediaAccounts",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerSocialMediaAccounts_PartnerId",
                table: "PartnerSocialMediaAccounts",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerSocialMediaAccounts_PartnerId1",
                table: "PartnerSocialMediaAccounts",
                column: "PartnerId1");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerSocialMediaAccounts_SocialMediaAccountTypeId",
                table: "PartnerSocialMediaAccounts",
                column: "SocialMediaAccountTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerLocations_Partners_PartnerId1",
                table: "PartnerLocations",
                column: "PartnerId1",
                principalTable: "Partners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_PartnerStatus",
                table: "Partners",
                column: "PartnerStatusId",
                principalTable: "PartnerStatus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_PartnerType",
                table: "Partners",
                column: "PartnerTypeId",
                principalTable: "PartnerType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerUsers_Partners_PartnerId1",
                table: "PartnerUsers",
                column: "PartnerId1",
                principalTable: "Partners",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerLocations_Partners_PartnerId1",
                table: "PartnerLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Partners_PartnerStatus",
                table: "Partners");

            migrationBuilder.DropForeignKey(
                name: "FK_Partners_PartnerType",
                table: "Partners");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerUsers_Partners_PartnerId1",
                table: "PartnerUsers");

            migrationBuilder.DropTable(
                name: "PartnerContacts");

            migrationBuilder.DropTable(
                name: "PartnerDocuments");

            migrationBuilder.DropTable(
                name: "PartnerNotes");

            migrationBuilder.DropTable(
                name: "PartnerSocialMediaAccounts");

            migrationBuilder.DropTable(
                name: "PartnerType");

            migrationBuilder.DropIndex(
                name: "IX_PartnerUsers_PartnerId1",
                table: "PartnerUsers");

            migrationBuilder.DropIndex(
                name: "IX_Partners_PartnerTypeId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_PartnerLocations_PartnerId1",
                table: "PartnerLocations");

            migrationBuilder.DropColumn(
                name: "PartnerId1",
                table: "PartnerUsers");

            migrationBuilder.DropColumn(
                name: "PartnerTypeId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "City",
                table: "PartnerRequests");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "PartnerRequests");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "PartnerRequests");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "PartnerRequests");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "PartnerRequests");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "PartnerRequests");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "PartnerRequests");

            migrationBuilder.DropColumn(
                name: "PartnerId1",
                table: "PartnerLocations");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "PartnerRequests",
                newName: "SecondaryPhone");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "PartnerRequests",
                newName: "SecondaryEmail");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Partners",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryEmail",
                table: "Partners",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryPhone",
                table: "Partners",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryEmail",
                table: "Partners",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryPhone",
                table: "Partners",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryEmail",
                table: "PartnerRequests",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryPhone",
                table: "PartnerRequests",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CommunityContactTypes",
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
                    table.PrimaryKey("PK_CommunityContactTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommunityRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityRequests_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityRequests_Users_LastUpdatedByUserId",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunityStatuses",
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
                    table.PrimaryKey("PK_CommunityStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocialMediaAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SocialMediaAccountTypeId = table.Column<int>(type: "int", nullable: false),
                    AccountIdentifier = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialMediaAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialMediaAccount_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SocialMediaAccount_LastUpdatedByUser_Id",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SocialMediaAccounts_SocialMediaAccountTypes_SocialMediaAccountTypeId",
                        column: x => x.SocialMediaAccountTypeId,
                        principalTable: "SocialMediaAccountTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Communities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityStatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    City = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Region = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communities", x => x.Id);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
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
                        principalTable: "Communities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommunityContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityContactTypeId = table.Column<int>(type: "int", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
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
                        principalTable: "Communities",
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
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
                        principalTable: "Communities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommunityPartners",
                columns: table => new
                {
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityPartners", x => new { x.CommunityId, x.PartnerId, x.PartnerLocationId });
                    table.ForeignKey(
                        name: "FK_CommunityPartners_Communities",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityPartners_PartnerLocations",
                        column: x => x.PartnerLocationId,
                        principalTable: "PartnerLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityPartners_Partners",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityPartners_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityPartners_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommunitySocialMediaAccounts",
                columns: table => new
                {
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SocialMediaAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunitySocialMediaAccounts", x => new { x.CommunityId, x.SocialMediaAccountId });
                    table.ForeignKey(
                        name: "FK_CommunitySocialMediaAccount_Communities",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunitySocialMediaAccount_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunitySocialMediaAccount_LastUpdatedByUser_Id",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunitySocialMediaAccount_SocialMediaAccount",
                        column: x => x.SocialMediaAccountId,
                        principalTable: "SocialMediaAccounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommunityUsers",
                columns: table => new
                {
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityUsers", x => new { x.CommunityId, x.UserId });
                    table.ForeignKey(
                        name: "FK_CommunityUser_Communities",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
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
                    { 0, "Community is not currently an active TrashMob community", 2, true, "Inactive" },
                    { 1, "Community is an active TrashMob community", 1, true, "Active" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Communities_CommunityStatusId",
                table: "Communities",
                column: "CommunityStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_CreatedByUserId",
                table: "Communities",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_LastUpdatedByUserId",
                table: "Communities",
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
                name: "IX_CommunityPartners_CreatedByUserId",
                table: "CommunityPartners",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityPartners_LastUpdatedByUserId",
                table: "CommunityPartners",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityPartners_PartnerId",
                table: "CommunityPartners",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityPartners_PartnerLocationId",
                table: "CommunityPartners",
                column: "PartnerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityRequests_CreatedByUserId",
                table: "CommunityRequests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityRequests_LastUpdatedByUserId",
                table: "CommunityRequests",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunitySocialMediaAccounts_CreatedByUserId",
                table: "CommunitySocialMediaAccounts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunitySocialMediaAccounts_LastUpdatedByUserId",
                table: "CommunitySocialMediaAccounts",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunitySocialMediaAccounts_SocialMediaAccountId",
                table: "CommunitySocialMediaAccounts",
                column: "SocialMediaAccountId");

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

            migrationBuilder.CreateIndex(
                name: "IX_SocialMediaAccounts_CreatedByUserId",
                table: "SocialMediaAccounts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialMediaAccounts_LastUpdatedByUserId",
                table: "SocialMediaAccounts",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialMediaAccounts_SocialMediaAccountTypeId",
                table: "SocialMediaAccounts",
                column: "SocialMediaAccountTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_PartnerRequestStatus",
                table: "Partners",
                column: "PartnerStatusId",
                principalTable: "PartnerStatus",
                principalColumn: "Id");
        }
    }
}
