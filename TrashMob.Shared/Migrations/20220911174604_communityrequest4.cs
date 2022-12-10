using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    public partial class communityrequest4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CommunityRequest",
                table: "CommunityRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Community",
                table: "Community");

            migrationBuilder.RenameTable(
                name: "CommunityRequest",
                newName: "CommunityRequests");

            migrationBuilder.RenameTable(
                name: "Community",
                newName: "Communities");

            migrationBuilder.RenameIndex(
                name: "IX_CommunityRequest_CreatedByUserId",
                table: "CommunityRequests",
                newName: "IX_CommunityRequests_CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Community_LastUpdatedByUserId",
                table: "Communities",
                newName: "IX_Communities_LastUpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Community_CreatedByUserId",
                table: "Communities",
                newName: "IX_Communities_CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Community_CommunityStatusId",
                table: "Communities",
                newName: "IX_Communities_CommunityStatusId");

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "CommunityRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommunityRequests",
                table: "CommunityRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Communities",
                table: "Communities",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CommunityRequests",
                table: "CommunityRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Communities",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "CommunityRequests");

            migrationBuilder.RenameTable(
                name: "CommunityRequests",
                newName: "CommunityRequest");

            migrationBuilder.RenameTable(
                name: "Communities",
                newName: "Community");

            migrationBuilder.RenameIndex(
                name: "IX_CommunityRequests_CreatedByUserId",
                table: "CommunityRequest",
                newName: "IX_CommunityRequest_CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Communities_LastUpdatedByUserId",
                table: "Community",
                newName: "IX_Community_LastUpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Communities_CreatedByUserId",
                table: "Community",
                newName: "IX_Community_CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Communities_CommunityStatusId",
                table: "Community",
                newName: "IX_Community_CommunityStatusId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommunityRequest",
                table: "CommunityRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Community",
                table: "Community",
                column: "Id");
        }
    }
}
