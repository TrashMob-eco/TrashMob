using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddNewsletterSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewsletterCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsletterTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HtmlContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Newsletters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PreviewText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HtmlContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "All"),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Draft"),
                    ScheduledDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SentDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RecipientCount = table.Column<int>(type: "int", nullable: false),
                    SentCount = table.Column<int>(type: "int", nullable: false),
                    DeliveredCount = table.Column<int>(type: "int", nullable: false),
                    OpenCount = table.Column<int>(type: "int", nullable: false),
                    ClickCount = table.Column<int>(type: "int", nullable: false),
                    BounceCount = table.Column<int>(type: "int", nullable: false),
                    UnsubscribeCount = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Newsletters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Newsletters_NewsletterCategory",
                        column: x => x.CategoryId,
                        principalTable: "NewsletterCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Newsletters_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Newsletters_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserNewsletterPreferences",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsSubscribed = table.Column<bool>(type: "bit", nullable: false),
                    SubscribedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UnsubscribedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNewsletterPreferences", x => new { x.UserId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_UserNewsletterPreferences_Category",
                        column: x => x.CategoryId,
                        principalTable: "NewsletterCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNewsletterPreferences_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Newsletters_CategoryId",
                table: "Newsletters",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Newsletters_CreatedByUserId",
                table: "Newsletters",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Newsletters_LastUpdatedByUserId",
                table: "Newsletters",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Newsletters_Scheduled",
                table: "Newsletters",
                column: "ScheduledDate",
                filter: "[Status] = 'Scheduled'");

            migrationBuilder.CreateIndex(
                name: "IX_Newsletters_Status",
                table: "Newsletters",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UserNewsletterPreferences_CategoryId",
                table: "UserNewsletterPreferences",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNewsletterPreferences_UserId",
                table: "UserNewsletterPreferences",
                column: "UserId");

            // Seed default newsletter categories
            migrationBuilder.InsertData(
                table: "NewsletterCategories",
                columns: new[] { "Id", "Name", "Description", "DisplayOrder", "IsActive", "IsDefault" },
                values: new object[,]
                {
                    { 1, "Monthly Digest", "Monthly updates about TrashMob events, impact stories, and community highlights", 1, true, true },
                    { 2, "Event Updates", "Notifications about new events in your area and event reminders", 2, true, true },
                    { 3, "Community News", "News and updates from communities you follow", 3, true, false },
                    { 4, "Team Updates", "Updates from teams you are a member of", 4, true, false }
                });

            // Seed default newsletter template
            migrationBuilder.InsertData(
                table: "NewsletterTemplates",
                columns: new[] { "Id", "Name", "Description", "DisplayOrder", "IsActive", "HtmlContent", "TextContent", "ThumbnailUrl" },
                values: new object[]
                {
                    1, "Standard Newsletter", "Default TrashMob newsletter template with header, content area, and footer", 1, true,
                    @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{{subject}}</title>
</head>
<body style=""font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f5f5f5;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""max-width: 600px; margin: 0 auto; background-color: #ffffff;"">
        <tr>
            <td style=""background-color: #00a651; padding: 20px; text-align: center;"">
                <img src=""https://www.trashmob.eco/trashmoblogo.png"" alt=""TrashMob.eco"" height=""50"" style=""display: block; margin: 0 auto;"">
            </td>
        </tr>
        <tr>
            <td style=""padding: 30px 20px;"">
                {{content}}
            </td>
        </tr>
        <tr>
            <td style=""background-color: #333333; color: #ffffff; padding: 20px; text-align: center; font-size: 12px;"">
                <p style=""margin: 0 0 10px 0;"">TrashMob.eco - Cleaning up our communities together</p>
                <p style=""margin: 0;"">
                    <a href=""{{unsubscribe_url}}"" style=""color: #ffffff;"">Unsubscribe</a> |
                    <a href=""{{preferences_url}}"" style=""color: #ffffff;"">Manage Preferences</a>
                </p>
            </td>
        </tr>
    </table>
</body>
</html>",
                    @"TrashMob.eco Newsletter

{{content}}

---
TrashMob.eco - Cleaning up our communities together

To unsubscribe: {{unsubscribe_url}}
To manage preferences: {{preferences_url}}",
                    null
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Newsletters");

            migrationBuilder.DropTable(
                name: "NewsletterTemplates");

            migrationBuilder.DropTable(
                name: "UserNewsletterPreferences");

            migrationBuilder.DropTable(
                name: "NewsletterCategories");
        }
    }
}
