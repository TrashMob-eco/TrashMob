#nullable disable

namespace TrashMob.Models
{
    using System;
    using System.Collections.Generic;

    public record ExportedProfile(
        string UserName,
        string Email,
        string GivenName,
        string Surname,
        DateTimeOffset? DateOfBirth,
        string City,
        string Region,
        string Country,
        string PostalCode,
        double? Latitude,
        double? Longitude,
        DateTimeOffset? MemberSince,
        bool PrefersMetric,
        bool ShowOnLeaderboards,
        bool AchievementNotificationsEnabled,
        int TravelLimitForLocalEvents);

    public record ExportedEventParticipation(
        Guid EventId,
        string EventName,
        DateTimeOffset EventDate,
        string City,
        DateTimeOffset SignUpDate,
        DateTimeOffset? CanceledDate,
        bool IsEventLead);

    public record ExportedEventLed(
        Guid EventId,
        string Name,
        string Description,
        DateTimeOffset EventDate,
        string City,
        string Region,
        int DurationHours,
        int DurationMinutes);

    public record ExportedEventSummary(
        Guid EventId,
        int? NumberOfBags,
        int? NumberOfBuckets,
        int? DurationInMinutes,
        int? ActualNumberOfAttendees,
        decimal? PickedWeight);

    public record ExportedRoute(
        Guid EventId,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime,
        int TotalDistanceMeters,
        int DurationMinutes,
        int? BagsCollected,
        decimal? WeightCollected,
        string PrivacyLevel,
        string Notes,
        string UserPathGeoJson);

    public record ExportedAttendeeMetrics(
        Guid EventId,
        int? BagsCollected,
        decimal? PickedWeight,
        int? DurationMinutes,
        string Status,
        string Notes);

    public record ExportedLitterReport(
        Guid Id,
        string Name,
        string Description,
        int LitterReportStatusId,
        DateTimeOffset? CreatedDate,
        List<string> ImageUrls);

    public record ExportedTeamMembership(
        Guid TeamId,
        string TeamName,
        bool IsTeamLead,
        DateTimeOffset JoinedDate);

    public record ExportedAchievement(
        string AchievementName,
        int AchievementTypeId,
        DateTimeOffset EarnedDate);

    public record ExportedWaiver(
        DateTimeOffset AcceptedDate,
        DateTimeOffset ExpiryDate,
        string TypedLegalName,
        string SigningMethod,
        bool IsMinor,
        string GuardianName,
        string GuardianRelationship);

    public record ExportedFeedback(
        string Category,
        string Description,
        string Status,
        DateTimeOffset? CreatedDate);

    public record ExportedPartnerAdminRole(
        Guid PartnerId,
        string PartnerName,
        DateTimeOffset? CreatedDate);

    public record ExportedNewsletterPreference(
        int CategoryId,
        bool IsSubscribed);
}
