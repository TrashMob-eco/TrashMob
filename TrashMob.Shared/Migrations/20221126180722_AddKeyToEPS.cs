using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class AddKeyToEPS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EventPartnerLocationServices",
                table: "EventPartnerLocationServices");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventPartnerLocationServices",
                table: "EventPartnerLocationServices",
                columns: new[] { "EventId", "PartnerLocationId", "ServiceTypeId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EventPartnerLocationServices",
                table: "EventPartnerLocationServices");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventPartnerLocationServices",
                table: "EventPartnerLocationServices",
                columns: new[] { "EventId", "PartnerLocationId" });
        }
    }
}
