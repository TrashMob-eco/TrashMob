using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddPartnerPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartnerPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UploadedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UploadedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModerationStatus = table.Column<int>(type: "int", nullable: false),
                    InReview = table.Column<bool>(type: "bit", nullable: false),
                    ReviewRequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewRequestedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModeratedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModeratedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModerationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerPhotos_Partner",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartnerPhotos_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerPhotos_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerPhotos_ReviewRequestedByUser",
                        column: x => x.ReviewRequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerPhotos_ModeratedByUser",
                        column: x => x.ModeratedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerPhotos_UploadedByUser",
                        column: x => x.UploadedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPhotos_CreatedByUserId",
                table: "PartnerPhotos",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPhotos_LastUpdatedByUserId",
                table: "PartnerPhotos",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPhotos_PartnerId",
                table: "PartnerPhotos",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPhotos_ReviewRequestedByUserId",
                table: "PartnerPhotos",
                column: "ReviewRequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPhotos_ModeratedByUserId",
                table: "PartnerPhotos",
                column: "ModeratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPhotos_UploadedByUserId",
                table: "PartnerPhotos",
                column: "UploadedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartnerPhotos");
        }
    }
}
