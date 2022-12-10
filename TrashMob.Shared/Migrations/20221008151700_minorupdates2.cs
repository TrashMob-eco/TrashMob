using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class minorupdates2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "EventAttendees",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "EventAttendees",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastUpdatedByUserId",
                table: "EventAttendees",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdatedDate",
                table: "EventAttendees",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendees_CreatedByUserId",
                table: "EventAttendees",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendees_LastUpdatedByUserId",
                table: "EventAttendees",
                column: "LastUpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendees_User_CreatedBy",
                table: "EventAttendees",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendees_User_LastUpdatedBy",
                table: "EventAttendees",
                column: "LastUpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendees_User_CreatedBy",
                table: "EventAttendees");

            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendees_User_LastUpdatedBy",
                table: "EventAttendees");

            migrationBuilder.DropIndex(
                name: "IX_EventAttendees_CreatedByUserId",
                table: "EventAttendees");

            migrationBuilder.DropIndex(
                name: "IX_EventAttendees_LastUpdatedByUserId",
                table: "EventAttendees");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "EventAttendees");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "EventAttendees");

            migrationBuilder.DropColumn(
                name: "LastUpdatedByUserId",
                table: "EventAttendees");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "EventAttendees");
        }
    }
}
