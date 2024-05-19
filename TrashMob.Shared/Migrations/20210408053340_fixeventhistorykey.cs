﻿#pragma warning disable CS8981

namespace TrashMob.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class fixeventhistorykey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_EventHistory",
                table: "EventHistory",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EventHistory",
                table: "EventHistory");
        }
    }
}
