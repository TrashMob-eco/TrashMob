using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class refactor16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerContacts_Partners_PartnerId",
                table: "PartnerContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerDocuments_Partners_PartnerId1",
                table: "PartnerDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerLocations_Partners_PartnerId1",
                table: "PartnerLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerNotes_Partners_PartnerId1",
                table: "PartnerNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerSocialMediaAccounts_Partners_PartnerId1",
                table: "PartnerSocialMediaAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerUsers_Partners_PartnerId1",
                table: "PartnerUsers");

            migrationBuilder.DropIndex(
                name: "IX_PartnerUsers_PartnerId1",
                table: "PartnerUsers");

            migrationBuilder.DropIndex(
                name: "IX_PartnerSocialMediaAccounts_PartnerId1",
                table: "PartnerSocialMediaAccounts");

            migrationBuilder.DropIndex(
                name: "IX_PartnerNotes_PartnerId1",
                table: "PartnerNotes");

            migrationBuilder.DropIndex(
                name: "IX_PartnerLocations_PartnerId1",
                table: "PartnerLocations");

            migrationBuilder.DropIndex(
                name: "IX_PartnerDocuments_PartnerId1",
                table: "PartnerDocuments");

            migrationBuilder.DropColumn(
                name: "PartnerId1",
                table: "PartnerUsers");

            migrationBuilder.DropColumn(
                name: "PartnerId1",
                table: "PartnerSocialMediaAccounts");

            migrationBuilder.DropColumn(
                name: "PartnerId1",
                table: "PartnerNotes");

            migrationBuilder.DropColumn(
                name: "PartnerId1",
                table: "PartnerLocations");

            migrationBuilder.DropColumn(
                name: "PartnerId1",
                table: "PartnerDocuments");

            migrationBuilder.AlterColumn<Guid>(
                name: "PartnerId",
                table: "PartnerContacts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerContacts_Partner",
                table: "PartnerContacts",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerContacts_Partner",
                table: "PartnerContacts");

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId1",
                table: "PartnerUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId1",
                table: "PartnerSocialMediaAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId1",
                table: "PartnerNotes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId1",
                table: "PartnerLocations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId1",
                table: "PartnerDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PartnerId",
                table: "PartnerContacts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerUsers_PartnerId1",
                table: "PartnerUsers",
                column: "PartnerId1");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerSocialMediaAccounts_PartnerId1",
                table: "PartnerSocialMediaAccounts",
                column: "PartnerId1");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerNotes_PartnerId1",
                table: "PartnerNotes",
                column: "PartnerId1");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLocations_PartnerId1",
                table: "PartnerLocations",
                column: "PartnerId1");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerDocuments_PartnerId1",
                table: "PartnerDocuments",
                column: "PartnerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerContacts_Partners_PartnerId",
                table: "PartnerContacts",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerDocuments_Partners_PartnerId1",
                table: "PartnerDocuments",
                column: "PartnerId1",
                principalTable: "Partners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerLocations_Partners_PartnerId1",
                table: "PartnerLocations",
                column: "PartnerId1",
                principalTable: "Partners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerNotes_Partners_PartnerId1",
                table: "PartnerNotes",
                column: "PartnerId1",
                principalTable: "Partners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerSocialMediaAccounts_Partners_PartnerId1",
                table: "PartnerSocialMediaAccounts",
                column: "PartnerId1",
                principalTable: "Partners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerUsers_Partners_PartnerId1",
                table: "PartnerUsers",
                column: "PartnerId1",
                principalTable: "Partners",
                principalColumn: "Id");
        }
    }
}
