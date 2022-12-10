using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class partneradmin3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerAdmins_InvitationStatuses_InvitationStatusId",
                table: "PartnerAdmins");

            migrationBuilder.DropIndex(
                name: "IX_PartnerAdmins_InvitationStatusId",
                table: "PartnerAdmins");

            migrationBuilder.DropColumn(
                name: "DateInvited",
                table: "PartnerAdmins");

            migrationBuilder.DropColumn(
                name: "InvitationStatusId",
                table: "PartnerAdmins");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateInvited",
                table: "PartnerAdmins",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "InvitationStatusId",
                table: "PartnerAdmins",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PartnerAdmins_InvitationStatusId",
                table: "PartnerAdmins",
                column: "InvitationStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerAdmins_InvitationStatuses_InvitationStatusId",
                table: "PartnerAdmins",
                column: "InvitationStatusId",
                principalTable: "InvitationStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
