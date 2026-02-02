using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamAdoption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamAdoptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdoptableAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ApplicationNotes = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamAdoptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamAdoptions_AdoptableArea",
                        column: x => x.AdoptableAreaId,
                        principalTable: "AdoptableAreas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeamAdoptions_ReviewedByUser",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeamAdoptions_Team",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamAdoptions_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeamAdoptions_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamAdoptions_AdoptableAreaId",
                table: "TeamAdoptions",
                column: "AdoptableAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAdoptions_CreatedByUserId",
                table: "TeamAdoptions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAdoptions_LastUpdatedByUserId",
                table: "TeamAdoptions",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAdoptions_ReviewedByUserId",
                table: "TeamAdoptions",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAdoptions_Status",
                table: "TeamAdoptions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAdoptions_TeamId_AdoptableAreaId",
                table: "TeamAdoptions",
                columns: new[] { "TeamId", "AdoptableAreaId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamAdoptions");
        }
    }
}
