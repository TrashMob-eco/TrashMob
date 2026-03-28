using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddPrivoConsentManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "IdentityVerifiedDate",
                table: "Users",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsIdentityVerified",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PrivoSid",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentalConsentId",
                table: "Dependents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrivoSid",
                table: "Dependents",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ParentalConsents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DependentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PrivoConsentIdentifier = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PrivoSid = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PrivoGranterSid = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConsentType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ConsentUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    VerifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RevokedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RevokedReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentalConsents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParentalConsents_Dependent",
                        column: x => x.DependentId,
                        principalTable: "Dependents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ParentalConsents_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ParentalConsents_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ParentalConsents_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ParentalConsents_User_Parent",
                        column: x => x.ParentUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000000"),
                columns: new[] { "IdentityVerifiedDate", "PrivoSid" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Dependents_ParentalConsentId",
                table: "Dependents",
                column: "ParentalConsentId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentalConsents_CreatedByUserId",
                table: "ParentalConsents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentalConsents_DependentId",
                table: "ParentalConsents",
                column: "DependentId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentalConsents_LastUpdatedByUserId",
                table: "ParentalConsents",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentalConsents_ParentUserId",
                table: "ParentalConsents",
                column: "ParentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentalConsents_PrivoConsentIdentifier",
                table: "ParentalConsents",
                column: "PrivoConsentIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_ParentalConsents_PrivoSid",
                table: "ParentalConsents",
                column: "PrivoSid");

            migrationBuilder.CreateIndex(
                name: "IX_ParentalConsents_UserId",
                table: "ParentalConsents",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dependents_ParentalConsent",
                table: "Dependents",
                column: "ParentalConsentId",
                principalTable: "ParentalConsents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dependents_ParentalConsent",
                table: "Dependents");

            migrationBuilder.DropTable(
                name: "ParentalConsents");

            migrationBuilder.DropIndex(
                name: "IX_Dependents_ParentalConsentId",
                table: "Dependents");

            migrationBuilder.DropColumn(
                name: "IdentityVerifiedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsIdentityVerified",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PrivoSid",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ParentalConsentId",
                table: "Dependents");

            migrationBuilder.DropColumn(
                name: "PrivoSid",
                table: "Dependents");
        }
    }
}
