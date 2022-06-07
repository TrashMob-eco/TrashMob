using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class AddNonEventUserNotifications3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOptedOutOfAllEmails",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "NonEventUserNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserNotificationTypeId = table.Column<int>(type: "int", nullable: false),
                    SentDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonEventUserNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NonEventUserNotifications_User_Id",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NonEventUserNotifications_UserNotificationTypes",
                        column: x => x.UserNotificationTypeId,
                        principalTable: "UserNotificationTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "UserNotificationTypes",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[] { 18, "Opt out of Event Summary Week Reminder for events you have lead", 9, null, "EventSummaryHostWeekReminder" });

            migrationBuilder.CreateIndex(
                name: "IX_NonEventUserNotifications_UserId",
                table: "NonEventUserNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NonEventUserNotifications_UserNotificationTypeId",
                table: "NonEventUserNotifications",
                column: "UserNotificationTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NonEventUserNotifications");

            migrationBuilder.DeleteData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.AddColumn<bool>(
                name: "IsOptedOutOfAllEmails",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
