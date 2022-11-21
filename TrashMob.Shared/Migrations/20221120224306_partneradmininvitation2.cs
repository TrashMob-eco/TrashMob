using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class partneradmininvitation2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartnerUsers");

            migrationBuilder.CreateTable(
                name: "InvitationStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvitationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartnerAdminInvitation",
                columns: table => new
                {
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InvitationStatusId = table.Column<int>(type: "int", nullable: false),
                    DateInvited = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerAdminInvitation", x => new { x.PartnerId, x.Email });
                    table.ForeignKey(
                        name: "FK_PartnerAdminInvitation_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerAdminInvitation_InvitationStatuses_InvitationStatusId",
                        column: x => x.InvitationStatusId,
                        principalTable: "InvitationStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartnerAdminInvitation_LastUpdatedByUser_Id",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerAdminInvitation_Partners",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartnerAdmins",
                columns: table => new
                {
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvitationStatusId = table.Column<int>(type: "int", nullable: false),
                    DateInvited = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerAdmins", x => new { x.PartnerId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PartnerAdmin_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerAdmin_LastUpdatedByUser_Id",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerAdmin_Partners",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartnerAdmin_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerAdmins_InvitationStatuses_InvitationStatusId",
                        column: x => x.InvitationStatusId,
                        principalTable: "InvitationStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "InvitationStatuses",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Invitation has not yet been sent", 1, true, "New" },
                    { 2, "Invitation has been sent", 2, true, "Sent" },
                    { 3, "Invitation has been accepted", 3, true, "Accepted" },
                    { 4, "Invitation has been canceled", 4, true, "Canceled" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerAdminInvitation_CreatedByUserId",
                table: "PartnerAdminInvitation",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerAdminInvitation_InvitationStatusId",
                table: "PartnerAdminInvitation",
                column: "InvitationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerAdminInvitation_LastUpdatedByUserId",
                table: "PartnerAdminInvitation",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerAdmins_CreatedByUserId",
                table: "PartnerAdmins",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerAdmins_InvitationStatusId",
                table: "PartnerAdmins",
                column: "InvitationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerAdmins_LastUpdatedByUserId",
                table: "PartnerAdmins",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerAdmins_UserId",
                table: "PartnerAdmins",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartnerAdminInvitation");

            migrationBuilder.DropTable(
                name: "PartnerAdmins");

            migrationBuilder.DropTable(
                name: "InvitationStatuses");

            migrationBuilder.CreateTable(
                name: "PartnerUsers",
                columns: table => new
                {
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerUsers", x => new { x.PartnerId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PartnerUser_Partners",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartnerUser_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerUsers_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerUsers_LastUpdatedByUser_Id",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerUsers_CreatedByUserId",
                table: "PartnerUsers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerUsers_LastUpdatedByUserId",
                table: "PartnerUsers",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerUsers_UserId",
                table: "PartnerUsers",
                column: "UserId");
        }
    }
}
