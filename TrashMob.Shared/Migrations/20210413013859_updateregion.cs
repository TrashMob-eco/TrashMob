#pragma warning disable CS8981
#pragma warning disable IDE1006

namespace TrashMob.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class updateregion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "State",
                table: "Users",
                newName: "Region");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Region",
                table: "Users",
                newName: "State");
        }
    }
}
