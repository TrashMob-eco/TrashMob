using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class RemoveWaivers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserWaivers");

            migrationBuilder.DropTable(
                name: "Waivers");

            migrationBuilder.DropTable(
                name: "WaiverDurationTypes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WaiverDurationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaiverDurationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Waivers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WaiverDurationTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EffectiveDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Version = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Waivers_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Waivers_LastUpdatedByUser_Id",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Waivers_WaiverDurationType_Id",
                        column: x => x.WaiverDurationTypeId,
                        principalTable: "WaiverDurationTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserWaivers",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WaiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EffectiveDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWaivers", x => new { x.UserId, x.WaiverId });
                    table.ForeignKey(
                        name: "FK_UserWaiver_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserWaiver_Waivers",
                        column: x => x.WaiverId,
                        principalTable: "Waivers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserWaivers_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserWaivers_LastUpdatedByUser_Id",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "WaiverDurationTypes",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Waiver Expires at the end of the current calendar year", 1, true, "Calendar Year" },
                    { 2, "Waiver Expires a year to the date after signing", 2, true, "Year from Signing" },
                    { 3, "Waiver Expires at the end of the current calendar month", 3, true, "Calendar Year" },
                    { 4, "Waiver Expires at the end of the current day", 4, true, "Calendar Year" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserWaivers_CreatedByUserId",
                table: "UserWaivers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWaivers_LastUpdatedByUserId",
                table: "UserWaivers",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWaivers_WaiverId",
                table: "UserWaivers",
                column: "WaiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Waivers_CreatedByUserId",
                table: "Waivers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Waivers_LastUpdatedByUserId",
                table: "Waivers",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Waivers_WaiverDurationTypeId",
                table: "Waivers",
                column: "WaiverDurationTypeId");
        }
    }
}
