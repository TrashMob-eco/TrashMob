namespace TrashMob.Shared.Managers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;

    /// <summary>
    /// Orchestrates PRIVO consent workflows for adult verification and parental consent.
    /// </summary>
    public class PrivoConsentManager(
        MobDbContext dbContext,
        IPrivoService privoService,
        IDependentInvitationManager dependentInvitationManager,
        IConfiguration configuration,
        ILogger<PrivoConsentManager> logger) : IPrivoConsentManager
    {
        private const int MinimumMinorAge = 13;
        private const int AdultAge = 18;

        /// <inheritdoc />
        public async Task<ParentalConsent> InitiateAdultVerificationAsync(
            Guid userId, CancellationToken cancellationToken)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                ?? throw new InvalidOperationException("User not found.");

            if (user.IsIdentityVerified)
            {
                throw new InvalidOperationException("User identity is already verified.");
            }

            // Step 1: Call PRIVO Section 2 — create the consent/verification request
            var privoResponse = await privoService.CreateAdultVerificationRequestAsync(user, cancellationToken)
                ?? throw new InvalidOperationException("Failed to create PRIVO verification request. Please try again.");

            // Step 2: Call PRIVO Section 3 — get the direct verification widget URL
            // This skips the consent pre-screens and goes directly to identity verification
            var redirectBaseUrl = configuration["Privo:RedirectBaseUrl"] ?? "https://www.trashmob.eco";
            var redirectUrl = redirectBaseUrl.TrimEnd('/') + "/privo/callback";
            var verificationUrl = await privoService.GetDirectVerificationUrlAsync(
                privoResponse.ConsentIdentifier, redirectUrl, cancellationToken);

            // Use the verification widget URL if available, otherwise fall back to consent URL
            var consentUrl = verificationUrl ?? privoResponse.ConsentUrl;

            // Store SiD on user
            user.PrivoSid = privoResponse.Sid;
            user.LastUpdatedDate = DateTimeOffset.UtcNow;

            // Create consent record
            var consent = new ParentalConsent
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PrivoConsentIdentifier = privoResponse.ConsentIdentifier,
                PrivoSid = privoResponse.Sid,
                PrivoGranterSid = privoResponse.GranterSid,
                ConsentType = ConsentType.AdultVerification,
                Status = ConsentStatus.Pending,
                ConsentUrl = consentUrl,
                CreatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = userId,
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };

            dbContext.ParentalConsents.Add(consent);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Adult verification initiated for User={UserId}, ConsentId={ConsentId}, PrivoSid={PrivoSid}, VerificationUrl={HasUrl}",
                userId, consent.Id, privoResponse.Sid, !string.IsNullOrEmpty(verificationUrl));

            return consent;
        }

        /// <inheritdoc />
        public async Task<ParentalConsent> InitiateParentChildConsentAsync(
            Guid parentUserId, Guid dependentId, CancellationToken cancellationToken)
        {
            var parent = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == parentUserId, cancellationToken)
                ?? throw new InvalidOperationException("Parent user not found.");

            if (!parent.IsIdentityVerified)
            {
                throw new InvalidOperationException("Parent must verify their identity before adding a 13-17 dependent.");
            }

            var dependent = await dbContext.Dependents.FirstOrDefaultAsync(
                    d => d.Id == dependentId && d.ParentUserId == parentUserId && d.IsActive, cancellationToken)
                ?? throw new InvalidOperationException("Dependent not found or does not belong to this parent.");

            var age = CalculateAge(dependent.DateOfBirth);
            if (age < MinimumMinorAge || age >= AdultAge)
            {
                throw new InvalidOperationException($"Dependent must be between {MinimumMinorAge} and {AdultAge - 1} years old for PRIVO consent.");
            }

            // Check for existing pending consent
            var existingConsent = await dbContext.ParentalConsents
                .FirstOrDefaultAsync(c => c.DependentId == dependentId && c.Status == ConsentStatus.Pending, cancellationToken);

            if (existingConsent != null)
            {
                throw new InvalidOperationException("A consent request is already pending for this dependent.");
            }

            // Call PRIVO Section 5
            var privoResponse = await privoService.CreateParentInitiatedChildConsentAsync(parent, dependent, cancellationToken)
                ?? throw new InvalidOperationException("Failed to create PRIVO consent request. Please try again.");

            // Store SiD on dependent
            dependent.PrivoSid = privoResponse.Sid;

            // Create consent record
            var consent = new ParentalConsent
            {
                Id = Guid.NewGuid(),
                UserId = parentUserId, // The adult initiating
                ParentUserId = parentUserId,
                DependentId = dependentId,
                PrivoConsentIdentifier = privoResponse.ConsentIdentifier,
                PrivoSid = privoResponse.Sid,
                PrivoGranterSid = parent.PrivoSid,
                ConsentType = ConsentType.ParentInitiatedChild,
                Status = ConsentStatus.Pending,
                ConsentUrl = privoResponse.ConsentUrl,
                CreatedByUserId = parentUserId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = parentUserId,
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };

            dbContext.ParentalConsents.Add(consent);

            // Link consent to dependent
            dependent.ParentalConsentId = consent.Id;

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Parent-child consent initiated: Parent={ParentUserId}, Dependent={DependentId}, ConsentId={ConsentId}",
                parentUserId, dependentId, consent.Id);

            return consent;
        }

        /// <inheritdoc />
        public async Task<ParentalConsent> InitiateChildConsentAsync(
            string childFirstName, string childEmail, DateOnly childBirthDate,
            string parentEmail, CancellationToken cancellationToken)
        {
            var age = CalculateAge(childBirthDate);
            if (age < MinimumMinorAge || age >= AdultAge)
            {
                throw new InvalidOperationException($"Child must be between {MinimumMinorAge} and {AdultAge - 1} years old.");
            }

            // Check if parent account exists
            var parent = await dbContext.Users.FirstOrDefaultAsync(
                u => u.Email == parentEmail, cancellationToken);

            if (parent == null)
            {
                // NO FLOW — parent must create account first
                logger.LogInformation(
                    "Child-initiated consent: parent email {ParentEmail} not found. Parent must create account.",
                    parentEmail);
                return null;
            }

            if (!parent.IsIdentityVerified)
            {
                logger.LogInformation(
                    "Child-initiated consent: parent {ParentUserId} exists but is not verified.",
                    parent.Id);
                throw new InvalidOperationException("Parent must verify their identity first.");
            }

            // Call PRIVO Section 6
            var privoResponse = await privoService.CreateChildInitiatedConsentAsync(
                    childFirstName, childEmail, childBirthDate, parentEmail, cancellationToken)
                ?? throw new InvalidOperationException("Failed to create PRIVO consent request. Please try again.");

            // Create consent record — no userId yet since child doesn't have an account
            var consent = new ParentalConsent
            {
                Id = Guid.NewGuid(),
                UserId = parent.Id, // Track under parent until child account exists
                ParentUserId = parent.Id,
                PrivoConsentIdentifier = privoResponse.ConsentIdentifier,
                PrivoSid = privoResponse.Sid,
                PrivoGranterSid = privoResponse.GranterSid ?? parent.PrivoSid,
                ConsentType = ConsentType.ChildInitiated,
                Status = ConsentStatus.Pending,
                CreatedByUserId = parent.Id,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = parent.Id,
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };

            dbContext.ParentalConsents.Add(consent);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Child-initiated consent created: Parent={ParentUserId}, Child={ChildEmail}, ConsentId={ConsentId}",
                parent.Id, childEmail, consent.Id);

            return consent;
        }

        /// <inheritdoc />
        public async Task ProcessWebhookAsync(
            PrivoWebhookPayload payload, CancellationToken cancellationToken)
        {
            if (payload.ConsentIdentifiers == null || payload.ConsentIdentifiers.Count == 0)
            {
                logger.LogWarning("PRIVO webhook received with no consent identifiers. WebhookId={WebhookId}", payload.Id);
                return;
            }

            var consentIdentifier = payload.ConsentIdentifiers[0];

            var consent = await dbContext.ParentalConsents
                .Include(c => c.Dependent)
                .FirstOrDefaultAsync(c => c.PrivoConsentIdentifier == consentIdentifier, cancellationToken);

            if (consent == null)
            {
                logger.LogWarning(
                    "PRIVO webhook: no matching consent record for identifier {ConsentIdentifier}. WebhookId={WebhookId}",
                    consentIdentifier, payload.Id);
                return;
            }

            // Idempotency: skip if already processed beyond Pending
            if (consent.Status != ConsentStatus.Pending)
            {
                logger.LogInformation(
                    "PRIVO webhook: consent {ConsentId} already in status {Status}, skipping. WebhookId={WebhookId}",
                    consent.Id, consent.Status, payload.Id);
                return;
            }

            var eventTypes = string.Join(",", payload.EventTypes);

            // Skip pure creation/notice events — poll PRIVO for anything else
            var isCreationOnly = payload.EventTypes.All(e =>
                e.Contains("created", StringComparison.OrdinalIgnoreCase) ||
                e.Contains("delivered", StringComparison.OrdinalIgnoreCase));

            if (isCreationOnly)
            {
                logger.LogInformation(
                    "PRIVO webhook: creation/notice event for ConsentId={ConsentId}, EventTypes={EventTypes}. No status change.",
                    consent.Id, eventTypes);
            }
            else
            {
                // For any substantive event (consent_updated, account_feature_updated, etc.),
                // poll PRIVO Section 7 to get the authoritative state
                logger.LogInformation(
                    "PRIVO webhook: substantive event for ConsentId={ConsentId}, EventTypes={EventTypes}. Polling PRIVO for status.",
                    consent.Id, eventTypes);

                var privoStatus = await privoService.GetConsentStatusAsync(
                    consent.PrivoConsentIdentifier, cancellationToken);

                logger.LogInformation(
                    "PRIVO webhook poll result: ConsentId={ConsentId}, PrivoStatus={PrivoStatus}",
                    consent.Id, privoStatus ?? "null");

                var statusLower = (privoStatus ?? string.Empty).ToLowerInvariant();

                if (statusLower.Contains("approved") || statusLower.Contains("completed") ||
                    statusLower.Contains("granted") || statusLower.Contains("verified"))
                {
                    consent.Status = ConsentStatus.Verified;
                    consent.VerifiedDate = DateTimeOffset.UtcNow;
                    consent.LastUpdatedDate = DateTimeOffset.UtcNow;

                    switch (consent.ConsentType)
                    {
                        case ConsentType.AdultVerification:
                            await ProcessAdultVerificationWebhookAsync(consent, cancellationToken);
                            break;

                        case ConsentType.ParentInitiatedChild:
                            await ProcessParentChildConsentWebhookAsync(consent, cancellationToken);
                            break;

                        case ConsentType.ChildInitiated:
                            logger.LogInformation("Child-initiated consent verified: ConsentId={ConsentId}", consent.Id);
                            break;
                    }
                }
                else if (statusLower.Contains("denied") || statusLower.Contains("rejected") ||
                         statusLower.Contains("declined"))
                {
                    consent.Status = ConsentStatus.Denied;
                    consent.LastUpdatedDate = DateTimeOffset.UtcNow;
                }
                else
                {
                    logger.LogInformation(
                        "PRIVO webhook: status still {PrivoStatus} for ConsentId={ConsentId}. No change.",
                        privoStatus, consent.Id);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "PRIVO webhook processed: ConsentId={ConsentId}, EventTypes={EventTypes}, Status={Status}, WebhookId={WebhookId}",
                consent.Id, eventTypes, consent.Status, payload.Id);
        }

        /// <inheritdoc />
        public async Task<ParentalConsent> GetConsentByUserIdAsync(
            Guid userId, CancellationToken cancellationToken)
        {
            return await dbContext.ParentalConsents
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedDate)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ParentalConsent> RefreshConsentStatusAsync(
            Guid userId, CancellationToken cancellationToken)
        {
            var consent = await dbContext.ParentalConsents
                .Where(c => c.UserId == userId && c.Status == ConsentStatus.Pending)
                .OrderByDescending(c => c.CreatedDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (consent == null || string.IsNullOrEmpty(consent.PrivoConsentIdentifier))
            {
                return consent;
            }

            // Poll PRIVO Section 7 for current status
            var privoStatus = await privoService.GetConsentStatusAsync(consent.PrivoConsentIdentifier, cancellationToken);

            logger.LogInformation(
                "PRIVO status poll: ConsentId={ConsentId}, PrivoStatus={PrivoStatus}",
                consent.Id, privoStatus ?? "null");

            if (string.IsNullOrEmpty(privoStatus))
            {
                return consent;
            }

            var statusLower = privoStatus.ToLowerInvariant();

            if (statusLower.Contains("approved") || statusLower.Contains("completed") ||
                statusLower.Contains("granted") || statusLower.Contains("verified"))
            {
                consent.Status = ConsentStatus.Verified;
                consent.VerifiedDate = DateTimeOffset.UtcNow;
                consent.LastUpdatedDate = DateTimeOffset.UtcNow;

                if (consent.ConsentType == ConsentType.AdultVerification)
                {
                    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == consent.UserId, cancellationToken);
                    if (user != null)
                    {
                        user.IsIdentityVerified = true;
                        user.IdentityVerifiedDate = DateTimeOffset.UtcNow;
                        user.LastUpdatedDate = DateTimeOffset.UtcNow;
                        logger.LogInformation("Adult identity verified via status poll: User={UserId}", user.Id);
                    }
                }

                await dbContext.SaveChangesAsync(cancellationToken);
            }
            else if (statusLower.Contains("denied") || statusLower.Contains("rejected") || statusLower.Contains("declined"))
            {
                consent.Status = ConsentStatus.Denied;
                consent.LastUpdatedDate = DateTimeOffset.UtcNow;
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return consent;
        }

        /// <inheritdoc />
        public async Task RevokeConsentAsync(
            Guid consentId, Guid requestingUserId, string reason, CancellationToken cancellationToken)
        {
            var consent = await dbContext.ParentalConsents
                .FirstOrDefaultAsync(c => c.Id == consentId, cancellationToken)
                ?? throw new InvalidOperationException("Consent record not found.");

            // Only the parent/user or an admin can revoke
            if (consent.UserId != requestingUserId && consent.ParentUserId != requestingUserId)
            {
                throw new UnauthorizedAccessException("Not authorized to revoke this consent.");
            }

            // Call PRIVO Section 9 if we have both SiDs
            if (!string.IsNullOrEmpty(consent.PrivoSid) && !string.IsNullOrEmpty(consent.PrivoGranterSid))
            {
                await privoService.RevokeConsentAsync(consent.PrivoSid, consent.PrivoGranterSid, cancellationToken);
            }

            consent.Status = ConsentStatus.Revoked;
            consent.RevokedDate = DateTimeOffset.UtcNow;
            consent.RevokedReason = reason;
            consent.LastUpdatedByUserId = requestingUserId;
            consent.LastUpdatedDate = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Consent revoked: ConsentId={ConsentId}, RevokedBy={UserId}, Reason={Reason}",
                consentId, requestingUserId, reason);
        }

        #region Private Helpers

        private async Task ProcessAdultVerificationWebhookAsync(
            ParentalConsent consent, CancellationToken cancellationToken)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == consent.UserId, cancellationToken);
            if (user != null)
            {
                user.IsIdentityVerified = true;
                user.IdentityVerifiedDate = DateTimeOffset.UtcNow;
                user.LastUpdatedDate = DateTimeOffset.UtcNow;

                logger.LogInformation("Adult identity verified: User={UserId}", user.Id);
            }
        }

        private async Task ProcessParentChildConsentWebhookAsync(
            ParentalConsent consent, CancellationToken cancellationToken)
        {
            if (consent.Dependent == null || consent.DependentId == null)
            {
                return;
            }

            // Consent approved — now send the account creation invitation to the child
            // This reuses the existing DependentInvitation system
            var dependent = consent.Dependent;
            var existingInvitations = await dependentInvitationManager.GetByDependentIdAsync(
                dependent.Id, cancellationToken);

            // Only create invitation if one doesn't already exist
            if (!existingInvitations.Any())
            {
                logger.LogInformation(
                    "Parent-child consent verified. Dependent {DependentId} is ready for account invitation.",
                    dependent.Id);
            }
            else
            {
                logger.LogInformation(
                    "Parent-child consent verified. Dependent {DependentId} already has invitations.",
                    dependent.Id);
            }
        }

        private static int CalculateAge(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age))
            {
                age--;
            }
            return age;
        }

        #endregion
    }
}
