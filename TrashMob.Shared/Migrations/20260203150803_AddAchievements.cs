using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddAchievements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AchievementTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Criteria = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAchievements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AchievementTypeId = table.Column<int>(type: "int", nullable: false),
                    EarnedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    NotificationSent = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAchievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAchievements_AchievementType",
                        column: x => x.AchievementTypeId,
                        principalTable: "AchievementTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAchievements_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAchievements_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserAchievements_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AchievementTypes",
                columns: new[] { "Id", "Category", "Criteria", "Description", "DisplayName", "DisplayOrder", "IconUrl", "IsActive", "Name", "Points" },
                values: new object[,]
                {
                    { 1, "Participation", "{\"eventsAttended\": 1}", "Attended your first cleanup event", "First Steps", 1, null, true, "FirstSteps", 10 },
                    { 2, "Participation", "{\"eventsAttended\": 10}", "Attended 10 cleanup events", "Regular Volunteer", 2, null, true, "RegularVolunteer", 50 },
                    { 3, "Participation", "{\"eventsAttended\": 25}", "Attended 25 cleanup events", "Dedicated Volunteer", 3, null, true, "DedicatedVolunteer", 100 },
                    { 4, "Impact", "{\"bagsCollected\": 10}", "Collected 10 bags of trash", "Trash Collector", 4, null, true, "TrashCollector", 25 },
                    { 5, "Impact", "{\"bagsCollected\": 100}", "Collected 100 bags of trash", "Trash Hero", 5, null, true, "TrashHero", 150 },
                    { 6, "Special", "{\"joinedTeam\": true}", "Joined a cleanup team", "Team Player", 6, null, true, "TeamPlayer", 20 },
                    { 7, "Special", "{\"eventsCreated\": 1}", "Created your first cleanup event", "Event Creator", 7, null, true, "EventCreator", 30 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_AchievementTypeId",
                table: "UserAchievements",
                column: "AchievementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_CreatedByUserId",
                table: "UserAchievements",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_LastUpdatedByUserId",
                table: "UserAchievements",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_UserId_AchievementTypeId",
                table: "UserAchievements",
                columns: new[] { "UserId", "AchievementTypeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAchievements");

            migrationBuilder.DropTable(
                name: "AchievementTypes");
        }
    }
}
