using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashMob.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunityHomePageFieldsToPartner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BannerImageUrl",
                table: "Partners",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrandingPrimaryColor",
                table: "Partners",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrandingSecondaryColor",
                table: "Partners",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Partners",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Partners",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HomePageEnabled",
                table: "Partners",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "HomePageEndDate",
                table: "Partners",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "HomePageStartDate",
                table: "Partners",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Partners",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Partners",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Partners",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Partners",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tagline",
                table: "Partners",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partners_Slug",
                table: "Partners",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Partners_Slug",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "BannerImageUrl",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "BrandingPrimaryColor",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "BrandingSecondaryColor",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "HomePageEnabled",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "HomePageEndDate",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "HomePageStartDate",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Tagline",
                table: "Partners");
        }
    }
}
