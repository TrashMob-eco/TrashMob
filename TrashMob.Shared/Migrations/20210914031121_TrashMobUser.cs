using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrashMob.Migrations
{
    public partial class TrashMobUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "City", "Country", "DateAgreedToPrivacyPolicy", "DateAgreedToTermsOfService", "Email", "GivenName", "IsOptedOutOfAllEmails", "IsSiteAdmin", "Latitude", "Longitude", "MemberSince", "NameIdentifier", "PostalCode", "PrefersMetric", "PrivacyPolicyVersion", "Region", "SourceSystemUserName", "SurName", "TermsOfServiceVersion", "TravelLimitForLocalEvents", "UserName" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), "Anytown", "AnyCountry", null, null, "info@trashmob.eco", "TrashMob", false, false, null, null, null, null, null, false, null, "AnyState", null, "Eco", null, 0, "TrashMob" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
