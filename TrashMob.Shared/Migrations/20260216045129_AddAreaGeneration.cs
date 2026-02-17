using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddAreaGeneration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AreaGenerationBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Queued"),
                    DiscoveredCount = table.Column<int>(type: "int", nullable: false),
                    ProcessedCount = table.Column<int>(type: "int", nullable: false),
                    SkippedCount = table.Column<int>(type: "int", nullable: false),
                    StagedCount = table.Column<int>(type: "int", nullable: false),
                    ApprovedCount = table.Column<int>(type: "int", nullable: false),
                    RejectedCount = table.Column<int>(type: "int", nullable: false),
                    CreatedCount = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CompletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    BoundsNorth = table.Column<double>(type: "float", nullable: true),
                    BoundsSouth = table.Column<double>(type: "float", nullable: true),
                    BoundsEast = table.Column<double>(type: "float", nullable: true),
                    BoundsWest = table.Column<double>(type: "float", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaGenerationBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AreaGenerationBatches_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AreaGenerationBatches_LastUpdatedByUser_Id",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AreaGenerationBatches_Partners",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StagedAdoptableAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    AreaType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GeoJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CenterLatitude = table.Column<double>(type: "float", nullable: false),
                    CenterLongitude = table.Column<double>(type: "float", nullable: false),
                    ReviewStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    Confidence = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Medium"),
                    IsPotentialDuplicate = table.Column<bool>(type: "bit", nullable: false),
                    DuplicateOfName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OsmId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OsmTags = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedAdoptableAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StagedAdoptableAreas_AreaGenerationBatches",
                        column: x => x.BatchId,
                        principalTable: "AreaGenerationBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StagedAdoptableAreas_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StagedAdoptableAreas_LastUpdatedByUser_Id",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StagedAdoptableAreas_Partners",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AreaGenerationBatches_CreatedByUserId",
                table: "AreaGenerationBatches",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaGenerationBatches_LastUpdatedByUserId",
                table: "AreaGenerationBatches",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaGenerationBatches_PartnerId_Status",
                table: "AreaGenerationBatches",
                columns: new[] { "PartnerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StagedAdoptableAreas_BatchId_ReviewStatus",
                table: "StagedAdoptableAreas",
                columns: new[] { "BatchId", "ReviewStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_StagedAdoptableAreas_CreatedByUserId",
                table: "StagedAdoptableAreas",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedAdoptableAreas_LastUpdatedByUserId",
                table: "StagedAdoptableAreas",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedAdoptableAreas_PartnerId",
                table: "StagedAdoptableAreas",
                column: "PartnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StagedAdoptableAreas");

            migrationBuilder.DropTable(
                name: "AreaGenerationBatches");
        }
    }
}
