using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailInvites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailInviteBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TotalCount = table.Column<int>(type: "int", nullable: false),
                    SentCount = table.Column<int>(type: "int", nullable: false),
                    DeliveredCount = table.Column<int>(type: "int", nullable: false),
                    BouncedCount = table.Column<int>(type: "int", nullable: false),
                    FailedCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    CompletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailInviteBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailInviteBatches_Community",
                        column: x => x.CommunityId,
                        principalTable: "Partners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmailInviteBatches_SenderUser",
                        column: x => x.SenderUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmailInviteBatches_Team",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmailInviteBatches_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmailInviteBatches_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmailInvites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    SentDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeliveredDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SignedUpUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SignedUpDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailInvites_Batch",
                        column: x => x.BatchId,
                        principalTable: "EmailInviteBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailInvites_SignedUpUser",
                        column: x => x.SignedUpUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmailInvites_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmailInvites_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailInviteBatches_CommunityId",
                table: "EmailInviteBatches",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInviteBatches_CreatedByUserId",
                table: "EmailInviteBatches",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInviteBatches_CreatedDate",
                table: "EmailInviteBatches",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInviteBatches_LastUpdatedByUserId",
                table: "EmailInviteBatches",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInviteBatches_SenderUserId",
                table: "EmailInviteBatches",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInviteBatches_Status",
                table: "EmailInviteBatches",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInviteBatches_TeamId",
                table: "EmailInviteBatches",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInvites_BatchId",
                table: "EmailInvites",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInvites_CreatedByUserId",
                table: "EmailInvites",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInvites_Email",
                table: "EmailInvites",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInvites_LastUpdatedByUserId",
                table: "EmailInvites",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInvites_SignedUpUserId",
                table: "EmailInvites",
                column: "SignedUpUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInvites_Status",
                table: "EmailInvites",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailInvites");

            migrationBuilder.DropTable(
                name: "EmailInviteBatches");
        }
    }
}
