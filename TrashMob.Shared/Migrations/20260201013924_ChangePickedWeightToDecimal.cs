using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class ChangePickedWeightToDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PickedWeight",
                table: "EventSummaries",
                type: "decimal(10,1)",
                precision: 10,
                scale: 1,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PickedWeight",
                table: "EventSummaries",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,1)",
                oldPrecision: 10,
                oldScale: 1);
        }
    }
}
