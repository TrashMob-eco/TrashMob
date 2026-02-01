using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddEventCoLeads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add IsEventLead column with default value of false
            migrationBuilder.AddColumn<bool>(
                name: "IsEventLead",
                table: "EventAttendees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Create index for efficient querying of event leads
            migrationBuilder.CreateIndex(
                name: "IX_EventAttendees_EventId_IsEventLead",
                table: "EventAttendees",
                columns: new[] { "EventId", "IsEventLead" });

            // Backfill: Ensure all event creators are marked as leads
            // First, insert EventAttendee records for event creators who aren't already attendees
            migrationBuilder.Sql(@"
                INSERT INTO EventAttendees (EventId, UserId, SignUpDate, IsEventLead, CreatedByUserId, CreatedDate, LastUpdatedByUserId, LastUpdatedDate)
                SELECT e.Id, e.CreatedByUserId, e.CreatedDate, 1, e.CreatedByUserId, GETUTCDATE(), e.CreatedByUserId, GETUTCDATE()
                FROM Events e
                WHERE NOT EXISTS (SELECT 1 FROM EventAttendees ea WHERE ea.EventId = e.Id AND ea.UserId = e.CreatedByUserId);
            ");

            // Then, update existing event creator attendees to be leads
            migrationBuilder.Sql(@"
                UPDATE ea SET IsEventLead = 1
                FROM EventAttendees ea
                INNER JOIN Events e ON ea.EventId = e.Id AND ea.UserId = e.CreatedByUserId;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventAttendees_EventId_IsEventLead",
                table: "EventAttendees");

            migrationBuilder.DropColumn(
                name: "IsEventLead",
                table: "EventAttendees");
        }
    }
}
