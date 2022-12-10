using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class communityrequest7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreetAddress",
                table: "Communities");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdatedDate",
                table: "CommunityRequests",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommunityRequests_LastUpdatedByUserId",
                table: "CommunityRequests",
                column: "LastUpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityRequests_Users_LastUpdatedByUserId",
                table: "CommunityRequests",
                column: "LastUpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommunityRequests_Users_LastUpdatedByUserId",
                table: "CommunityRequests");

            migrationBuilder.DropIndex(
                name: "IX_CommunityRequests_LastUpdatedByUserId",
                table: "CommunityRequests");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "CommunityRequests");

            migrationBuilder.AddColumn<string>(
                name: "StreetAddress",
                table: "Communities",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
