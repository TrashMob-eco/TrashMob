using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddEventAttendeeMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventAttendeeMetrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BagsCollected = table.Column<int>(type: "int", nullable: true),
                    PickedWeight = table.Column<decimal>(type: "decimal(10,1)", nullable: true),
                    PickedWeightUnitId = table.Column<int>(type: "int", nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AdjustedBagsCollected = table.Column<int>(type: "int", nullable: true),
                    AdjustedPickedWeight = table.Column<decimal>(type: "decimal(10,1)", nullable: true),
                    AdjustedPickedWeightUnitId = table.Column<int>(type: "int", nullable: true),
                    AdjustedDurationMinutes = table.Column<int>(type: "int", nullable: true),
                    AdjustmentReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAttendeeMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventAttendeeMetrics_AdjustedPickedWeightUnit",
                        column: x => x.AdjustedPickedWeightUnitId,
                        principalTable: "WeightUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventAttendeeMetrics_Event",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventAttendeeMetrics_PickedWeightUnit",
                        column: x => x.PickedWeightUnitId,
                        principalTable: "WeightUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventAttendeeMetrics_ReviewedByUser",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventAttendeeMetrics_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventAttendeeMetrics_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventAttendeeMetrics_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeMetrics_AdjustedPickedWeightUnitId",
                table: "EventAttendeeMetrics",
                column: "AdjustedPickedWeightUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeMetrics_CreatedByUserId",
                table: "EventAttendeeMetrics",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeMetrics_EventId_UserId",
                table: "EventAttendeeMetrics",
                columns: new[] { "EventId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeMetrics_LastUpdatedByUserId",
                table: "EventAttendeeMetrics",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeMetrics_PickedWeightUnitId",
                table: "EventAttendeeMetrics",
                column: "PickedWeightUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeMetrics_ReviewedByUserId",
                table: "EventAttendeeMetrics",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeMetrics_Status",
                table: "EventAttendeeMetrics",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeMetrics_UserId",
                table: "EventAttendeeMetrics",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventAttendeeMetrics");
        }
    }
}
