#nullable disable
#pragma warning disable CS8981

namespace TrashMob.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class addifttt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IftttTriggers",
                columns: table => new
                {
                    TriggerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TriggerFields = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IftttSource = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Limit = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IftttTriggers", x => x.TriggerId);
                    table.ForeignKey(
                        name: "FK_IffttTriggers_LastUpdatedByUser_Id",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IftttTriggers_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IftttTriggers_CreatedByUserId",
                table: "IftttTriggers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IftttTriggers_LastUpdatedByUserId",
                table: "IftttTriggers",
                column: "LastUpdatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IftttTriggers");
        }
    }
}
