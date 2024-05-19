#pragma warning disable CS8981
#pragma warning disable IDE1006
#pragma warning disable CA1861
#pragma warning disable IDE0300

namespace TrashMob.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PartnerUsers",
                table: "PartnerUsers");

            migrationBuilder.DropIndex(
                name: "IX_PartnerUsers_PartnerId",
                table: "PartnerUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventAttendees",
                table: "EventAttendees");

            migrationBuilder.DropIndex(
                name: "IX_EventAttendees_EventId",
                table: "EventAttendees");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartnerUsers",
                table: "PartnerUsers",
                columns: new[] { "PartnerId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventAttendees",
                table: "EventAttendees",
                columns: new[] { "EventId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerUsers_UserId",
                table: "PartnerUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendees_UserId",
                table: "EventAttendees",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PartnerUsers",
                table: "PartnerUsers");

            migrationBuilder.DropIndex(
                name: "IX_PartnerUsers_UserId",
                table: "PartnerUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventAttendees",
                table: "EventAttendees");

            migrationBuilder.DropIndex(
                name: "IX_EventAttendees_UserId",
                table: "EventAttendees");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartnerUsers",
                table: "PartnerUsers",
                columns: new[] { "UserId", "PartnerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventAttendees",
                table: "EventAttendees",
                columns: new[] { "UserId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerUsers_PartnerId",
                table: "PartnerUsers",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendees_EventId",
                table: "EventAttendees",
                column: "EventId");
        }
    }
}
