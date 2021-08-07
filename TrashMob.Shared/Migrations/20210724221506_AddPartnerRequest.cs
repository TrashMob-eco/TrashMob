using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrashMob.Migrations
{
    public partial class AddPartnerRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartnerRequestStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerRequestStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartnerRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PrimaryEmail = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SecondaryEmail = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PrimaryPhone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    SecondaryPhone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    PartnerRequestStatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerRequests_CreatedByUser_Id",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartnerRequests_LastUpdatedByUser_Id",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartnerRequests_PartnerRequestStatus",
                        column: x => x.PartnerRequestStatusId,
                        principalTable: "PartnerRequestStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "PartnerRequestStatus",
                columns: new[] { "Id", "Description", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, "Request is Pending Approval by Site Administrator", 1, "Pending" },
                    { 2, "Request has been canceled by the requestor", 2, "Canceled" },
                    { 3, "Request has been approved by the Site Administrator", 3, "Approved" },
                    { 4, "Request has been approved by the Site Administrator", 4, "Rejected" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerRequests_CreatedByUserId",
                table: "PartnerRequests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerRequests_LastUpdatedByUserId",
                table: "PartnerRequests",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerRequests_PartnerRequestStatusId",
                table: "PartnerRequests",
                column: "PartnerRequestStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartnerRequests");

            migrationBuilder.DropTable(
                name: "PartnerRequestStatus");
        }
    }
}
