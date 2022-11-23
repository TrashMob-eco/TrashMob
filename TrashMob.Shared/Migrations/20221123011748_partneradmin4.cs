using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class partneradmin4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PartnerRequestStatus",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[] { 4, "Invitiation is pending approval by TrshMob.eco admin", 4, true, "Pending Approval" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PartnerRequestStatus",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
