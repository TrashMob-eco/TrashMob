using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamAdoptionEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "AdoptionEndDate",
                table: "TeamAdoptions",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "AdoptionStartDate",
                table: "TeamAdoptions",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventCount",
                table: "TeamAdoptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompliant",
                table: "TeamAdoptions",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastEventDate",
                table: "TeamAdoptions",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TeamAdoptionEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamAdoptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamAdoptionEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamAdoptionEvents_Event",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeamAdoptionEvents_TeamAdoption",
                        column: x => x.TeamAdoptionId,
                        principalTable: "TeamAdoptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamAdoptionEvents_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeamAdoptionEvents_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamAdoptionEvents_CreatedByUserId",
                table: "TeamAdoptionEvents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAdoptionEvents_EventId",
                table: "TeamAdoptionEvents",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAdoptionEvents_LastUpdatedByUserId",
                table: "TeamAdoptionEvents",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAdoptionEvents_TeamAdoptionId_EventId",
                table: "TeamAdoptionEvents",
                columns: new[] { "TeamAdoptionId", "EventId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamAdoptionEvents");

            migrationBuilder.DropColumn(
                name: "AdoptionEndDate",
                table: "TeamAdoptions");

            migrationBuilder.DropColumn(
                name: "AdoptionStartDate",
                table: "TeamAdoptions");

            migrationBuilder.DropColumn(
                name: "EventCount",
                table: "TeamAdoptions");

            migrationBuilder.DropColumn(
                name: "IsCompliant",
                table: "TeamAdoptions");

            migrationBuilder.DropColumn(
                name: "LastEventDate",
                table: "TeamAdoptions");
        }
    }
}
