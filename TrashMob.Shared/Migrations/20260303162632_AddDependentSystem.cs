using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddDependentSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dependents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Relationship = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MedicalNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EmergencyContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dependents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dependents_User",
                        column: x => x.ParentUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Dependents_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Dependents_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DependentWaivers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DependentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WaiverVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SignedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypedLegalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WaiverTextSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcceptedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DocumentUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DependentWaivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DependentWaivers_Dependent",
                        column: x => x.DependentId,
                        principalTable: "Dependents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DependentWaivers_SignedByUser",
                        column: x => x.SignedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DependentWaivers_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DependentWaivers_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DependentWaivers_WaiverVersion",
                        column: x => x.WaiverVersionId,
                        principalTable: "WaiverVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventDependents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DependentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DependentWaiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventDependents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventDependents_Dependent",
                        column: x => x.DependentId,
                        principalTable: "Dependents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventDependents_DependentWaiver",
                        column: x => x.DependentWaiverId,
                        principalTable: "DependentWaivers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventDependents_Event",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventDependents_User",
                        column: x => x.ParentUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventDependents_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventDependents_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dependents_CreatedByUserId",
                table: "Dependents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Dependents_LastUpdatedByUserId",
                table: "Dependents",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Dependents_ParentUserId",
                table: "Dependents",
                column: "ParentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Dependents_ParentUserId_IsActive",
                table: "Dependents",
                columns: new[] { "ParentUserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_DependentWaivers_CreatedByUserId",
                table: "DependentWaivers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DependentWaivers_DependentId",
                table: "DependentWaivers",
                column: "DependentId");

            migrationBuilder.CreateIndex(
                name: "IX_DependentWaivers_DependentId_WaiverVersionId",
                table: "DependentWaivers",
                columns: new[] { "DependentId", "WaiverVersionId" });

            migrationBuilder.CreateIndex(
                name: "IX_DependentWaivers_ExpiryDate",
                table: "DependentWaivers",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_DependentWaivers_LastUpdatedByUserId",
                table: "DependentWaivers",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DependentWaivers_SignedByUserId",
                table: "DependentWaivers",
                column: "SignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DependentWaivers_WaiverVersionId",
                table: "DependentWaivers",
                column: "WaiverVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_EventDependents_CreatedByUserId",
                table: "EventDependents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventDependents_DependentId",
                table: "EventDependents",
                column: "DependentId");

            migrationBuilder.CreateIndex(
                name: "IX_EventDependents_DependentWaiverId",
                table: "EventDependents",
                column: "DependentWaiverId");

            migrationBuilder.CreateIndex(
                name: "IX_EventDependents_EventId_DependentId",
                table: "EventDependents",
                columns: new[] { "EventId", "DependentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventDependents_LastUpdatedByUserId",
                table: "EventDependents",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventDependents_ParentUserId",
                table: "EventDependents",
                column: "ParentUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventDependents");

            migrationBuilder.DropTable(
                name: "DependentWaivers");

            migrationBuilder.DropTable(
                name: "Dependents");
        }
    }
}
