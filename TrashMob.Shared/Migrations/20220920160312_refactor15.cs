using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class refactor15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerContacts_Partner",
                table: "PartnerContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerContacts_Partners_PartnerId1",
                table: "PartnerContacts");

            migrationBuilder.DropIndex(
                name: "IX_PartnerContacts_PartnerId1",
                table: "PartnerContacts");

            migrationBuilder.DropColumn(
                name: "PartnerId1",
                table: "PartnerContacts");

            migrationBuilder.AlterColumn<Guid>(
                name: "PartnerId",
                table: "PartnerContacts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerContacts_Partners_PartnerId",
                table: "PartnerContacts",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerContacts_Partners_PartnerId",
                table: "PartnerContacts");

            migrationBuilder.AlterColumn<Guid>(
                name: "PartnerId",
                table: "PartnerContacts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId1",
                table: "PartnerContacts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartnerContacts_PartnerId1",
                table: "PartnerContacts",
                column: "PartnerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerContacts_Partner",
                table: "PartnerContacts",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerContacts_Partners_PartnerId1",
                table: "PartnerContacts",
                column: "PartnerId1",
                principalTable: "Partners",
                principalColumn: "Id");
        }
    }
}
