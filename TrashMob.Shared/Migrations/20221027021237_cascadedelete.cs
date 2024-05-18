#nullable disable
#pragma warning disable CS8981

namespace TrashMob.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class cascadedelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerContacts_Partner",
                table: "PartnerContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerDocuments_Partner",
                table: "PartnerDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerLocationContacts_PartnerLocation",
                table: "PartnerLocationContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerLocations_Partners",
                table: "PartnerLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnersLocationService_PartnerLocations",
                table: "PartnerLocationServices");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerSocialMediaAccount_Partner",
                table: "PartnerSocialMediaAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerUser_Partners",
                table: "PartnerUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerContacts_Partner",
                table: "PartnerContacts",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerDocuments_Partner",
                table: "PartnerDocuments",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerLocationContacts_PartnerLocation",
                table: "PartnerLocationContacts",
                column: "PartnerLocationId",
                principalTable: "PartnerLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerLocations_Partners",
                table: "PartnerLocations",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnersLocationService_PartnerLocations",
                table: "PartnerLocationServices",
                column: "PartnerLocationId",
                principalTable: "PartnerLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerSocialMediaAccount_Partner",
                table: "PartnerSocialMediaAccounts",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerUser_Partners",
                table: "PartnerUsers",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerContacts_Partner",
                table: "PartnerContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerDocuments_Partner",
                table: "PartnerDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerLocationContacts_PartnerLocation",
                table: "PartnerLocationContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerLocations_Partners",
                table: "PartnerLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnersLocationService_PartnerLocations",
                table: "PartnerLocationServices");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerSocialMediaAccount_Partner",
                table: "PartnerSocialMediaAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerUser_Partners",
                table: "PartnerUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerContacts_Partner",
                table: "PartnerContacts",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerDocuments_Partner",
                table: "PartnerDocuments",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerLocationContacts_PartnerLocation",
                table: "PartnerLocationContacts",
                column: "PartnerLocationId",
                principalTable: "PartnerLocations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerLocations_Partners",
                table: "PartnerLocations",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnersLocationService_PartnerLocations",
                table: "PartnerLocationServices",
                column: "PartnerLocationId",
                principalTable: "PartnerLocations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerSocialMediaAccount_Partner",
                table: "PartnerSocialMediaAccounts",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerUser_Partners",
                table: "PartnerUsers",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id");
        }
    }
}
