using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class repopattern1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PartnerStatus",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PartnerRequestStatus",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "EventStatuses",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "EventPartnerStatuses",
                type: "bit",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CommunityStatuses",
                keyColumn: "Id",
                keyValue: 0,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "CommunityStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "EventPartnerStatuses",
                keyColumn: "Id",
                keyValue: 0,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "EventPartnerStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "EventPartnerStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "EventPartnerStatuses",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "EventStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "EventStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "EventStatuses",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "EventStatuses",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "PartnerRequestStatus",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "PartnerRequestStatus",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "PartnerRequestStatus",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "PartnerStatus",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "PartnerStatus",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsActive",
                value: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PartnerStatus");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PartnerRequestStatus");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "EventStatuses");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "EventPartnerStatuses");

            migrationBuilder.UpdateData(
                table: "CommunityStatuses",
                keyColumn: "Id",
                keyValue: 0,
                column: "IsActive",
                value: null);

            migrationBuilder.UpdateData(
                table: "CommunityStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: null);
        }
    }
}
