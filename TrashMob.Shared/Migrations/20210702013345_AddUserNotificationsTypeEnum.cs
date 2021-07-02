using Microsoft.EntityFrameworkCore.Migrations;

namespace TrashMob.Migrations
{
    public partial class AddUserNotificationsTypeEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_UserNotificationPreferences_UserNotificationTypes", "UserNotificationPreferences");
            migrationBuilder.DropForeignKey("FK_UserNotifications_UserNotificationTypes", "UserNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserNotificationType",
                table: "UserNotificationType");

            migrationBuilder.RenameTable(
                name: "UserNotificationType",
                newName: "UserNotificationTypes");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UserNotificationTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserNotificationTypes",
                table: "UserNotificationTypes",
                column: "Id");

            migrationBuilder.InsertData(
                table: "UserNotificationTypes",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Opt out of Post Event Summary", 1, null, "EventSummaryAttendee" },
                    { 2, "Opt out of Event Summary Reminder for events you have lead", 2, null, "EventSummaryHostReminder" },
                    { 3, "Opt out of notifications for events upcoming this week you are attending", 3, null, "UpcomingEventAttendingThisWeek" },
                    { 4, "Opt out of notifications for events happening today you are attending", 4, null, "UpcomingEventAttendingToday" },
                    { 5, "Opt out of notifications for events upcoming this week you are leading", 5, null, "UpcomingEventHostingThisWeek" },
                    { 6, "Opt out of notifications for events happening today you are leading", 6, null, "UpcomingEventHostingToday" },
                    { 7, "Opt out of notification for new events upcoming in your area this week", 7, null, "UpcomingEventsInYourAreaThisWeek" },
                    { 8, "Opt out of notification for new events happening in your area today", 8, null, "UpcomingEventsInYourAreaToday" }
                });

            migrationBuilder.AddForeignKey("FK_UserNotifications_UserNotificationTypes", 
                                           "UserNotifications", "UserNotificationTypeId", 
                                           "UserNotificationTypes", 
                                           principalColumn: "Id",
                                           onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey("FK_UserNotificationPreferences_UserNotificationTypes",
                                           "UserNotificationPreferences", "UserNotificationTypeId",
                                           "UserNotificationTypes",
                                           principalColumn: "Id",
                                           onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserNotificationTypes",
                table: "UserNotificationTypes");

            migrationBuilder.DeleteData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.RenameTable(
                name: "UserNotificationTypes",
                newName: "UserNotificationType");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UserNotificationType",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserNotificationType",
                table: "UserNotificationType",
                column: "Id");
        }
    }
}
