using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TrashMob.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAndUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    GrantedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrantedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RevokedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RevokedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RevokedReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Role",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserRoles_User_GrantedBy",
                        column: x => x.GrantedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Full administrative access. Manages users, roles, waivers, events, moderation, and every other site-admin surface.", 1, true, "SiteAdmin" },
                    { 2, "Manages the municipal sales pipeline (prospects, contacts, activities) and reads the sales reports. Cannot administer users, waivers, or events.", 2, true, "SalesRep" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_CreatedByUserId",
                table: "UserRoles",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_GrantedByUserId",
                table: "UserRoles",
                column: "GrantedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_LastUpdatedByUserId",
                table: "UserRoles",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId_Active",
                table: "UserRoles",
                column: "RoleId",
                filter: "[RevokedDate] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId_Active",
                table: "UserRoles",
                column: "UserId",
                filter: "[RevokedDate] IS NULL");

            migrationBuilder.CreateIndex(
                name: "UX_UserRoles_Active",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                unique: true,
                filter: "[RevokedDate] IS NULL");

            // Project 64 Phase 1 backfill: grant every existing IsSiteAdmin=true user
            // the SiteAdmin role (id=1). GrantedByUserId points at the system user id
            // used elsewhere for automated writes. Preserves current behavior; the
            // compatibility bridge in IUserRoleService.HasRoleAsync also honours the
            // legacy boolean, so no read path breaks between deploy and this backfill.
            migrationBuilder.Sql(@"
                DECLARE @SystemUserId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001';
                DECLARE @Now DATETIMEOFFSET = SYSDATETIMEOFFSET();

                INSERT INTO UserRoles (Id, UserId, RoleId, GrantedByUserId, GrantedDate,
                                       CreatedByUserId, CreatedDate,
                                       LastUpdatedByUserId, LastUpdatedDate)
                SELECT NEWID(), u.Id, 1, @SystemUserId, @Now,
                       @SystemUserId, @Now,
                       @SystemUserId, @Now
                FROM Users u
                WHERE u.IsSiteAdmin = 1
                  AND NOT EXISTS (
                    SELECT 1 FROM UserRoles ur
                    WHERE ur.UserId = u.Id AND ur.RoleId = 1 AND ur.RevokedDate IS NULL
                  );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
