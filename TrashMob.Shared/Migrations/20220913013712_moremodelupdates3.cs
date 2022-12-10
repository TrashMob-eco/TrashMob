using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class moremodelupdates3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunityAttachments");

            migrationBuilder.DropTable(
                name: "CommunityContacts");

            migrationBuilder.DropTable(
                name: "CommunityNotes");

            migrationBuilder.AddColumn<Guid>(
                name: "LastUpdatedByUserId",
                table: "EventMedias",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CommunityUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CommunityAttachmens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityAttachmens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityAttachments_ApplicationUser_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityAttachments_ApplicationUser_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityAttachments_Community",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommunityContacs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityContactTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityContacs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityContacts_ApplicationUser_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityContacts_ApplicationUser_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityContacts_Community",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityContacts_CommunityContactTypes",
                        column: x => x.CommunityContactTypeId,
                        principalTable: "CommunityContactTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommunityNots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityNots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityNotes_ApplicationUser_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityNotes_ApplicationUser_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityNotes_Community",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventMedias_LastUpdatedByUserId",
                table: "EventMedias",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityAttachmens_CommunityId",
                table: "CommunityAttachmens",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityAttachmens_CreatedByUserId",
                table: "CommunityAttachmens",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityAttachmens_LastUpdatedByUserId",
                table: "CommunityAttachmens",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContacs_CommunityContactTypeId",
                table: "CommunityContacs",
                column: "CommunityContactTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContacs_CommunityId",
                table: "CommunityContacs",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContacs_CreatedByUserId",
                table: "CommunityContacs",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContacs_LastUpdatedByUserId",
                table: "CommunityContacs",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityNots_CommunityId",
                table: "CommunityNots",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityNots_CreatedByUserId",
                table: "CommunityNots",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityNots_LastUpdatedByUserId",
                table: "CommunityNots",
                column: "LastUpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventMedias_Users_LastUpdatedByUserId",
                table: "EventMedias",
                column: "LastUpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventMedias_Users_LastUpdatedByUserId",
                table: "EventMedias");

            migrationBuilder.DropTable(
                name: "CommunityAttachmens");

            migrationBuilder.DropTable(
                name: "CommunityContacs");

            migrationBuilder.DropTable(
                name: "CommunityNots");

            migrationBuilder.DropIndex(
                name: "IX_EventMedias_LastUpdatedByUserId",
                table: "EventMedias");

            migrationBuilder.DropColumn(
                name: "LastUpdatedByUserId",
                table: "EventMedias");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CommunityUsers");

            migrationBuilder.CreateTable(
                name: "CommunityAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityAttachments_ApplicationUser_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityAttachments_ApplicationUser_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityAttachments_Community",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommunityContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CommunityContactTypeId = table.Column<int>(type: "int", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityContacts_ApplicationUser_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityContacts_ApplicationUser_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityContacts_Community",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityContacts_CommunityContactTypes",
                        column: x => x.CommunityContactTypeId,
                        principalTable: "CommunityContactTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommunityNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityNotes_ApplicationUser_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityNotes_ApplicationUser_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityNotes_Community",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommunityAttachments_CommunityId",
                table: "CommunityAttachments",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityAttachments_CreatedByUserId",
                table: "CommunityAttachments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityAttachments_LastUpdatedByUserId",
                table: "CommunityAttachments",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContacts_CommunityContactTypeId",
                table: "CommunityContacts",
                column: "CommunityContactTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContacts_CommunityId",
                table: "CommunityContacts",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContacts_CreatedByUserId",
                table: "CommunityContacts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContacts_LastUpdatedByUserId",
                table: "CommunityContacts",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityNotes_CommunityId",
                table: "CommunityNotes",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityNotes_CreatedByUserId",
                table: "CommunityNotes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityNotes_LastUpdatedByUserId",
                table: "CommunityNotes",
                column: "LastUpdatedByUserId");
        }
    }
}
