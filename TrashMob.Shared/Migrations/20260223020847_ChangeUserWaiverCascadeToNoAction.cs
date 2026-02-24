using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Shared.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserWaiverCascadeToNoAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWaivers_User",
                table: "UserWaivers");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWaivers_User",
                table: "UserWaivers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWaivers_User",
                table: "UserWaivers");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWaivers_User",
                table: "UserWaivers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
