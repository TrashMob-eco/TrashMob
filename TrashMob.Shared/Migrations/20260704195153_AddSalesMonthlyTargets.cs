using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesMonthlyTargets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesMonthlyTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<DateTime>(type: "date", nullable: false),
                    Metric = table.Column<int>(type: "int", nullable: false),
                    Target = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesMonthlyTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesMonthlyTargets_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesMonthlyTargets_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesMonthlyTargets_CreatedByUserId",
                table: "SalesMonthlyTargets",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesMonthlyTargets_LastUpdatedByUserId",
                table: "SalesMonthlyTargets",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesMonthlyTargets_Month",
                table: "SalesMonthlyTargets",
                column: "Month");

            migrationBuilder.CreateIndex(
                name: "UX_SalesMonthlyTargets_MonthMetric",
                table: "SalesMonthlyTargets",
                columns: new[] { "Month", "Metric" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesMonthlyTargets");
        }
    }
}
