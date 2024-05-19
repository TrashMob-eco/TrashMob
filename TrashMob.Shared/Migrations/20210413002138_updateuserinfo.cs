#pragma warning disable CS8981
#pragma warning disable IDE1006

namespace TrashMob.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class updateuserinfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "Events",
                newName: "PostalCode");

            migrationBuilder.RenameColumn(
                name: "StateProvince",
                table: "Events",
                newName: "Region");

            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "EventHistory",
                newName: "PostalCode");

            migrationBuilder.RenameColumn(
                name: "StateProvince",
                table: "EventHistory",
                newName: "Region");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GivenName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SurName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GivenName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SurName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Region",
                table: "Events",
                newName: "StateProvince");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "Events",
                newName: "ZipCode");

            migrationBuilder.RenameColumn(
                name: "Region",
                table: "EventHistory",
                newName: "StateProvince");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "EventHistory",
                newName: "ZipCode");
        }
    }
}
