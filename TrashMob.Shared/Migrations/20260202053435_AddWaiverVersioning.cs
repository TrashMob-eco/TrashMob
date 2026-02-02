using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddWaiverVersioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WaiverVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WaiverText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EffectiveDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaiverVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaiverVersions_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WaiverVersions_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommunityWaivers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WaiverVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityWaivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityWaivers_Partner",
                        column: x => x.CommunityId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityWaivers_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityWaivers_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityWaivers_WaiverVersion",
                        column: x => x.WaiverVersionId,
                        principalTable: "WaiverVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWaivers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WaiverVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcceptedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TypedLegalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    WaiverTextSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SigningMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DocumentUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    UploadedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsMinor = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    GuardianUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuardianName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GuardianRelationship = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWaivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWaivers_GuardianUser",
                        column: x => x.GuardianUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserWaivers_UploadedByUser",
                        column: x => x.UploadedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserWaivers_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWaivers_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserWaivers_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserWaivers_WaiverVersion",
                        column: x => x.WaiverVersionId,
                        principalTable: "WaiverVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommunityWaivers_CommunityId_WaiverVersionId",
                table: "CommunityWaivers",
                columns: new[] { "CommunityId", "WaiverVersionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommunityWaivers_CreatedByUserId",
                table: "CommunityWaivers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityWaivers_LastUpdatedByUserId",
                table: "CommunityWaivers",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityWaivers_WaiverVersionId",
                table: "CommunityWaivers",
                column: "WaiverVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWaivers_CreatedByUserId",
                table: "UserWaivers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWaivers_ExpiryDate",
                table: "UserWaivers",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserWaivers_GuardianUserId",
                table: "UserWaivers",
                column: "GuardianUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWaivers_LastUpdatedByUserId",
                table: "UserWaivers",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWaivers_UploadedByUserId",
                table: "UserWaivers",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWaivers_UserId",
                table: "UserWaivers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWaivers_UserId_WaiverVersionId",
                table: "UserWaivers",
                columns: new[] { "UserId", "WaiverVersionId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserWaivers_WaiverVersionId",
                table: "UserWaivers",
                column: "WaiverVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_WaiverVersions_CreatedByUserId",
                table: "WaiverVersions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WaiverVersions_LastUpdatedByUserId",
                table: "WaiverVersions",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WaiverVersions_Name_Version",
                table: "WaiverVersions",
                columns: new[] { "Name", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaiverVersions_Scope_IsActive",
                table: "WaiverVersions",
                columns: new[] { "Scope", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunityWaivers");

            migrationBuilder.DropTable(
                name: "UserWaivers");

            migrationBuilder.DropTable(
                name: "WaiverVersions");
        }
    }
}
