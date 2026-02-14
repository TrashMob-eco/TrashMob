using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddEventVisibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add new columns
            migrationBuilder.AddColumn<int>(
                name: "EventVisibilityId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "Events",
                type: "uniqueidentifier",
                nullable: true);

            // Step 2: Migrate data from IsEventPublic to EventVisibilityId
            // Public (IsEventPublic = 1) -> EventVisibilityId = 1 (Public)
            // Private (IsEventPublic = 0) -> EventVisibilityId = 3 (Private)
            migrationBuilder.Sql(
                "UPDATE [Events] SET [EventVisibilityId] = CASE WHEN [IsEventPublic] = 1 THEN 1 ELSE 3 END");

            // Step 3: Drop the old column
            migrationBuilder.DropColumn(
                name: "IsEventPublic",
                table: "Events");

            // Step 4: Add indexes and FK
            migrationBuilder.CreateIndex(
                name: "IX_Events_EventVisibilityId",
                table: "Events",
                column: "EventVisibilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_TeamId",
                table: "Events",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Teams_TeamId",
                table: "Events",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Teams_TeamId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_EventVisibilityId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_TeamId",
                table: "Events");

            // Re-add IsEventPublic before dropping EventVisibilityId
            migrationBuilder.AddColumn<bool>(
                name: "IsEventPublic",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Migrate data back: Public (1) -> true, everything else -> false
            migrationBuilder.Sql(
                "UPDATE [Events] SET [IsEventPublic] = CASE WHEN [EventVisibilityId] = 1 THEN 1 ELSE 0 END");

            migrationBuilder.DropColumn(
                name: "EventVisibilityId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Events");
        }
    }
}
