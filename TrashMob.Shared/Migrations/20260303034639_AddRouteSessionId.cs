using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteSessionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donations_Pledge",
                table: "Donations");

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "EventAttendeeRoutes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeRoutes_SessionId",
                table: "EventAttendeeRoutes",
                column: "SessionId",
                filter: "[SessionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_Pledge",
                table: "Donations",
                column: "PledgeId",
                principalTable: "Pledges",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donations_Pledge",
                table: "Donations");

            migrationBuilder.DropIndex(
                name: "IX_EventAttendeeRoutes_SessionId",
                table: "EventAttendeeRoutes");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "EventAttendeeRoutes");

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_Pledge",
                table: "Donations",
                column: "PledgeId",
                principalTable: "Pledges",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
