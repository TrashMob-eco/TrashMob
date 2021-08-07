using Microsoft.EntityFrameworkCore.Migrations;

namespace TrashMob.Migrations
{
    public partial class addMissingPartnerRequestStatus2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PartnerRequestStatus",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "PartnerRequestStatus",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Request has been approved by the Site Administrator", "Approved" });

            migrationBuilder.UpdateData(
                table: "PartnerRequestStatus",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Denied");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PartnerRequestStatus",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Request has been canceled by the requestor", "Canceled" });

            migrationBuilder.UpdateData(
                table: "PartnerRequestStatus",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Approved");

            migrationBuilder.InsertData(
                table: "PartnerRequestStatus",
                columns: new[] { "Id", "Description", "DisplayOrder", "Name" },
                values: new object[] { 4, "Request has been approved by the Site Administrator", 4, "Rejected" });
        }
    }
}
