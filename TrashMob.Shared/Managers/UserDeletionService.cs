namespace TrashMob.Shared.Managers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;

    /// <summary>
    /// Handles comprehensive user data deletion and anonymization across all tables.
    /// Uses ExecuteUpdateAsync/ExecuteDeleteAsync for efficient bulk operations
    /// and wraps everything in a transaction for atomicity.
    /// </summary>
    public class UserDeletionService(MobDbContext context, ILogger<UserDeletionService> logger) : IUserDeletionService
    {
        private static readonly Guid AnonymousUserId = Guid.Empty;

        /// <inheritdoc />
        public async Task<int> DeleteUserDataAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
            {
                return 0;
            }

            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                logger.LogInformation("Starting user data deletion for user {UserId}", userId);

                // Phase A: Delete user-specific records with no historical value
                await DeleteUserSpecificRecordsAsync(userId, cancellationToken);

                // Phase B: Anonymize records with required UserId (preserve for history/metrics)
                await AnonymizeRequiredUserIdFieldsAsync(userId, cancellationToken);

                // Phase C: Anonymize photo upload references
                await AnonymizePhotoReferencesAsync(userId, cancellationToken);

                // Phase D: Clean up nullable user references
                await CleanUpNullableUserReferencesAsync(userId, cancellationToken);

                // Phase E: Handle waivers (anonymize, don't delete)
                await AnonymizeWaiverRecordsAsync(userId, cancellationToken);

                // Phase F: Anonymize audit fields across all BaseModel entities
                await AnonymizeAllAuditFieldsAsync(userId, cancellationToken);

                // Phase G: Handle team lead transfer before cascade deletes membership
                await HandleTeamLeadTransferAsync(userId, cancellationToken);

                // Phase H: Delete the user record (must be last)
                context.Users.Remove(user);
                var result = await context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                logger.LogInformation("Successfully deleted all data for user {UserId}", userId);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete data for user {UserId}. Rolling back transaction", userId);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        /// <summary>
        /// Phase A: Delete user-specific records that have no historical value.
        /// </summary>
        private async Task DeleteUserSpecificRecordsAsync(Guid userId, CancellationToken ct)
        {
            // Event attendance
            await context.EventAttendees
                .Where(ea => ea.UserId == userId)
                .ExecuteDeleteAsync(ct);

            // Notifications
            await context.UserNotifications
                .Where(un => un.UserId == userId)
                .ExecuteDeleteAsync(ct);

            await context.NonEventUserNotifications
                .Where(n => n.UserId == userId)
                .ExecuteDeleteAsync(ct);

            // IFTTT triggers (user-specific)
            await context.IftttTriggers
                .Where(t => t.UserId == userId)
                .ExecuteDeleteAsync(ct);

            // Professional company membership
            await context.ProfessionalCompanyUsers
                .Where(pcu => pcu.UserId == userId)
                .ExecuteDeleteAsync(ct);

            // Partner admin membership (delete where user is the admin)
            await context.PartnerAdmins
                .Where(pa => pa.UserId == userId)
                .ExecuteDeleteAsync(ct);
        }

        /// <summary>
        /// Phase B: Anonymize records with required UserId fields to preserve historical data.
        /// </summary>
        private async Task AnonymizeRequiredUserIdFieldsAsync(Guid userId, CancellationToken ct)
        {
            // EventAttendeeRoute: preserve route data for community heatmaps
            await context.EventAttendeeRoutes
                .Where(r => r.UserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(r => r.UserId, AnonymousUserId),
                    ct);

            // EventAttendeeMetrics: preserve for event-level aggregate totals
            await context.EventAttendeeMetrics
                .Where(m => m.UserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(m => m.UserId, AnonymousUserId)
                    .SetProperty(m => m.ReviewedByUserId, m => m.ReviewedByUserId == userId ? null : m.ReviewedByUserId),
                    ct);

            // PhotoFlag: preserve moderation audit trail
            await context.PhotoFlags
                .Where(f => f.FlaggedByUserId == userId || f.ResolvedByUserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(f => f.FlaggedByUserId, f => f.FlaggedByUserId == userId ? AnonymousUserId : f.FlaggedByUserId)
                    .SetProperty(f => f.ResolvedByUserId, f => f.ResolvedByUserId == userId ? null : f.ResolvedByUserId),
                    ct);

            // PhotoModerationLog: preserve moderation audit trail
            await context.PhotoModerationLogs
                .Where(l => l.PerformedByUserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(l => l.PerformedByUserId, AnonymousUserId),
                    ct);

            // EmailInviteBatch: preserve invite history
            await context.EmailInviteBatches
                .Where(b => b.SenderUserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.SenderUserId, AnonymousUserId),
                    ct);
        }

        /// <summary>
        /// Phase C: Anonymize photo upload references across all photo entity types.
        /// </summary>
        private async Task AnonymizePhotoReferencesAsync(Guid userId, CancellationToken ct)
        {
            // EventPhoto
            await context.EventPhotos
                .Where(p => p.UploadedByUserId == userId || p.ReviewRequestedByUserId == userId || p.ModeratedByUserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.UploadedByUserId, p => p.UploadedByUserId == userId ? AnonymousUserId : p.UploadedByUserId)
                    .SetProperty(p => p.ReviewRequestedByUserId, p => p.ReviewRequestedByUserId == userId ? null : p.ReviewRequestedByUserId)
                    .SetProperty(p => p.ModeratedByUserId, p => p.ModeratedByUserId == userId ? null : p.ModeratedByUserId),
                    ct);

            // PartnerPhoto
            await context.PartnerPhotos
                .Where(p => p.UploadedByUserId == userId || p.ReviewRequestedByUserId == userId || p.ModeratedByUserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.UploadedByUserId, p => p.UploadedByUserId == userId ? AnonymousUserId : p.UploadedByUserId)
                    .SetProperty(p => p.ReviewRequestedByUserId, p => p.ReviewRequestedByUserId == userId ? null : p.ReviewRequestedByUserId)
                    .SetProperty(p => p.ModeratedByUserId, p => p.ModeratedByUserId == userId ? null : p.ModeratedByUserId),
                    ct);

            // TeamPhoto
            await context.TeamPhotos
                .Where(p => p.UploadedByUserId == userId || p.ReviewRequestedByUserId == userId || p.ModeratedByUserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.UploadedByUserId, p => p.UploadedByUserId == userId ? AnonymousUserId : p.UploadedByUserId)
                    .SetProperty(p => p.ReviewRequestedByUserId, p => p.ReviewRequestedByUserId == userId ? null : p.ReviewRequestedByUserId)
                    .SetProperty(p => p.ModeratedByUserId, p => p.ModeratedByUserId == userId ? null : p.ModeratedByUserId),
                    ct);
        }

        /// <summary>
        /// Phase D: Clean up nullable user references.
        /// </summary>
        private async Task CleanUpNullableUserReferencesAsync(Guid userId, CancellationToken ct)
        {
            // UserFeedback (UserId is nullable)
            await context.UserFeedback
                .Where(f => f.UserId == userId || f.ReviewedByUserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(f => f.UserId, f => f.UserId == userId ? null : f.UserId)
                    .SetProperty(f => f.ReviewedByUserId, f => f.ReviewedByUserId == userId ? null : f.ReviewedByUserId),
                    ct);

            // EmailInvite (SignedUpUserId is nullable)
            await context.EmailInvites
                .Where(i => i.SignedUpUserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(i => i.SignedUpUserId, (Guid?)null),
                    ct);

            // TeamAdoption (ReviewedByUserId is nullable)
            await context.TeamAdoptions
                .Where(a => a.ReviewedByUserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(a => a.ReviewedByUserId, (Guid?)null),
                    ct);

            // TeamJoinRequest (ReviewedByUserId is nullable - UserId has Cascade so auto-handled)
            await context.TeamJoinRequests
                .Where(r => r.ReviewedByUserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(r => r.ReviewedByUserId, (Guid?)null),
                    ct);

            // LitterImage (ReviewRequestedByUserId and ModeratedByUserId are nullable)
            await context.LitterImages
                .Where(i => i.ReviewRequestedByUserId == userId || i.ModeratedByUserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(i => i.ReviewRequestedByUserId, i => i.ReviewRequestedByUserId == userId ? null : i.ReviewRequestedByUserId)
                    .SetProperty(i => i.ModeratedByUserId, i => i.ModeratedByUserId == userId ? null : i.ModeratedByUserId),
                    ct);
        }

        /// <summary>
        /// Phase E: Handle waivers — anonymize user reference, retain record for legal compliance.
        /// UserWaiver cascade behavior must be changed from Cascade to NoAction before this works.
        /// </summary>
        private async Task AnonymizeWaiverRecordsAsync(Guid userId, CancellationToken ct)
        {
            await context.UserWaivers
                .Where(w => w.UserId == userId || w.UploadedByUserId == userId || w.GuardianUserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(w => w.UserId, w => w.UserId == userId ? AnonymousUserId : w.UserId)
                    .SetProperty(w => w.UploadedByUserId, w => w.UploadedByUserId == userId ? null : w.UploadedByUserId)
                    .SetProperty(w => w.GuardianUserId, w => w.GuardianUserId == userId ? null : w.GuardianUserId),
                    ct);
        }

        /// <summary>
        /// Phase F: Anonymize CreatedByUserId/LastUpdatedByUserId audit fields across all entities.
        /// Uses a generic helper since all entities inherit from BaseModel.
        /// </summary>
        private async Task AnonymizeAllAuditFieldsAsync(Guid userId, CancellationToken ct)
        {
            // Entities already handled in earlier phases for specific UserId fields
            // still need audit field anonymization for records where user is only in audit fields
            await AnonymizeAuditFieldsAsync<EventAttendee>(userId, ct);
            await AnonymizeAuditFieldsAsync<EventAttendeeRoute>(userId, ct);
            await AnonymizeAuditFieldsAsync<EventAttendeeMetrics>(userId, ct);
            await AnonymizeAuditFieldsAsync<PhotoFlag>(userId, ct);
            await AnonymizeAuditFieldsAsync<PhotoModerationLog>(userId, ct);
            await AnonymizeAuditFieldsAsync<EmailInviteBatch>(userId, ct);
            await AnonymizeAuditFieldsAsync<EmailInvite>(userId, ct);
            await AnonymizeAuditFieldsAsync<EventPhoto>(userId, ct);
            await AnonymizeAuditFieldsAsync<PartnerPhoto>(userId, ct);
            await AnonymizeAuditFieldsAsync<TeamPhoto>(userId, ct);
            await AnonymizeAuditFieldsAsync<UserFeedback>(userId, ct);
            await AnonymizeAuditFieldsAsync<UserWaiver>(userId, ct);
            await AnonymizeAuditFieldsAsync<TeamAdoption>(userId, ct);
            await AnonymizeAuditFieldsAsync<TeamJoinRequest>(userId, ct);
            await AnonymizeAuditFieldsAsync<ProfessionalCompanyUser>(userId, ct);
            await AnonymizeAuditFieldsAsync<IftttTrigger>(userId, ct);

            // Entities from the original UserManager.DeleteAsync that were already handled
            await AnonymizeAuditFieldsAsync<PartnerRequest>(userId, ct);
            await AnonymizeAuditFieldsAsync<EventSummary>(userId, ct);
            await AnonymizeAuditFieldsAsync<EventPartnerLocationService>(userId, ct);
            await AnonymizeAuditFieldsAsync<Event>(userId, ct);
            await AnonymizeAuditFieldsAsync<Partner>(userId, ct);
            await AnonymizeAuditFieldsAsync<PartnerAdmin>(userId, ct);
            await AnonymizeAuditFieldsAsync<PartnerLocation>(userId, ct);

            // Entities missing from the original flow
            await AnonymizeAuditFieldsAsync<ContactRequest>(userId, ct);
            await AnonymizeAuditFieldsAsync<JobOpportunity>(userId, ct);
            await AnonymizeAuditFieldsAsync<PartnerDocument>(userId, ct);
            await AnonymizeAuditFieldsAsync<PartnerContact>(userId, ct);
            await AnonymizeAuditFieldsAsync<PartnerLocationContact>(userId, ct);
            await AnonymizeAuditFieldsAsync<Models.LitterReport>(userId, ct);
            await AnonymizeAuditFieldsAsync<LitterImage>(userId, ct);
            await AnonymizeAuditFieldsAsync<EventLitterReport>(userId, ct);
            await AnonymizeAuditFieldsAsync<Team>(userId, ct);
            await AnonymizeAuditFieldsAsync<PickupLocation>(userId, ct);
            await AnonymizeAuditFieldsAsync<TeamAdoptionEvent>(userId, ct);
            await AnonymizeAuditFieldsAsync<PartnerSocialMediaAccount>(userId, ct);
            await AnonymizeAuditFieldsAsync<SponsoredAdoption>(userId, ct);
            await AnonymizeAuditFieldsAsync<ProfessionalCleanupLog>(userId, ct);
            await AnonymizeAuditFieldsAsync<AdoptableArea>(userId, ct);
            await AnonymizeAuditFieldsAsync<AreaGenerationBatch>(userId, ct);
            await AnonymizeAuditFieldsAsync<StagedAdoptableArea>(userId, ct);
            await AnonymizeAuditFieldsAsync<TeamMember>(userId, ct);
            await AnonymizeAuditFieldsAsync<TeamEvent>(userId, ct);
            await AnonymizeAuditFieldsAsync<UserNotification>(userId, ct);
            await AnonymizeAuditFieldsAsync<NonEventUserNotification>(userId, ct);
            await AnonymizeAuditFieldsAsync<PartnerLocationService>(userId, ct);
            await AnonymizeAuditFieldsAsync<UserAchievement>(userId, ct);
            await AnonymizeAuditFieldsAsync<MessageRequest>(userId, ct);
        }

        /// <summary>
        /// Phase G: Transfer team lead role before the user's TeamMember records are cascade-deleted.
        /// </summary>
        private async Task HandleTeamLeadTransferAsync(Guid userId, CancellationToken ct)
        {
            // Find teams where this user is the lead
            var teamLeadMemberships = await context.TeamMembers
                .Where(tm => tm.UserId == userId && tm.IsTeamLead)
                .ToListAsync(ct);

            foreach (var membership in teamLeadMemberships)
            {
                // Find the longest-tenured other member of the team
                var nextLead = await context.TeamMembers
                    .Where(tm => tm.TeamId == membership.TeamId && tm.UserId != userId)
                    .OrderBy(tm => tm.JoinedDate)
                    .FirstOrDefaultAsync(ct);

                if (nextLead != null)
                {
                    nextLead.IsTeamLead = true;
                    // SaveChangesAsync will be called as part of the user deletion
                }
                // If no other members, team will be left without a lead
                // (TeamMember cascade will delete this user's membership)
            }

            if (teamLeadMemberships.Count > 0)
            {
                await context.SaveChangesAsync(ct);
            }
        }

        /// <summary>
        /// Generic helper to anonymize CreatedByUserId and LastUpdatedByUserId audit fields.
        /// Works for any entity that inherits from BaseModel.
        /// </summary>
        private async Task AnonymizeAuditFieldsAsync<T>(Guid userId, CancellationToken ct)
            where T : BaseModel
        {
            await context.Set<T>()
                .Where(e => e.CreatedByUserId == userId || e.LastUpdatedByUserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(e => e.CreatedByUserId, e => e.CreatedByUserId == userId ? AnonymousUserId : e.CreatedByUserId)
                    .SetProperty(e => e.LastUpdatedByUserId, e => e.LastUpdatedByUserId == userId ? AnonymousUserId : e.LastUpdatedByUserId),
                    ct);
        }
    }
}
