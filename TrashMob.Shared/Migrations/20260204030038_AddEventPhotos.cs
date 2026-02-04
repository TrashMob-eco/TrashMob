using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddEventPhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UploadedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PhotoType = table.Column<int>(type: "int", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TakenAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UploadedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModerationStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    InReview = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReviewRequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewRequestedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModeratedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModeratedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModerationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventPhotos_Event",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventPhotos_ModeratedByUser",
                        column: x => x.ModeratedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventPhotos_ReviewRequestedByUser",
                        column: x => x.ReviewRequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventPhotos_UploadedByUser",
                        column: x => x.UploadedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventPhotos_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventPhotos_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventPhotos_CreatedByUserId",
                table: "EventPhotos",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPhotos_EventId",
                table: "EventPhotos",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPhotos_LastUpdatedByUserId",
                table: "EventPhotos",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPhotos_ModeratedByUserId",
                table: "EventPhotos",
                column: "ModeratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPhotos_ModerationStatus",
                table: "EventPhotos",
                column: "ModerationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_EventPhotos_PhotoType",
                table: "EventPhotos",
                column: "PhotoType");

            migrationBuilder.CreateIndex(
                name: "IX_EventPhotos_ReviewRequestedByUserId",
                table: "EventPhotos",
                column: "ReviewRequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPhotos_UploadedByUserId",
                table: "EventPhotos",
                column: "UploadedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventPhotos");
        }
    }
}
