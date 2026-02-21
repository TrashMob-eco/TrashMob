using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoModeration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InReview",
                table: "TeamPhotos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModeratedByUserId",
                table: "TeamPhotos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModeratedDate",
                table: "TeamPhotos",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModerationReason",
                table: "TeamPhotos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModerationStatus",
                table: "TeamPhotos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewRequestedByUserId",
                table: "TeamPhotos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReviewRequestedDate",
                table: "TeamPhotos",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InReview",
                table: "LitterImages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModeratedByUserId",
                table: "LitterImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModeratedDate",
                table: "LitterImages",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModerationReason",
                table: "LitterImages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModerationStatus",
                table: "LitterImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewRequestedByUserId",
                table: "LitterImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReviewRequestedDate",
                table: "LitterImages",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PhotoFlags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhotoType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FlaggedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlagReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FlaggedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ResolvedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ResolvedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Resolution = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoFlags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoFlag_FlaggedByUser",
                        column: x => x.FlaggedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhotoFlag_ResolvedByUser",
                        column: x => x.ResolvedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhotoFlag_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhotoFlag_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PhotoModerationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhotoType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PerformedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerformedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoModerationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoModerationLog_PerformedByUser",
                        column: x => x.PerformedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhotoModerationLog_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhotoModerationLog_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamPhotos_ModeratedByUserId",
                table: "TeamPhotos",
                column: "ModeratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPhotos_ReviewRequestedByUserId",
                table: "TeamPhotos",
                column: "ReviewRequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterImages_ModeratedByUserId",
                table: "LitterImages",
                column: "ModeratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterImages_ReviewRequestedByUserId",
                table: "LitterImages",
                column: "ReviewRequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoFlags_CreatedByUserId",
                table: "PhotoFlags",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoFlags_FlaggedByUserId",
                table: "PhotoFlags",
                column: "FlaggedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoFlags_LastUpdatedByUserId",
                table: "PhotoFlags",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoFlags_ResolvedByUserId",
                table: "PhotoFlags",
                column: "ResolvedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoModerationLogs_CreatedByUserId",
                table: "PhotoModerationLogs",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoModerationLogs_LastUpdatedByUserId",
                table: "PhotoModerationLogs",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoModerationLogs_PerformedByUserId",
                table: "PhotoModerationLogs",
                column: "PerformedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LitterImage_ModeratedByUser",
                table: "LitterImages",
                column: "ModeratedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LitterImage_ReviewRequestedByUser",
                table: "LitterImages",
                column: "ReviewRequestedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamPhotos_ModeratedByUser",
                table: "TeamPhotos",
                column: "ModeratedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamPhotos_ReviewRequestedByUser",
                table: "TeamPhotos",
                column: "ReviewRequestedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LitterImage_ModeratedByUser",
                table: "LitterImages");

            migrationBuilder.DropForeignKey(
                name: "FK_LitterImage_ReviewRequestedByUser",
                table: "LitterImages");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamPhotos_ModeratedByUser",
                table: "TeamPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamPhotos_ReviewRequestedByUser",
                table: "TeamPhotos");

            migrationBuilder.DropTable(
                name: "PhotoFlags");

            migrationBuilder.DropTable(
                name: "PhotoModerationLogs");

            migrationBuilder.DropIndex(
                name: "IX_TeamPhotos_ModeratedByUserId",
                table: "TeamPhotos");

            migrationBuilder.DropIndex(
                name: "IX_TeamPhotos_ReviewRequestedByUserId",
                table: "TeamPhotos");

            migrationBuilder.DropIndex(
                name: "IX_LitterImages_ModeratedByUserId",
                table: "LitterImages");

            migrationBuilder.DropIndex(
                name: "IX_LitterImages_ReviewRequestedByUserId",
                table: "LitterImages");

            migrationBuilder.DropColumn(
                name: "InReview",
                table: "TeamPhotos");

            migrationBuilder.DropColumn(
                name: "ModeratedByUserId",
                table: "TeamPhotos");

            migrationBuilder.DropColumn(
                name: "ModeratedDate",
                table: "TeamPhotos");

            migrationBuilder.DropColumn(
                name: "ModerationReason",
                table: "TeamPhotos");

            migrationBuilder.DropColumn(
                name: "ModerationStatus",
                table: "TeamPhotos");

            migrationBuilder.DropColumn(
                name: "ReviewRequestedByUserId",
                table: "TeamPhotos");

            migrationBuilder.DropColumn(
                name: "ReviewRequestedDate",
                table: "TeamPhotos");

            migrationBuilder.DropColumn(
                name: "InReview",
                table: "LitterImages");

            migrationBuilder.DropColumn(
                name: "ModeratedByUserId",
                table: "LitterImages");

            migrationBuilder.DropColumn(
                name: "ModeratedDate",
                table: "LitterImages");

            migrationBuilder.DropColumn(
                name: "ModerationReason",
                table: "LitterImages");

            migrationBuilder.DropColumn(
                name: "ModerationStatus",
                table: "LitterImages");

            migrationBuilder.DropColumn(
                name: "ReviewRequestedByUserId",
                table: "LitterImages");

            migrationBuilder.DropColumn(
                name: "ReviewRequestedDate",
                table: "LitterImages");
        }
    }
}
