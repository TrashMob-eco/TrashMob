#pragma warning disable CS8981
#pragma warning disable IDE1006

namespace TrashMob.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class historyfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "EventHistory",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventId",
                table: "EventHistory");
        }
    }
}
