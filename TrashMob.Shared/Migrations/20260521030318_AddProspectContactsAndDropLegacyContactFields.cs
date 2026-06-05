using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddProspectContactsAndDropLegacyContactFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create ProspectContacts table.
            migrationBuilder.CreateTable(
                name: "ProspectContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProspectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReferredByContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProspectContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProspectContacts_CommunityProspect",
                        column: x => x.ProspectId,
                        principalTable: "CommunityProspects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProspectContacts_ReferredByContact",
                        column: x => x.ReferredByContactId,
                        principalTable: "ProspectContacts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProspectContacts_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProspectContacts_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProspectContacts_CreatedByUserId",
                table: "ProspectContacts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProspectContacts_LastUpdatedByUserId",
                table: "ProspectContacts",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProspectContacts_ProspectId",
                table: "ProspectContacts",
                column: "ProspectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProspectContacts_ReferredByContactId",
                table: "ProspectContacts",
                column: "ReferredByContactId");

            // 2. Add nullable ProspectContactId FK columns to existing activity/outreach tables.
            migrationBuilder.AddColumn<Guid>(
                name: "ProspectContactId",
                table: "ProspectOutreachEmails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProspectContactId",
                table: "ProspectActivities",
                type: "uniqueidentifier",
                nullable: true);

            // 3. Backfill ProspectContacts from existing CommunityProspect contact columns.
            //    One row per prospect that had ANY contact field populated. Marked primary.
            //    This step MUST run before the DropColumn calls below.
            migrationBuilder.Sql(@"
                INSERT INTO ProspectContacts
                    (Id, ProspectId, Name, Title, Email, Phone, ContactStatus, IsPrimary,
                     CreatedByUserId, CreatedDate, LastUpdatedByUserId, LastUpdatedDate)
                SELECT
                    NEWID(),
                    p.Id,
                    ISNULL(NULLIF(LTRIM(RTRIM(p.ContactName)), ''), '(unknown)'),
                    NULLIF(LTRIM(RTRIM(p.ContactTitle)), ''),
                    NULLIF(LTRIM(RTRIM(p.ContactEmail)), ''),
                    NULLIF(LTRIM(RTRIM(p.ContactPhone)), ''),
                    0, -- ContactStatus.Active
                    1, -- IsPrimary
                    p.CreatedByUserId,
                    ISNULL(p.CreatedDate, SYSDATETIMEOFFSET()),
                    p.LastUpdatedByUserId,
                    ISNULL(p.LastUpdatedDate, SYSDATETIMEOFFSET())
                FROM CommunityProspects p
                WHERE
                    (p.ContactName IS NOT NULL AND LTRIM(RTRIM(p.ContactName)) <> '')
                 OR (p.ContactEmail IS NOT NULL AND LTRIM(RTRIM(p.ContactEmail)) <> '')
                 OR (p.ContactTitle IS NOT NULL AND LTRIM(RTRIM(p.ContactTitle)) <> '')
                 OR (p.ContactPhone IS NOT NULL AND LTRIM(RTRIM(p.ContactPhone)) <> '');
            ");

            // 4. Now safe to drop the legacy columns.
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "CommunityProspects");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "CommunityProspects");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "CommunityProspects");

            migrationBuilder.DropColumn(
                name: "ContactTitle",
                table: "CommunityProspects");

            // 5. Indexes + FKs for the new ProspectContactId columns.
            migrationBuilder.CreateIndex(
                name: "IX_ProspectOutreachEmails_ProspectContactId",
                table: "ProspectOutreachEmails",
                column: "ProspectContactId");

            migrationBuilder.CreateIndex(
                name: "IX_ProspectActivities_ProspectContactId",
                table: "ProspectActivities",
                column: "ProspectContactId");

            // ReferentialAction.NoAction (not SetNull) is required here. SQL Server refuses
            // FKs that create multiple cascade paths to the same table: ProspectActivities
            // already has a Cascade FK to CommunityProspects, and ProspectContacts has its own
            // Cascade FK to CommunityProspects, so a second SetNull path from ProspectContacts
            // back to ProspectActivities triggers SQL error 1785. The DeleteBehavior on the
            // EF side is ClientSetNull, which gives the same observable behavior (null'd in
            // memory when EF loads the entity) without the DB-level cascade.
            migrationBuilder.AddForeignKey(
                name: "FK_ProspectActivities_ProspectContact",
                table: "ProspectActivities",
                column: "ProspectContactId",
                principalTable: "ProspectContacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ProspectOutreachEmails_ProspectContact",
                table: "ProspectOutreachEmails",
                column: "ProspectContactId",
                principalTable: "ProspectContacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate legacy columns (as nullable). Backfill primary-contact data into them
            // so a rollback preserves the data we previously moved into ProspectContacts.
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "CommunityProspects",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "CommunityProspects",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "CommunityProspects",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactTitle",
                table: "CommunityProspects",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE p
                SET p.ContactName = c.Name,
                    p.ContactTitle = c.Title,
                    p.ContactEmail = c.Email,
                    p.ContactPhone = c.Phone
                FROM CommunityProspects p
                INNER JOIN ProspectContacts c ON c.ProspectId = p.Id AND c.IsPrimary = 1;
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_ProspectActivities_ProspectContact",
                table: "ProspectActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_ProspectOutreachEmails_ProspectContact",
                table: "ProspectOutreachEmails");

            migrationBuilder.DropIndex(
                name: "IX_ProspectOutreachEmails_ProspectContactId",
                table: "ProspectOutreachEmails");

            migrationBuilder.DropIndex(
                name: "IX_ProspectActivities_ProspectContactId",
                table: "ProspectActivities");

            migrationBuilder.DropColumn(
                name: "ProspectContactId",
                table: "ProspectOutreachEmails");

            migrationBuilder.DropColumn(
                name: "ProspectContactId",
                table: "ProspectActivities");

            migrationBuilder.DropTable(
                name: "ProspectContacts");
        }
    }
}
