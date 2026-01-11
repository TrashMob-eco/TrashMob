using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddPickupWeightToEventSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PickedWeight",
                table: "EventSummaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PickedWeightUnitId",
                table: "EventSummaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WeightUnitId",
                table: "EventSummaries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WeightUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightUnits", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "WeightUnits",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 0, "Weight unit not set", 1, true, "None" },
                    { 1, "Weight in Imperial Pounds", 2, true, "lb" },
                    { 2, "Weight in Kilograms", 3, true, "kg" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventSummaries_PickedWeightUnitId",
                table: "EventSummaries",
                column: "PickedWeightUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSummaries_WeightUnitId",
                table: "EventSummaries",
                column: "WeightUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventSummaries_WeightUnits_WeightUnitId",
                table: "EventSummaries",
                column: "WeightUnitId",
                principalTable: "WeightUnits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventSummary_PickedWeightUnits",
                table: "EventSummaries",
                column: "PickedWeightUnitId",
                principalTable: "WeightUnits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventSummaries_WeightUnits_WeightUnitId",
                table: "EventSummaries");

            migrationBuilder.DropForeignKey(
                name: "FK_EventSummary_PickedWeightUnits",
                table: "EventSummaries");

            migrationBuilder.DropTable(
                name: "WeightUnits");

            migrationBuilder.DropIndex(
                name: "IX_EventSummaries_PickedWeightUnitId",
                table: "EventSummaries");

            migrationBuilder.DropIndex(
                name: "IX_EventSummaries_WeightUnitId",
                table: "EventSummaries");

            migrationBuilder.DropColumn(
                name: "PickedWeight",
                table: "EventSummaries");

            migrationBuilder.DropColumn(
                name: "PickedWeightUnitId",
                table: "EventSummaries");

            migrationBuilder.DropColumn(
                name: "WeightUnitId",
                table: "EventSummaries");
        }
    }
}
