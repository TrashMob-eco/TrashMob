using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddSponsoredAdoptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfessionalCompanies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionalCompanies_Partners",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfessionalCompanies_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProfessionalCompanies_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Sponsors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ShowOnPublicMap = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sponsors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sponsors_Partners",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sponsors_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sponsors_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProfessionalCompanyUsers",
                columns: table => new
                {
                    ProfessionalCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalCompanyUsers", x => new { x.ProfessionalCompanyId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ProfessionalCompanyUsers_ProfessionalCompany",
                        column: x => x.ProfessionalCompanyId,
                        principalTable: "ProfessionalCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfessionalCompanyUsers_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProfessionalCompanyUsers_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProfessionalCompanyUsers_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SponsoredAdoptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdoptableAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SponsorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfessionalCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CleanupFrequencyDays = table.Column<int>(type: "int", nullable: false, defaultValue: 14),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsoredAdoptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SponsoredAdoptions_AdoptableArea",
                        column: x => x.AdoptableAreaId,
                        principalTable: "AdoptableAreas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SponsoredAdoptions_ProfessionalCompany",
                        column: x => x.ProfessionalCompanyId,
                        principalTable: "ProfessionalCompanies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SponsoredAdoptions_Sponsor",
                        column: x => x.SponsorId,
                        principalTable: "Sponsors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SponsoredAdoptions_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SponsoredAdoptions_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProfessionalCleanupLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SponsoredAdoptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfessionalCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CleanupDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    BagsCollected = table.Column<int>(type: "int", nullable: false),
                    WeightInPounds = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    WeightInKilograms = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalCleanupLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionalCleanupLogs_ProfessionalCompany",
                        column: x => x.ProfessionalCompanyId,
                        principalTable: "ProfessionalCompanies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProfessionalCleanupLogs_SponsoredAdoption",
                        column: x => x.SponsoredAdoptionId,
                        principalTable: "SponsoredAdoptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfessionalCleanupLogs_User_CreatedBy",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProfessionalCleanupLogs_User_LastUpdatedBy",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalCleanupLogs_CleanupDate",
                table: "ProfessionalCleanupLogs",
                column: "CleanupDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalCleanupLogs_CreatedByUserId",
                table: "ProfessionalCleanupLogs",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalCleanupLogs_LastUpdatedByUserId",
                table: "ProfessionalCleanupLogs",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalCleanupLogs_ProfessionalCompanyId",
                table: "ProfessionalCleanupLogs",
                column: "ProfessionalCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalCleanupLogs_SponsoredAdoptionId",
                table: "ProfessionalCleanupLogs",
                column: "SponsoredAdoptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalCompanies_CreatedByUserId",
                table: "ProfessionalCompanies",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalCompanies_LastUpdatedByUserId",
                table: "ProfessionalCompanies",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalCompanies_PartnerId",
                table: "ProfessionalCompanies",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalCompanyUsers_CreatedByUserId",
                table: "ProfessionalCompanyUsers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalCompanyUsers_LastUpdatedByUserId",
                table: "ProfessionalCompanyUsers",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalCompanyUsers_UserId",
                table: "ProfessionalCompanyUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsoredAdoptions_AdoptableAreaId",
                table: "SponsoredAdoptions",
                column: "AdoptableAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsoredAdoptions_CreatedByUserId",
                table: "SponsoredAdoptions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsoredAdoptions_LastUpdatedByUserId",
                table: "SponsoredAdoptions",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsoredAdoptions_ProfessionalCompanyId",
                table: "SponsoredAdoptions",
                column: "ProfessionalCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsoredAdoptions_SponsorId",
                table: "SponsoredAdoptions",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsoredAdoptions_Status",
                table: "SponsoredAdoptions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_CreatedByUserId",
                table: "Sponsors",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_LastUpdatedByUserId",
                table: "Sponsors",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_PartnerId",
                table: "Sponsors",
                column: "PartnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfessionalCleanupLogs");

            migrationBuilder.DropTable(
                name: "ProfessionalCompanyUsers");

            migrationBuilder.DropTable(
                name: "SponsoredAdoptions");

            migrationBuilder.DropTable(
                name: "ProfessionalCompanies");

            migrationBuilder.DropTable(
                name: "Sponsors");
        }
    }
}
