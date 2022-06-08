using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class AddMissingEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserNotificationTypes",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[] { 19, "Opt out of notifications for User Profile Location", 10, null, "UserProfileUpdateLocation" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserNotificationTypes",
                keyColumn: "Id",
                keyValue: 19);
        }
    }
}
