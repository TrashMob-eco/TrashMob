#nullable disable
#pragma warning disable CS8981
#pragma warning disable IDE1006
#pragma warning disable CA1861
#pragma warning disable IDE0300

namespace TrashMob.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class eventtypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DisplayOrder",
                value: 3);

            migrationBuilder.UpdateData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "DisplayOrder",
                value: 4);

            migrationBuilder.UpdateData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "DisplayOrder",
                value: 5);

            migrationBuilder.UpdateData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "DisplayOrder",
                value: 6);

            migrationBuilder.UpdateData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "DisplayOrder",
                value: 14);

            migrationBuilder.UpdateData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 14,
                column: "DisplayOrder",
                value: 16);

            migrationBuilder.InsertData(
                table: "EventTypes",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 15, "Snow Removal", 15, true, "Snow Removal" },
                    { 16, "Streetside Cleanup", 2, true, "Streetside Cleanup" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.UpdateData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DisplayOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "DisplayOrder",
                value: 3);

            migrationBuilder.UpdateData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "DisplayOrder",
                value: 4);

            migrationBuilder.UpdateData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "DisplayOrder",
                value: 5);

            migrationBuilder.UpdateData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "DisplayOrder",
                value: 6);

            migrationBuilder.UpdateData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 14,
                column: "DisplayOrder",
                value: 14);
        }
    }
}
