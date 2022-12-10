using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class refactor19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventMedias");

            migrationBuilder.DropTable(
                name: "MediaType");

            migrationBuilder.DropTable(
                name: "MediaUsageType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaType",
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
                    table.PrimaryKey("PK_MediaType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaUsageType",
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
                    table.PrimaryKey("PK_MediaUsageType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventMedias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaTypeId = table.Column<int>(type: "int", nullable: false),
                    MediaUsageTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    MediaUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventMedia_Event_Id",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventMedias_MediaTypes",
                        column: x => x.MediaTypeId,
                        principalTable: "MediaType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventMedias_MediaUsageTypes",
                        column: x => x.MediaUsageTypeId,
                        principalTable: "MediaUsageType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventMedias_User_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventMedias_Users_LastUpdatedByUserId",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MediaType",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Instagram Image or Video", 1, true, "Instagram" },
                    { 2, "YouTube Video", 2, true, "YouTube" }
                });

            migrationBuilder.InsertData(
                table: "MediaUsageType",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Before a cleanup event", 1, true, "BeforeEvent" },
                    { 2, "After a cleanup event", 2, true, "AfterEvent" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventMedias_CreatedByUserId",
                table: "EventMedias",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventMedias_EventId",
                table: "EventMedias",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventMedias_LastUpdatedByUserId",
                table: "EventMedias",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventMedias_MediaTypeId",
                table: "EventMedias",
                column: "MediaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventMedias_MediaUsageTypeId",
                table: "EventMedias",
                column: "MediaUsageTypeId");
        }
    }
}
