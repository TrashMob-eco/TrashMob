namespace TrashMob.Shared.Managers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;

    /// <summary>
    /// Exports all personal data for a user as streaming JSON.
    /// Uses Utf8JsonWriter to write directly to the output stream,
    /// querying each data category sequentially to avoid timeout issues.
    /// </summary>
    public class UserDataExportManager(MobDbContext context) : IUserDataExportManager
    {
        /// <inheritdoc />
        public async Task WriteExportToStreamAsync(Guid userId, Stream outputStream, CancellationToken ct = default)
        {
            await using var writer = new Utf8JsonWriter(outputStream, new JsonWriterOptions { Indented = true });

            writer.WriteStartObject();

            WriteMetadata(writer);
            await WriteProfileAsync(writer, userId, ct);
            await WriteEventParticipationAsync(writer, userId, ct);
            await WriteEventsLedAsync(writer, userId, ct);
            await WriteEventSummariesAsync(writer, userId, ct);
            await WriteRouteDataAsync(writer, userId, ct);
            await WriteAttendeeMetricsAsync(writer, userId, ct);
            await WriteLitterReportsAsync(writer, userId, ct);
            await WriteTeamMembershipsAsync(writer, userId, ct);
            await WriteAchievementsAsync(writer, userId, ct);
            await WriteWaiversAsync(writer, userId, ct);
            await WriteFeedbackAsync(writer, userId, ct);
            await WritePartnerAdminRolesAsync(writer, userId, ct);
            await WriteNotificationPreferencesAsync(writer, userId, ct);

            writer.WriteEndObject();
            await writer.FlushAsync(ct);
        }

        private static void WriteMetadata(Utf8JsonWriter writer)
        {
            writer.WriteStartObject("_metadata");
            writer.WriteString("exportDate", DateTimeOffset.UtcNow);
            writer.WriteString("format", "TrashMob User Data Export v1");
            writer.WriteString("description", "Contains all personal data associated with your TrashMob account. Each section represents a different type of data. Dates are in UTC. Coordinates are in WGS84 (latitude/longitude).");
            writer.WriteEndObject();
        }

        private async Task WriteProfileAsync(Utf8JsonWriter writer, Guid userId, CancellationToken ct)
        {
            var user = await context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new ExportedProfile(
                    u.UserName,
                    u.Email,
                    u.GivenName,
                    u.Surname,
                    u.DateOfBirth,
                    u.City,
                    u.Region,
                    u.Country,
                    u.PostalCode,
                    u.Latitude,
                    u.Longitude,
                    u.MemberSince,
                    u.PrefersMetric,
                    u.ShowOnLeaderboards,
                    u.AchievementNotificationsEnabled,
                    u.TravelLimitForLocalEvents))
                .FirstOrDefaultAsync(ct);

            writer.WritePropertyName("profile");
            JsonSerializer.Serialize(writer, user);
            await writer.FlushAsync(ct);
        }

        private async Task WriteEventParticipationAsync(Utf8JsonWriter writer, Guid userId, CancellationToken ct)
        {
            var events = await context.EventAttendees
                .AsNoTracking()
                .Where(ea => ea.UserId == userId)
                .Join(context.Events, ea => ea.EventId, e => e.Id,
                    (ea, e) => new { ea, e })
                .OrderByDescending(x => x.e.EventDate)
                .Select(x => new ExportedEventParticipation(
                    x.e.Id,
                    x.e.Name,
                    x.e.EventDate,
                    x.e.City,
                    x.ea.SignUpDate,
                    x.ea.CanceledDate,
                    x.ea.IsEventLead))
                .ToListAsync(ct);

            writer.WritePropertyName("eventParticipation");
            JsonSerializer.Serialize(writer, events);
            await writer.FlushAsync(ct);
        }

        private async Task WriteEventsLedAsync(Utf8JsonWriter writer, Guid userId, CancellationToken ct)
        {
            var events = await context.Events
                .AsNoTracking()
                .Where(e => e.CreatedByUserId == userId)
                .Select(e => new ExportedEventLed(
                    e.Id,
                    e.Name,
                    e.Description,
                    e.EventDate,
                    e.City,
                    e.Region,
                    e.DurationHours,
                    e.DurationMinutes))
                .OrderByDescending(e => e.EventDate)
                .ToListAsync(ct);

            writer.WritePropertyName("eventsLed");
            JsonSerializer.Serialize(writer, events);
            await writer.FlushAsync(ct);
        }

        private async Task WriteEventSummariesAsync(Utf8JsonWriter writer, Guid userId, CancellationToken ct)
        {
            var summaries = await context.EventSummaries
                .AsNoTracking()
                .Where(es => es.CreatedByUserId == userId)
                .Select(es => new ExportedEventSummary(
                    es.EventId,
                    es.NumberOfBags,
                    es.NumberOfBuckets,
                    es.DurationInMinutes,
                    es.ActualNumberOfAttendees,
                    es.PickedWeight))
                .ToListAsync(ct);

            writer.WritePropertyName("eventSummaries");
            JsonSerializer.Serialize(writer, summaries);
            await writer.FlushAsync(ct);
        }

        private async Task WriteRouteDataAsync(Utf8JsonWriter writer, Guid userId, CancellationToken ct)
        {
            var routes = await context.EventAttendeeRoutes
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .Select(r => new
                {
                    r.EventId,
                    r.StartTime,
                    r.EndTime,
                    r.TotalDistanceMeters,
                    r.DurationMinutes,
                    r.BagsCollected,
                    r.WeightCollected,
                    r.PrivacyLevel,
                    r.Notes,
                    r.UserPath
                })
                .OrderByDescending(r => r.StartTime)
                .ToListAsync(ct);

            var exportedRoutes = routes.Select(r => new ExportedRoute(
                r.EventId,
                r.StartTime,
                r.EndTime,
                r.TotalDistanceMeters,
                r.DurationMinutes,
                r.BagsCollected,
                r.WeightCollected,
                r.PrivacyLevel,
                r.Notes,
                r.UserPath?.AsText())).ToList();

            writer.WritePropertyName("routeData");
            JsonSerializer.Serialize(writer, exportedRoutes);
            await writer.FlushAsync(ct);
        }

        private async Task WriteAttendeeMetricsAsync(Utf8JsonWriter writer, Guid userId, CancellationToken ct)
        {
            var metrics = await context.EventAttendeeMetrics
                .AsNoTracking()
                .Where(m => m.UserId == userId)
                .Select(m => new ExportedAttendeeMetrics(
                    m.EventId,
                    m.BagsCollected,
                    m.PickedWeight,
                    m.DurationMinutes,
                    m.Status,
                    m.Notes))
                .ToListAsync(ct);

            writer.WritePropertyName("attendeeMetrics");
            JsonSerializer.Serialize(writer, metrics);
            await writer.FlushAsync(ct);
        }

        private async Task WriteLitterReportsAsync(Utf8JsonWriter writer, Guid userId, CancellationToken ct)
        {
            var reports = await context.LitterReports
                .AsNoTracking()
                .Where(lr => lr.CreatedByUserId == userId)
                .Select(lr => new ExportedLitterReport(
                    lr.Id,
                    lr.Name,
                    lr.Description,
                    lr.LitterReportStatusId,
                    lr.CreatedDate,
                    lr.LitterImages.Select(li => li.AzureBlobURL).ToList()))
                .ToListAsync(ct);

            writer.WritePropertyName("litterReports");
            JsonSerializer.Serialize(writer, reports);
            await writer.FlushAsync(ct);
        }

        private async Task WriteTeamMembershipsAsync(Utf8JsonWriter writer, Guid userId, CancellationToken ct)
        {
            var memberships = await context.TeamMembers
                .AsNoTracking()
                .Where(tm => tm.UserId == userId)
                .Join(context.Teams, tm => tm.TeamId, t => t.Id,
                    (tm, t) => new { tm, t })
                .Select(x => new ExportedTeamMembership(
                    x.t.Id,
                    x.t.Name,
                    x.tm.IsTeamLead,
                    x.tm.JoinedDate))
                .ToListAsync(ct);

            writer.WritePropertyName("teamMemberships");
            JsonSerializer.Serialize(writer, memberships);
            await writer.FlushAsync(ct);
        }

        private async Task WriteAchievementsAsync(Utf8JsonWriter writer, Guid userId, CancellationToken ct)
        {
            var achievements = await context.UserAchievements
                .AsNoTracking()
                .Where(ua => ua.UserId == userId)
                .Join(context.Set<AchievementType>(), ua => ua.AchievementTypeId, at => at.Id,
                    (ua, at) => new { ua, at })
                .OrderByDescending(x => x.ua.EarnedDate)
                .Select(x => new ExportedAchievement(
                    x.at.Name,
                    x.at.Id,
                    x.ua.EarnedDate))
                .ToListAsync(ct);

            writer.WritePropertyName("achievements");
            JsonSerializer.Serialize(writer, achievements);
            await writer.FlushAsync(ct);
        }

        private async Task WriteWaiversAsync(Utf8JsonWriter writer, Guid userId, CancellationToken ct)
        {
            var waivers = await context.UserWaivers
                .AsNoTracking()
                .Where(uw => uw.UserId == userId)
                .Select(uw => new ExportedWaiver(
                    uw.AcceptedDate,
                    uw.ExpiryDate,
                    uw.TypedLegalName,
                    uw.SigningMethod,
                    uw.IsMinor,
                    uw.GuardianName,
                    uw.GuardianRelationship))
                .OrderByDescending(w => w.AcceptedDate)
                .ToListAsync(ct);

            writer.WritePropertyName("waivers");
            JsonSerializer.Serialize(writer, waivers);
            await writer.FlushAsync(ct);
        }

        private async Task WriteFeedbackAsync(Utf8JsonWriter writer, Guid userId, CancellationToken ct)
        {
            var feedback = await context.UserFeedback
                .AsNoTracking()
                .Where(uf => uf.UserId == userId)
                .Select(uf => new ExportedFeedback(
                    uf.Category,
                    uf.Description,
                    uf.Status,
                    uf.CreatedDate))
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync(ct);

            writer.WritePropertyName("feedback");
            JsonSerializer.Serialize(writer, feedback);
            await writer.FlushAsync(ct);
        }

        private async Task WritePartnerAdminRolesAsync(Utf8JsonWriter writer, Guid userId, CancellationToken ct)
        {
            var roles = await context.PartnerAdmins
                .AsNoTracking()
                .Where(pa => pa.UserId == userId)
                .Join(context.Partners, pa => pa.PartnerId, p => p.Id,
                    (pa, p) => new { pa, p })
                .Select(x => new ExportedPartnerAdminRole(
                    x.p.Id,
                    x.p.Name,
                    x.pa.CreatedDate))
                .ToListAsync(ct);

            writer.WritePropertyName("partnerAdminRoles");
            JsonSerializer.Serialize(writer, roles);
            await writer.FlushAsync(ct);
        }

        private async Task WriteNotificationPreferencesAsync(Utf8JsonWriter writer, Guid userId, CancellationToken ct)
        {
            var prefs = await context.UserNewsletterPreferences
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .Select(p => new ExportedNewsletterPreference(
                    p.CategoryId,
                    p.IsSubscribed))
                .ToListAsync(ct);

            writer.WritePropertyName("notificationPreferences");
            JsonSerializer.Serialize(writer, prefs);
            await writer.FlushAsync(ct);
        }
    }
}
