using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrashMob.Migrations
{
    public partial class UserProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendeeNotification_ApplicationUser",
                table: "AttendeeNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendees_ApplicationUser",
                table: "EventAttendees");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_ApplicationUser_CreatedBy",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_ApplicationUser_LastUpdatedBy",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFeedback_ApplicationUser",
                table: "UserFeedback");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFeedback_ApplicationUserRegarding",
                table: "UserFeedback");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_ApplicationUserFollowed",
                table: "UserSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_ApplicationUsersFollowing",
                table: "UserSubscription");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateAgreedToPrivacyPolicy = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PrivacyPolicyVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateAgreedToTermsOfService = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TermsOfServiceVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RecruitedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberSince = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUser_RecruitedBy",
                        column: x => x.RecruitedByUserId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_RecruitedByUserId",
                table: "UserProfiles",
                column: "RecruitedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendeeNotification_ApplicationUser",
                table: "AttendeeNotification",
                column: "UserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendees_ApplicationUser",
                table: "EventAttendees",
                column: "UserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_ApplicationUser_CreatedBy",
                table: "Events",
                column: "CreatedByUserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_ApplicationUser_LastUpdatedBy",
                table: "Events",
                column: "LastUpdatedByUserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFeedback_ApplicationUser",
                table: "UserFeedback",
                column: "UserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFeedback_ApplicationUserRegarding",
                table: "UserFeedback",
                column: "RegardingUserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_ApplicationUserFollowed",
                table: "UserSubscription",
                column: "UserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_ApplicationUsersFollowing",
                table: "UserSubscription",
                column: "FollowingId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendeeNotification_ApplicationUser",
                table: "AttendeeNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendees_ApplicationUser",
                table: "EventAttendees");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_ApplicationUser_CreatedBy",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_ApplicationUser_LastUpdatedBy",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFeedback_ApplicationUser",
                table: "UserFeedback");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFeedback_ApplicationUserRegarding",
                table: "UserFeedback");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_ApplicationUserFollowed",
                table: "UserSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_ApplicationUsersFollowing",
                table: "UserSubscription");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateAgreedToPrivacyPolicy = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DateAgreedToTermsOfService = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    MemberSince = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PrivacyPolicyVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RecruitedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TermsOfServiceVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUser_RecruitedBy",
                        column: x => x.RecruitedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RecruitedByUserId",
                table: "Users",
                column: "RecruitedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendeeNotification_ApplicationUser",
                table: "AttendeeNotification",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendees_ApplicationUser",
                table: "EventAttendees",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_ApplicationUser_CreatedBy",
                table: "Events",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_ApplicationUser_LastUpdatedBy",
                table: "Events",
                column: "LastUpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFeedback_ApplicationUser",
                table: "UserFeedback",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFeedback_ApplicationUserRegarding",
                table: "UserFeedback",
                column: "RegardingUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_ApplicationUserFollowed",
                table: "UserSubscription",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_ApplicationUsersFollowing",
                table: "UserSubscription",
                column: "FollowingId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
