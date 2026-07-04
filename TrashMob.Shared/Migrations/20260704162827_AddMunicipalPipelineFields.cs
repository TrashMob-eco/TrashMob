using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddMunicipalPipelineFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "CommunityProspects",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyObjection",
                table: "CommunityProspects",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PricingFeedback",
                table: "CommunityProspects",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "CommunityProspects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeRaw",
                table: "CommunityProspects",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            // Preserve the original free-form Type string in TypeRaw before we
            // constrain Type to MunicipalityTypeEnum values. Existing rows
            // primarily came from the Project 40 discovery service and use
            // terms like "municipality", "nonprofit", "civicorg", "hoa" —
            // most of these don't map cleanly to Cynthia's municipal-only
            // enum, so anything ambiguous is bucketed to "Other". The salesperson
            // can adjust individual rows in the admin UI.
            migrationBuilder.Sql(@"
                UPDATE CommunityProspects
                SET TypeRaw = Type
                WHERE Type IS NOT NULL AND TypeRaw IS NULL;

                UPDATE CommunityProspects
                SET Type =
                    CASE
                        WHEN Type IS NULL OR LTRIM(RTRIM(Type)) = '' THEN NULL
                        WHEN LOWER(LTRIM(RTRIM(Type))) LIKE '%county%' THEN 'County'
                        WHEN LOWER(LTRIM(RTRIM(Type))) LIKE '%town%' THEN 'Town'
                        WHEN LOWER(LTRIM(RTRIM(Type))) LIKE 'regional%' THEN 'RegionalAgency'
                        WHEN LOWER(LTRIM(RTRIM(Type))) LIKE '%special%'
                             OR LOWER(LTRIM(RTRIM(Type))) LIKE '%district%' THEN 'SpecialDistrict'
                        WHEN LOWER(LTRIM(RTRIM(Type))) LIKE '%city%' THEN 'City'
                        WHEN LOWER(LTRIM(RTRIM(Type))) LIKE '%muni%' THEN 'City'
                        ELSE 'Other'
                    END;
            ");

            // Remap PipelineStage from the old 5-value scheme to the new
            // 10-value PipelineStageEnum introduced in Project 63.
            //  0 (Identified)   -> 0 (Identified)
            //  1 (Contacted)    -> 2 (Contacted)
            //  2 (Responded)    -> 4 (Responded)
            //  3 (Interested)   -> 5 (DiscoveryInProgress)
            //  5 (Converted)    -> 5 (DiscoveryInProgress) — converted rows
            //                      are also flagged by ConvertedPartnerId, so
            //                      the stage becomes historical only.
            // Order the UPDATEs high-to-low so a row remapped to a higher
            // value in an earlier statement is not re-hit by a later statement.
            migrationBuilder.Sql(@"
                UPDATE CommunityProspects SET PipelineStage = 5 WHERE PipelineStage = 3;
                UPDATE CommunityProspects SET PipelineStage = 4 WHERE PipelineStage = 2;
                UPDATE CommunityProspects SET PipelineStage = 2 WHERE PipelineStage = 1;
                -- PipelineStage = 0 (Identified) and = 5 (was Converted, now
                -- treated as DiscoveryInProgress) are both left unchanged.
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "CommunityProspects");

            migrationBuilder.DropColumn(
                name: "KeyObjection",
                table: "CommunityProspects");

            migrationBuilder.DropColumn(
                name: "PricingFeedback",
                table: "CommunityProspects");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "CommunityProspects");

            migrationBuilder.DropColumn(
                name: "TypeRaw",
                table: "CommunityProspects");
        }
    }
}
