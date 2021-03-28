using Microsoft.EntityFrameworkCore.Migrations;

namespace TrashMob.Migrations
{
    public partial class BaseData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EventStatuses",
                columns: new[] { "Id", "Description", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, "Event is actively recruiting new members", 1, "Active" },
                    { 2, "Event is full", 2, "Full" },
                    { 3, "Event has been canceled", 3, "Canceled" },
                    { 4, "Event has completed", 4, "Completed" }
                });

            migrationBuilder.InsertData(
                table: "EventTypes",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 12, "Vandalism Cleanup", 12, true, "Vandalism Cleanup" },
                    { 11, "Waterway Cleanup", 11, true, "Waterway Cleanup" },
                    { 10, "Dog Park Cleanup", 10, true, "Dog Park Cleanup" },
                    { 9, "Private Land Cleanup", 9, true, "Private Land Cleanup" },
                    { 8, "Reef Cleanup", 8, true, "Reef Cleanup" },
                    { 7, "Trail Cleanup", 7, true, "Trail Cleanup" },
                    { 5, "Highway Cleanup", 5, true, "Highway Cleanup" },
                    { 13, "Social Event", 13, true, "Social Event" },
                    { 4, "Beach Cleanup", 4, true, "Beach Cleanup" },
                    { 3, "Neighborhood Cleanup", 3, true, "Neighborhood Cleanup" },
                    { 2, "School Cleanup", 2, true, "School Cleanup" },
                    { 1, "Park Cleanup", 1, true, "Park Cleanup" },
                    { 6, "Natural Disaster Cleanup", 6, true, "Natural Disaster Cleanup" },
                    { 14, "Other", 14, true, "Other" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventStatuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EventStatuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EventStatuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EventStatuses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 14);
        }
    }
}
