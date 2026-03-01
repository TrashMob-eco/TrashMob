namespace TrashMob.Shared.Managers.Contacts
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Computes engagement scores, donor lifecycle stages, and fundraising dashboard metrics.
    /// All data is computed on-the-fly — no stored scores.
    /// </summary>
    public class FundraisingAnalyticsManager(
        IKeyedRepository<Contact> contactRepository,
        IKeyedRepository<Donation> donationRepository,
        IKeyedRepository<ContactNote> contactNoteRepository,
        IKeyedRepository<Grant> grantRepository,
        IBaseRepository<EventAttendee> eventAttendeeRepository,
        IKeyedRepository<EventAttendeeMetrics> eventAttendeeMetricsRepository)
        : IFundraisingAnalyticsManager
    {
        private static readonly string[] GrantStatusLabels =
            ["", "Prospect", "LOI Submitted", "Application Submitted", "Awarded", "Declined", "Reporting", "Renewal", "Closed"];

        public async Task<IEnumerable<ContactEngagementScore>> GetEngagementScoresAsync(
            CancellationToken cancellationToken = default)
        {
            return await ComputeAllScoresAsync(cancellationToken);
        }

        public async Task<ContactEngagementScore> GetEngagementScoreAsync(
            Guid contactId, CancellationToken cancellationToken = default)
        {
            var allScores = await ComputeAllScoresAsync(cancellationToken);
            return allScores.FirstOrDefault(s => s.ContactId == contactId)
                ?? new ContactEngagementScore
                {
                    ContactId = contactId,
                    DonorLifecycleStage = "Prospect",
                };
        }

        public async Task<IEnumerable<ContactEngagementScore>> GetVolunteerToDonorPipelineAsync(
            CancellationToken cancellationToken = default)
        {
            var allScores = await ComputeAllScoresAsync(cancellationToken);
            return allScores
                .Where(s => s.DonationCount == 0 && s.EngagementScore >= 20)
                .OrderByDescending(s => s.EngagementScore);
        }

        public async Task<IEnumerable<ContactEngagementScore>> GetLybuntContactsAsync(
            CancellationToken cancellationToken = default)
        {
            var allScores = await ComputeAllScoresAsync(cancellationToken);
            return allScores
                .Where(s => s.IsLybunt)
                .OrderByDescending(s => s.TotalDonations);
        }

        public async Task<FundraisingDashboard> GetDashboardAsync(
            CancellationToken cancellationToken = default)
        {
            var dashboard = new FundraisingDashboard();
            var now = DateTimeOffset.UtcNow;
            var ytdStart = new DateTimeOffset(now.Year, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var lastYearStart = new DateTimeOffset(now.Year - 1, 1, 1, 0, 0, 0, TimeSpan.Zero);

            // YTD donation aggregations
            var ytdDonations = await donationRepository.Get()
                .Where(d => d.DonationDate >= ytdStart)
                .ToListAsync(cancellationToken);

            dashboard.TotalRaisedYtd = ytdDonations.Sum(d => d.Amount);
            dashboard.DonationCountYtd = ytdDonations.Count;
            dashboard.DonorCountYtd = ytdDonations.Select(d => d.ContactId).Distinct().Count();
            dashboard.AverageGiftSizeYtd = ytdDonations.Count > 0
                ? Math.Round(ytdDonations.Average(d => d.Amount), 2)
                : 0;

            // Last year total
            var lastYearDonations = await donationRepository.Get()
                .Where(d => d.DonationDate >= lastYearStart && d.DonationDate < ytdStart)
                .ToListAsync(cancellationToken);

            dashboard.TotalRaisedLastYear = lastYearDonations.Sum(d => d.Amount);

            // Donor retention: donors who gave last year AND this year
            var lastYearDonorIds = lastYearDonations.Select(d => d.ContactId).Distinct().ToHashSet();
            var ytdDonorIds = ytdDonations.Select(d => d.ContactId).Distinct().ToHashSet();

            if (lastYearDonorIds.Count > 0)
            {
                var retainedCount = ytdDonorIds.Intersect(lastYearDonorIds).Count();
                dashboard.RetentionRate = Math.Round((double)retainedCount / lastYearDonorIds.Count * 100, 1);
            }

            // New vs repeat donors YTD
            var allDonorIdsBeforeYtd = await donationRepository.Get()
                .Where(d => d.DonationDate < ytdStart)
                .Select(d => d.ContactId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var priorDonorSet = allDonorIdsBeforeYtd.ToHashSet();
            dashboard.NewDonorsYtd = ytdDonorIds.Count(id => !priorDonorSet.Contains(id));
            dashboard.RepeatDonorsYtd = ytdDonorIds.Count(id => priorDonorSet.Contains(id));

            // Lifecycle breakdown from engagement scores
            var scores = await ComputeAllScoresAsync(cancellationToken);
            var lifecycleGroups = scores
                .GroupBy(s => s.DonorLifecycleStage)
                .Select(g => new DonorLifecycleStat { Stage = g.Key, Count = g.Count() })
                .OrderBy(s => GetLifecycleOrder(s.Stage))
                .ToList();

            dashboard.LifecycleBreakdown = lifecycleGroups;
            dashboard.LapsedDonors = scores.Count(s => s.DonorLifecycleStage == "Lapsed");
            dashboard.LybuntCount = scores.Count(s => s.IsLybunt);

            // Monthly giving (last 12 months)
            var twelveMonthsAgo = now.AddMonths(-12);
            dashboard.MonthlyGiving = await donationRepository.Get()
                .Where(d => d.DonationDate >= twelveMonthsAgo)
                .GroupBy(d => new { d.DonationDate.Year, d.DonationDate.Month })
                .Select(g => new MonthlyGivingStat
                {
                    Month = g.Key.Year + "-" + (g.Key.Month < 10 ? "0" : "") + g.Key.Month,
                    Amount = g.Sum(d => d.Amount),
                    DonationCount = g.Count(),
                })
                .OrderBy(m => m.Month)
                .ToListAsync(cancellationToken);

            // Campaign breakdown
            dashboard.CampaignBreakdown = await donationRepository.Get()
                .GroupBy(d => d.Campaign ?? "Uncategorized")
                .Select(g => new CampaignStat
                {
                    Campaign = g.Key,
                    TotalRaised = g.Sum(d => d.Amount),
                    DonorCount = g.Select(d => d.ContactId).Distinct().Count(),
                    DonationCount = g.Count(),
                })
                .OrderByDescending(c => c.TotalRaised)
                .ToListAsync(cancellationToken);

            // Grant pipeline
            var activeStatuses = new[] { 1, 2, 3, 4, 6, 7 };
            var pendingStatuses = new[] { 1, 2, 3 };

            var grantGroups = await grantRepository.Get()
                .GroupBy(g => g.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count(),
                    TotalAmount = g.Sum(gr => gr.AmountAwarded ?? gr.AmountMax ?? 0),
                })
                .ToListAsync(cancellationToken);

            dashboard.GrantPipeline = grantGroups
                .Where(g => g.Status >= 1 && g.Status <= 8)
                .Select(g => new GrantPipelineStat
                {
                    Status = g.Status,
                    Label = g.Status >= 1 && g.Status < GrantStatusLabels.Length ? GrantStatusLabels[g.Status] : "Unknown",
                    Count = g.Count,
                    TotalAmount = g.TotalAmount,
                })
                .OrderBy(g => g.Status)
                .ToList();

            dashboard.ActiveGrantCount = grantGroups.Where(g => activeStatuses.Contains(g.Status)).Sum(g => g.Count);
            dashboard.TotalGrantsAwarded = grantGroups.Where(g => g.Status == 4).Sum(g => g.TotalAmount);
            dashboard.TotalGrantsPending = grantGroups.Where(g => pendingStatuses.Contains(g.Status)).Sum(g => g.TotalAmount);

            // Upcoming grant deadlines (next 30 days)
            var thirtyDaysFromNow = now.AddDays(30);
            dashboard.UpcomingDeadlineCount = await grantRepository.Get()
                .CountAsync(g => g.SubmissionDeadline.HasValue
                    && g.SubmissionDeadline.Value >= now
                    && g.SubmissionDeadline.Value <= thirtyDaysFromNow, cancellationToken);

            return dashboard;
        }

        public async Task<string> GenerateDonorReportCsvAsync(CancellationToken cancellationToken = default)
        {
            var scores = await ComputeAllScoresAsync(cancellationToken);
            var sb = new StringBuilder();
            sb.AppendLine("Name,Email,Type,Engagement Score,Lifecycle Stage,Total Donations,Donation Count,Last Donation,Events Attended,Volunteer Hours,Notes,Last Interaction,LYBUNT");

            foreach (var s in scores.OrderByDescending(s => s.EngagementScore))
            {
                sb.AppendLine(string.Join(",",
                    EscapeCsvField(s.ContactName),
                    EscapeCsvField(s.Email),
                    s.ContactType,
                    s.EngagementScore,
                    EscapeCsvField(s.DonorLifecycleStage),
                    s.TotalDonations.ToString("F2", CultureInfo.InvariantCulture),
                    s.DonationCount,
                    s.LastDonationDate?.ToString("yyyy-MM-dd") ?? "",
                    s.EventsAttended,
                    Math.Round(s.TotalVolunteerMinutes / 60.0, 1).ToString(CultureInfo.InvariantCulture),
                    s.NoteCount,
                    s.LastInteractionDate?.ToString("yyyy-MM-dd") ?? "",
                    s.IsLybunt ? "Yes" : "No"));
            }

            return sb.ToString();
        }

        public async Task<string> GenerateFundraisingSummaryCsvAsync(CancellationToken cancellationToken = default)
        {
            var dashboard = await GetDashboardAsync(cancellationToken);
            var sb = new StringBuilder();

            sb.AppendLine("Metric,Value");
            sb.AppendLine($"Total Raised YTD,{dashboard.TotalRaisedYtd:F2}");
            sb.AppendLine($"Total Raised Last Year,{dashboard.TotalRaisedLastYear:F2}");
            sb.AppendLine($"Donor Count YTD,{dashboard.DonorCountYtd}");
            sb.AppendLine($"Average Gift Size,{dashboard.AverageGiftSizeYtd:F2}");
            sb.AppendLine($"Donation Count YTD,{dashboard.DonationCountYtd}");
            sb.AppendLine($"Retention Rate,{dashboard.RetentionRate:F1}%");
            sb.AppendLine($"New Donors YTD,{dashboard.NewDonorsYtd}");
            sb.AppendLine($"Repeat Donors YTD,{dashboard.RepeatDonorsYtd}");
            sb.AppendLine($"Lapsed Donors,{dashboard.LapsedDonors}");
            sb.AppendLine($"LYBUNT Count,{dashboard.LybuntCount}");
            sb.AppendLine($"Grants Awarded Total,{dashboard.TotalGrantsAwarded:F2}");
            sb.AppendLine($"Grants Pending Total,{dashboard.TotalGrantsPending:F2}");
            sb.AppendLine($"Active Grant Count,{dashboard.ActiveGrantCount}");

            if (dashboard.MonthlyGiving.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Month,Amount,Donations");
                foreach (var m in dashboard.MonthlyGiving)
                {
                    sb.AppendLine($"{m.Month},{m.Amount:F2},{m.DonationCount}");
                }
            }

            if (dashboard.CampaignBreakdown.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Campaign,Total Raised,Donors,Donations");
                foreach (var c in dashboard.CampaignBreakdown)
                {
                    sb.AppendLine($"{EscapeCsvField(c.Campaign)},{c.TotalRaised:F2},{c.DonorCount},{c.DonationCount}");
                }
            }

            return sb.ToString();
        }

        private async Task<List<ContactEngagementScore>> ComputeAllScoresAsync(
            CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;
            var oneYearAgo = now.AddYears(-1);
            var thisYearStart = new DateTimeOffset(now.Year, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var lastYearStart = new DateTimeOffset(now.Year - 1, 1, 1, 0, 0, 0, TimeSpan.Zero);

            // Load all active contacts
            var contacts = await contactRepository.Get()
                .Where(c => c.IsActive)
                .Select(c => new
                {
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    c.Email,
                    c.ContactType,
                    c.UserId,
                })
                .ToListAsync(cancellationToken);

            // Load donation aggregates per contact
            var donationAgg = await donationRepository.Get()
                .GroupBy(d => d.ContactId)
                .Select(g => new
                {
                    ContactId = g.Key,
                    TotalAmount = g.Sum(d => d.Amount),
                    Count = g.Count(),
                    LastDate = g.Max(d => d.DonationDate),
                })
                .ToListAsync(cancellationToken);

            var donationMap = donationAgg.ToDictionary(d => d.ContactId);

            // Check which contacts donated this year vs last year (for LYBUNT)
            var donorsLastYear = await donationRepository.Get()
                .Where(d => d.DonationDate >= lastYearStart && d.DonationDate < thisYearStart)
                .Select(d => d.ContactId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var donorsThisYear = await donationRepository.Get()
                .Where(d => d.DonationDate >= thisYearStart)
                .Select(d => d.ContactId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var donorsLastYearSet = donorsLastYear.ToHashSet();
            var donorsThisYearSet = donorsThisYear.ToHashSet();

            // Load event attendance counts per user
            var userIds = contacts
                .Where(c => c.UserId.HasValue)
                .Select(c => c.UserId!.Value)
                .Distinct()
                .ToList();

            var attendanceCounts = new Dictionary<Guid, int>();
            var volunteerMinutes = new Dictionary<Guid, int>();

            if (userIds.Count > 0)
            {
                attendanceCounts = await eventAttendeeRepository.Get()
                    .Where(ea => userIds.Contains(ea.UserId))
                    .GroupBy(ea => ea.UserId)
                    .Select(g => new { UserId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(g => g.UserId, g => g.Count, cancellationToken);

                // Volunteer duration from approved/adjusted metrics
                var approvedStatuses = new[] { "Approved", "Adjusted" };
                volunteerMinutes = await eventAttendeeMetricsRepository.Get()
                    .Where(m => userIds.Contains(m.UserId) && approvedStatuses.Contains(m.Status))
                    .GroupBy(m => m.UserId)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        TotalMinutes = g.Sum(m => m.AdjustedDurationMinutes ?? m.DurationMinutes ?? 0),
                    })
                    .ToDictionaryAsync(g => g.UserId, g => g.TotalMinutes, cancellationToken);
            }

            // Load note aggregates per contact
            var noteAgg = await contactNoteRepository.Get()
                .GroupBy(n => n.ContactId)
                .Select(g => new
                {
                    ContactId = g.Key,
                    Count = g.Count(),
                    LastDate = g.Max(n => n.CreatedDate),
                })
                .ToListAsync(cancellationToken);

            var noteMap = noteAgg.ToDictionary(n => n.ContactId);

            // Compute scores
            var results = new List<ContactEngagementScore>(contacts.Count);

            foreach (var contact in contacts)
            {
                var score = new ContactEngagementScore
                {
                    ContactId = contact.Id,
                    ContactName = $"{contact.FirstName} {contact.LastName}".Trim(),
                    Email = contact.Email ?? string.Empty,
                    ContactType = contact.ContactType,
                    HasUserId = contact.UserId.HasValue,
                };

                // Donation data
                if (donationMap.TryGetValue(contact.Id, out var donData))
                {
                    score.TotalDonations = donData.TotalAmount;
                    score.DonationCount = donData.Count;
                    score.LastDonationDate = donData.LastDate;
                }

                // Volunteer data
                if (contact.UserId.HasValue)
                {
                    score.EventsAttended = attendanceCounts.GetValueOrDefault(contact.UserId.Value, 0);
                    score.TotalVolunteerMinutes = volunteerMinutes.GetValueOrDefault(contact.UserId.Value, 0);
                }

                // Note data
                if (noteMap.TryGetValue(contact.Id, out var noteData))
                {
                    score.NoteCount = noteData.Count;
                    score.LastInteractionDate = noteData.LastDate;
                }

                // LYBUNT detection
                score.IsLybunt = donorsLastYearSet.Contains(contact.Id) && !donorsThisYearSet.Contains(contact.Id);

                // Compute engagement score components
                ComputeScoreComponents(score, now, oneYearAgo);

                // Compute donor lifecycle stage
                score.DonorLifecycleStage = ComputeLifecycleStage(score, oneYearAgo);

                results.Add(score);
            }

            return results.OrderByDescending(s => s.EngagementScore).ToList();
        }

        private static void ComputeScoreComponents(ContactEngagementScore score, DateTimeOffset now, DateTimeOffset oneYearAgo)
        {
            // Donation component (raw 0–40)
            var donationScore = 0;
            if (score.DonationCount > 0)
            {
                // Recency (0–15)
                if (score.LastDonationDate.HasValue)
                {
                    var daysSinceLastDonation = (now - score.LastDonationDate.Value).TotalDays;
                    donationScore += daysSinceLastDonation <= 90 ? 15 : daysSinceLastDonation <= 180 ? 10 : daysSinceLastDonation <= 365 ? 5 : 0;
                }

                // Frequency (0–15)
                donationScore += score.DonationCount >= 5 ? 15 : score.DonationCount >= 3 ? 10 : score.DonationCount >= 2 ? 5 : 3;

                // Amount (0–10)
                donationScore += score.TotalDonations >= 5000 ? 10 : score.TotalDonations >= 1000 ? 7 : score.TotalDonations >= 250 ? 4 : 2;
            }

            // Volunteer component (raw 0–40)
            var volunteerScore = 0;
            if (score.HasUserId)
            {
                // Events attended (0–20)
                volunteerScore += score.EventsAttended >= 10 ? 20 : score.EventsAttended >= 5 ? 15 : score.EventsAttended >= 3 ? 10 : score.EventsAttended >= 1 ? 5 : 0;

                // Volunteer hours (0–20)
                var hours = score.TotalVolunteerMinutes / 60.0;
                volunteerScore += hours >= 20 ? 20 : hours >= 10 ? 15 : hours >= 5 ? 10 : hours > 0 ? 5 : 0;
            }

            // Interaction component (raw 0–20)
            var interactionScore = 0;
            if (score.NoteCount > 0)
            {
                // Note count (0–10)
                interactionScore += score.NoteCount >= 10 ? 10 : score.NoteCount >= 5 ? 7 : 3;

                // Last note recency (0–10)
                if (score.LastInteractionDate.HasValue)
                {
                    var daysSinceLastNote = (now - score.LastInteractionDate.Value).TotalDays;
                    interactionScore += daysSinceLastNote <= 30 ? 10 : daysSinceLastNote <= 90 ? 7 : daysSinceLastNote <= 180 ? 4 : daysSinceLastNote <= 365 ? 2 : 0;
                }
            }

            // Score redistribution when no UserId (volunteer data unavailable)
            if (!score.HasUserId)
            {
                // Redistribute 40 volunteer points proportionally: donation gets 57/100, interaction gets 43/100
                var rawDonation = donationScore;
                var rawInteraction = interactionScore;

                // Scale donation from 0-40 to 0-57
                donationScore = rawDonation > 0 ? (int)Math.Round(rawDonation * 57.0 / 40.0) : 0;
                // Scale interaction from 0-20 to 0-43
                interactionScore = rawInteraction > 0 ? (int)Math.Round(rawInteraction * 43.0 / 20.0) : 0;
            }

            score.DonationScoreComponent = donationScore;
            score.VolunteerScoreComponent = volunteerScore;
            score.InteractionScoreComponent = interactionScore;
            score.EngagementScore = Math.Min(100, donationScore + volunteerScore + interactionScore);
        }

        private static string ComputeLifecycleStage(ContactEngagementScore score, DateTimeOffset oneYearAgo)
        {
            if (score.DonationCount == 0)
            {
                return "Prospect";
            }

            // Lapsed overrides other stages
            if (score.LastDonationDate.HasValue && score.LastDonationDate.Value < oneYearAgo)
            {
                return "Lapsed";
            }

            if (score.TotalDonations >= 5000)
            {
                return "Major Donor";
            }

            if (score.DonationCount >= 2)
            {
                return "Repeat Donor";
            }

            return "First-Time Donor";
        }

        private static int GetLifecycleOrder(string stage) => stage switch
        {
            "Prospect" => 0,
            "First-Time Donor" => 1,
            "Repeat Donor" => 2,
            "Major Donor" => 3,
            "Lapsed" => 4,
            _ => 5,
        };

        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return string.Empty;
            }

            if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }
    }
}
