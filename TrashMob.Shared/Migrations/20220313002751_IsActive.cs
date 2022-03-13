using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class IsActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "WaiverDurationTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "WaiverDurationTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "WaiverDurationTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "WaiverDurationTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsActive",
                value: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "WaiverDurationTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: null);

            migrationBuilder.UpdateData(
                table: "WaiverDurationTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsActive",
                value: null);

            migrationBuilder.UpdateData(
                table: "WaiverDurationTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsActive",
                value: null);

            migrationBuilder.UpdateData(
                table: "WaiverDurationTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsActive",
                value: null);
        }
    }
}
