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
                name: "AdoptableAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    AreaType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Available"),
                    GeoJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartLatitude = table.Column<double>(type: "float", nullable: true),
                    StartLongitude = table.Column<double>(type: "float", nullable: true),
                    EndLatitude = table.Column<double>(type: "float", nullable: true),
                    EndLongitude = table.Column<double>(type: "float", nullable: true),
                    CleanupFrequencyDays = table.Column<int>(type: "int", nullable: false, defaultValue: 90),
                    MinEventsPerYear = table.Column<int>(type: "int", nullable: false, defaultValue: 4),
                    SafetyRequirements = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AllowCoAdoption = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptableAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdoptableAreas_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdoptableAreas_LastUpdatedByUser_Id",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdoptableAreas_Partners",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdoptableAreas_CreatedByUserId",
                table: "AdoptableAreas",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptableAreas_LastUpdatedByUserId",
                table: "AdoptableAreas",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptableAreas_PartnerId",
                table: "AdoptableAreas",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptableAreas_PartnerId_Name",
                table: "AdoptableAreas",
                columns: new[] { "PartnerId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdoptableAreas_Status",
                table: "AdoptableAreas",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdoptableAreas");
        }
    }
}
