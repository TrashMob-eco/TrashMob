using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddDependentInvitationAndMinorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DependentId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMinor",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentUserId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DependentInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DependentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    InvitationStatusId = table.Column<int>(type: "int", nullable: false),
                    DateInvited = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiresDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateAccepted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AcceptedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DependentInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DependentInvitations_Dependent",
                        column: x => x.DependentId,
                        principalTable: "Dependents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DependentInvitations_InvitationStatus",
                        column: x => x.InvitationStatusId,
                        principalTable: "InvitationStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DependentInvitations_User_AcceptedBy",
                        column: x => x.AcceptedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DependentInvitations_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DependentInvitations_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DependentInvitations_User_Parent",
                        column: x => x.ParentUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000000"),
                columns: new[] { "DependentId", "ParentUserId" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_DependentInvitations_AcceptedByUserId",
                table: "DependentInvitations",
                column: "AcceptedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DependentInvitations_CreatedByUserId",
                table: "DependentInvitations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DependentInvitations_DependentId",
                table: "DependentInvitations",
                column: "DependentId");

            migrationBuilder.CreateIndex(
                name: "IX_DependentInvitations_Email_Status",
                table: "DependentInvitations",
                columns: new[] { "Email", "InvitationStatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_DependentInvitations_InvitationStatusId",
                table: "DependentInvitations",
                column: "InvitationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_DependentInvitations_LastUpdatedByUserId",
                table: "DependentInvitations",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DependentInvitations_ParentUserId",
                table: "DependentInvitations",
                column: "ParentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DependentInvitations_TokenHash",
                table: "DependentInvitations",
                column: "TokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DependentInvitations");

            migrationBuilder.DropColumn(
                name: "DependentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsMinor",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ParentUserId",
                table: "Users");
        }
    }
}
