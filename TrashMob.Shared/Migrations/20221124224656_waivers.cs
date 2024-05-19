#nullable disable
#pragma warning disable CS8981
#pragma warning disable CA1861
#pragma warning disable IDE0300

namespace TrashMob.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class waivers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WaiverStatuses",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsWaiverEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaiverStatuses", x => x.Name);
                    table.ForeignKey(
                        name: "FK_WaiverStatuses_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WaiverStatuses_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "WaiverStatuses",
                columns: new[] { "Name", "CreatedByUserId", "CreatedDate", "Id", "IsWaiverEnabled", "LastUpdatedByUserId", "LastUpdatedDate" },
                values: new object[] { "trashmob", new Guid("00000000-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2022, 11, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("4d222d04-ac1f-4a87-886d-fdb686f9f55c"), false, new Guid("00000000-0000-0000-0000-000000000000"), null });

            migrationBuilder.CreateIndex(
                name: "IX_WaiverStatuses_CreatedByUserId",
                table: "WaiverStatuses",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WaiverStatuses_LastUpdatedByUserId",
                table: "WaiverStatuses",
                column: "LastUpdatedByUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaiverStatuses");
        }
    }
}
