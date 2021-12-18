using Microsoft.EntityFrameworkCore.Migrations;

namespace TrashMob.Migrations
{
    public partial class ChangeEmailNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Opt out of notifications for events happening soon you are attending", "UpcomingEventAttendingSoon" });

            migrationBuilder.UpdateData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Opt out of notifications for events happening soon you are leading", "UpcomingEventHostingSoon" });

            migrationBuilder.UpdateData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Opt out of notification for new events happening in your area soon", "UpcomingEventsInYourAreaSoon" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Opt out of notifications for events happening today you are attending", "UpcomingEventAttendingToday" });

            migrationBuilder.UpdateData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Opt out of notifications for events happening today you are leading", "UpcomingEventHostingToday" });

            migrationBuilder.UpdateData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Opt out of notification for new events happening in your area today", "UpcomingEventsInYourAreaToday" });
        }
    }
}
