using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class updatefields6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventHistory");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CommunityUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CommunityAttachments");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Communities");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "CommunityContacts",
                newName: "Phone");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Users",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastUpdatedByUserId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdatedDate",
                table: "Users",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "UserNotifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "UserNotifications",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastUpdatedByUserId",
                table: "UserNotifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdatedDate",
                table: "UserNotifications",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "SiteMetrics",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "SiteMetrics",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastUpdatedByUserId",
                table: "SiteMetrics",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdatedDate",
                table: "SiteMetrics",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "NonEventUserNotifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "NonEventUserNotifications",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastUpdatedByUserId",
                table: "NonEventUserNotifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdatedDate",
                table: "NonEventUserNotifications",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "MessageRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastUpdatedByUserId",
                table: "MessageRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdatedDate",
                table: "MessageRequests",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ContactRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastUpdatedByUserId",
                table: "ContactRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdatedDate",
                table: "ContactRequests",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "CommunityNotes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CommunityPartners",
                columns: table => new
                {
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                name: "SocialMediaAccountTypes",
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
                    table.PrimaryKey("PK_SocialMediaAccountTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocialMediaAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SocialMediaAccountTypeId = table.Column<int>(type: "int", nullable: false),
                    AccountIdentifier = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                name: "CommunitySocialMediaAccounts",
                columns: table => new
                {
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SocialMediaAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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

            migrationBuilder.InsertData(
                table: "SocialMediaAccountTypes",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Facebook", 1, null, "Facebook" },
                    { 2, "Twitter", 2, null, "Twitter" },
                    { 3, "Instagram", 3, null, "Instagram" },
                    { 4, "TikTok", 4, null, "TikTok" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedByUserId",
                table: "Users",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LastUpdatedByUserId",
                table: "Users",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_CreatedByUserId",
                table: "UserNotifications",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_LastUpdatedByUserId",
                table: "UserNotifications",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteMetrics_CreatedByUserId",
                table: "SiteMetrics",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteMetrics_LastUpdatedByUserId",
                table: "SiteMetrics",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_NonEventUserNotifications_CreatedByUserId",
                table: "NonEventUserNotifications",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_NonEventUserNotifications_LastUpdatedByUserId",
                table: "NonEventUserNotifications",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRequests_CreatedByUserId",
                table: "MessageRequests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRequests_LastUpdatedByUserId",
                table: "MessageRequests",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactRequests_CreatedByUserId",
                table: "ContactRequests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactRequests_LastUpdatedByUserId",
                table: "ContactRequests",
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
                name: "FK_ContactRequests_User_CreatedBy",
                table: "ContactRequests",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactRequests_User_LastUpdatedBy",
                table: "ContactRequests",
                column: "LastUpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageRequests_CreatedByUser_Id",
                table: "MessageRequests",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageRequests_LastUpdatedByUser_Id",
                table: "MessageRequests",
                column: "LastUpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NonEventUserNotification_CreatedByUser_Id",
                table: "NonEventUserNotifications",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NonEventUserNotification_LastUpdatedByUser_Id",
                table: "NonEventUserNotifications",
                column: "LastUpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SiteMetrics_CreatedByUser_Id",
                table: "SiteMetrics",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SiteMetrics_LastUpdatedByUser_Id",
                table: "SiteMetrics",
                column: "LastUpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotification_CreatedByUser_Id",
                table: "UserNotifications",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotification_LastUpdatedByUser_Id",
                table: "UserNotifications",
                column: "LastUpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_CreatedByUser_Id",
                table: "Users",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_LastUpdatedByUser_Id",
                table: "Users",
                column: "LastUpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactRequests_User_CreatedBy",
                table: "ContactRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactRequests_User_LastUpdatedBy",
                table: "ContactRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageRequests_CreatedByUser_Id",
                table: "MessageRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageRequests_LastUpdatedByUser_Id",
                table: "MessageRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_NonEventUserNotification_CreatedByUser_Id",
                table: "NonEventUserNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_NonEventUserNotification_LastUpdatedByUser_Id",
                table: "NonEventUserNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteMetrics_CreatedByUser_Id",
                table: "SiteMetrics");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteMetrics_LastUpdatedByUser_Id",
                table: "SiteMetrics");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotification_CreatedByUser_Id",
                table: "UserNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotification_LastUpdatedByUser_Id",
                table: "UserNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_User_CreatedByUser_Id",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_User_LastUpdatedByUser_Id",
                table: "Users");

            migrationBuilder.DropTable(
                name: "CommunityPartners");

            migrationBuilder.DropTable(
                name: "CommunitySocialMediaAccounts");

            migrationBuilder.DropTable(
                name: "SocialMediaAccounts");

            migrationBuilder.DropTable(
                name: "SocialMediaAccountTypes");

            migrationBuilder.DropIndex(
                name: "IX_Users_CreatedByUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_LastUpdatedByUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_UserNotifications_CreatedByUserId",
                table: "UserNotifications");

            migrationBuilder.DropIndex(
                name: "IX_UserNotifications_LastUpdatedByUserId",
                table: "UserNotifications");

            migrationBuilder.DropIndex(
                name: "IX_SiteMetrics_CreatedByUserId",
                table: "SiteMetrics");

            migrationBuilder.DropIndex(
                name: "IX_SiteMetrics_LastUpdatedByUserId",
                table: "SiteMetrics");

            migrationBuilder.DropIndex(
                name: "IX_NonEventUserNotifications_CreatedByUserId",
                table: "NonEventUserNotifications");

            migrationBuilder.DropIndex(
                name: "IX_NonEventUserNotifications_LastUpdatedByUserId",
                table: "NonEventUserNotifications");

            migrationBuilder.DropIndex(
                name: "IX_MessageRequests_CreatedByUserId",
                table: "MessageRequests");

            migrationBuilder.DropIndex(
                name: "IX_MessageRequests_LastUpdatedByUserId",
                table: "MessageRequests");

            migrationBuilder.DropIndex(
                name: "IX_ContactRequests_CreatedByUserId",
                table: "ContactRequests");

            migrationBuilder.DropIndex(
                name: "IX_ContactRequests_LastUpdatedByUserId",
                table: "ContactRequests");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastUpdatedByUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "LastUpdatedByUserId",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "SiteMetrics");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SiteMetrics");

            migrationBuilder.DropColumn(
                name: "LastUpdatedByUserId",
                table: "SiteMetrics");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "SiteMetrics");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "NonEventUserNotifications");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "NonEventUserNotifications");

            migrationBuilder.DropColumn(
                name: "LastUpdatedByUserId",
                table: "NonEventUserNotifications");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "NonEventUserNotifications");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "MessageRequests");

            migrationBuilder.DropColumn(
                name: "LastUpdatedByUserId",
                table: "MessageRequests");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "MessageRequests");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ContactRequests");

            migrationBuilder.DropColumn(
                name: "LastUpdatedByUserId",
                table: "ContactRequests");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "ContactRequests");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "CommunityNotes");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "CommunityContacts",
                newName: "PhoneNumber");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CommunityUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CommunityAttachments",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Communities",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EventHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CancellationReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    DurationHours = table.Column<int>(type: "int", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    EventDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventStatusId = table.Column<int>(type: "int", nullable: false),
                    EventTypeId = table.Column<int>(type: "int", nullable: false),
                    IsEventPublic = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    MaxNumberOfParticipants = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventHistory", x => x.Id);
                });
        }
    }
}
