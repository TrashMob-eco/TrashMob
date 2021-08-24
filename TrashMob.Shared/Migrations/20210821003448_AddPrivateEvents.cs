using Microsoft.EntityFrameworkCore.Migrations;

namespace TrashMob.Migrations
{
    public partial class AddPrivateEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEventPublic",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEventPublic",
                table: "EventHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEventPublic",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "IsEventPublic",
                table: "EventHistory");
        }
    }
}
