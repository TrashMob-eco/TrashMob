namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manages the lifecycle of dependent invitations for minors to create their own accounts.
    /// </summary>
    public class DependentInvitationManager(
        IKeyedRepository<DependentInvitation> repository,
        IKeyedRepository<Dependent> dependentRepository,
        IKeyedRepository<User> userRepository,
        IEmailManager emailManager,
        ILogger<DependentInvitationManager> logger)
        : KeyedManager<DependentInvitation>(repository), IDependentInvitationManager
    {
        private const int TokenSizeBytes = 32;
        private const int InvitationExpiryDays = 30;
        private const int MinimumAgeForAccount = 13;

        /// <inheritdoc />
        public async Task<DependentInvitation> CreateInvitationAsync(
            Guid dependentId, string email, Guid parentUserId, CancellationToken cancellationToken = default)
        {
            var dependent = await dependentRepository.GetAsync(dependentId, cancellationToken)
                ?? throw new InvalidOperationException("Dependent not found.");

            if (dependent.ParentUserId != parentUserId)
            {
                throw new InvalidOperationException("Dependent does not belong to this parent.");
            }

            // Validate age (must be 13+)
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - dependent.DateOfBirth.Year;
            if (today < dependent.DateOfBirth.AddYears(age))
            {
                age--;
            }

            if (age < MinimumAgeForAccount)
            {
                throw new InvalidOperationException($"Dependent must be at least {MinimumAgeForAccount} years old to create an account.");
            }

            // Check for existing active invitation
            var existingInvitation = await Repo.Get(
                    i => i.DependentId == dependentId
                         && i.InvitationStatusId == (int)InvitationStatusEnum.Sent
                         && i.ExpiresDate > DateTimeOffset.UtcNow)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingInvitation != null)
            {
                throw new InvalidOperationException("An active invitation already exists for this dependent. Cancel it first or use resend.");
            }

            // Generate secure token
            var plainToken = GenerateToken();
            var tokenHash = HashToken(plainToken);

            var invitation = new DependentInvitation
            {
                Id = Guid.NewGuid(),
                DependentId = dependentId,
                ParentUserId = parentUserId,
                Email = email.Trim().ToLowerInvariant(),
                TokenHash = tokenHash,
                InvitationStatusId = (int)InvitationStatusEnum.New,
                DateInvited = DateTimeOffset.UtcNow,
                ExpiresDate = DateTimeOffset.UtcNow.AddDays(InvitationExpiryDays),
            };

            var created = await base.AddAsync(invitation, parentUserId, cancellationToken);

            // Send email
            await SendInvitationEmailAsync(created, dependent, parentUserId, plainToken, cancellationToken);

            created.InvitationStatusId = (int)InvitationStatusEnum.Sent;
            await base.UpdateAsync(created, parentUserId, cancellationToken);

            logger.LogInformation(
                "Dependent invitation sent for dependent {DependentId} to {Email}",
                dependentId, email);

            return created;
        }

        /// <inheritdoc />
        public async Task<DependentInvitationInfo> VerifyTokenAsync(
            string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new DependentInvitationInfo { IsValid = false, ErrorMessage = "Invalid invitation link." };
            }

            var tokenHash = HashToken(token);

            var invitation = await Repo.Get(
                    i => i.TokenHash == tokenHash
                         && i.InvitationStatusId == (int)InvitationStatusEnum.Sent)
                .Include(i => i.Dependent)
                .Include(i => i.ParentUser)
                .FirstOrDefaultAsync(cancellationToken);

            if (invitation == null)
            {
                return new DependentInvitationInfo { IsValid = false, ErrorMessage = "This invitation is not valid or has already been used." };
            }

            if (invitation.ExpiresDate <= DateTimeOffset.UtcNow)
            {
                return new DependentInvitationInfo { IsValid = false, ErrorMessage = "This invitation has expired. Ask your parent to send a new one." };
            }

            return new DependentInvitationInfo
            {
                IsValid = true,
                ParentName = invitation.ParentUser?.UserName ?? "Your parent",
                DependentFirstName = invitation.Dependent?.FirstName ?? string.Empty,
            };
        }

        /// <inheritdoc />
        public async Task<bool> TryAcceptByEmailAsync(
            string email, User minorUser, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var normalizedEmail = email.Trim().ToLowerInvariant();

            var invitation = await Repo.Get(
                    i => i.Email == normalizedEmail
                         && i.InvitationStatusId == (int)InvitationStatusEnum.Sent
                         && i.ExpiresDate > DateTimeOffset.UtcNow)
                .Include(i => i.Dependent)
                .FirstOrDefaultAsync(cancellationToken);

            if (invitation == null)
            {
                return false;
            }

            // Mark invitation as accepted
            invitation.InvitationStatusId = (int)InvitationStatusEnum.Accepted;
            invitation.DateAccepted = DateTimeOffset.UtcNow;
            invitation.AcceptedByUserId = minorUser.Id;
            await base.UpdateAsync(invitation, minorUser.Id, cancellationToken);

            // Link the minor user to the parent
            minorUser.IsMinor = true;
            minorUser.ParentUserId = invitation.ParentUserId;
            minorUser.DependentId = invitation.DependentId;
            await userRepository.UpdateAsync(minorUser);

            logger.LogInformation(
                "Minor user {UserId} linked to parent {ParentUserId} via dependent invitation {InvitationId}",
                minorUser.Id, invitation.ParentUserId, invitation.Id);

            return true;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DependentInvitation>> GetByDependentIdAsync(
            Guid dependentId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(i => i.DependentId == dependentId)
                .OrderByDescending(i => i.DateInvited)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DependentInvitation>> GetByParentUserIdAsync(
            Guid parentUserId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(i => i.ParentUserId == parentUserId)
                .OrderByDescending(i => i.DateInvited)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task CancelInvitationAsync(
            Guid invitationId, Guid userId, CancellationToken cancellationToken = default)
        {
            var invitation = await Repo.GetAsync(invitationId, cancellationToken)
                ?? throw new InvalidOperationException("Invitation not found.");

            if (invitation.ParentUserId != userId)
            {
                throw new InvalidOperationException("Only the parent who sent the invitation can cancel it.");
            }

            if (invitation.InvitationStatusId == (int)InvitationStatusEnum.Accepted)
            {
                throw new InvalidOperationException("Cannot cancel an already accepted invitation.");
            }

            invitation.InvitationStatusId = (int)InvitationStatusEnum.Canceled;
            await base.UpdateAsync(invitation, userId, cancellationToken);

            logger.LogInformation("Dependent invitation {InvitationId} canceled by user {UserId}", invitationId, userId);
        }

        /// <inheritdoc />
        public async Task<DependentInvitation> ResendInvitationAsync(
            Guid invitationId, Guid userId, CancellationToken cancellationToken = default)
        {
            var invitation = await Repo.Get(i => i.Id == invitationId)
                .Include(i => i.Dependent)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new InvalidOperationException("Invitation not found.");

            if (invitation.ParentUserId != userId)
            {
                throw new InvalidOperationException("Only the parent who sent the invitation can resend it.");
            }

            if (invitation.InvitationStatusId == (int)InvitationStatusEnum.Accepted)
            {
                throw new InvalidOperationException("Cannot resend an already accepted invitation.");
            }

            // Generate new token
            var plainToken = GenerateToken();
            invitation.TokenHash = HashToken(plainToken);
            invitation.ExpiresDate = DateTimeOffset.UtcNow.AddDays(InvitationExpiryDays);
            invitation.InvitationStatusId = (int)InvitationStatusEnum.New;

            await SendInvitationEmailAsync(invitation, invitation.Dependent, userId, plainToken, cancellationToken);

            invitation.InvitationStatusId = (int)InvitationStatusEnum.Sent;
            await base.UpdateAsync(invitation, userId, cancellationToken);

            logger.LogInformation("Dependent invitation {InvitationId} resent to {Email}", invitationId, invitation.Email);

            return invitation;
        }

        private async Task SendInvitationEmailAsync(
            DependentInvitation invitation,
            Dependent dependent,
            Guid parentUserId,
            string plainToken,
            CancellationToken cancellationToken)
        {
            var parent = await userRepository.GetAsync(parentUserId, cancellationToken);
            var parentName = parent?.UserName ?? "Your parent";
            var inviteUrl = $"https://www.trashmob.eco/invite/accept?token={Uri.EscapeDataString(plainToken)}";

            var emailBody = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.InviteMinorToCreateAccount.ToString());
            // Prepend personalized greeting and append the invite link
            var personalizedBody = $"<p>Hi {dependent.FirstName},</p>{emailBody}<p><a href=\"{inviteUrl}\" style=\"display:inline-block;padding:12px 24px;background-color:#16a34a;color:white;text-decoration:none;border-radius:6px;font-weight:bold;\">Create My TrashMob Account</a></p><p>Invited by: {parentName}</p>";

            var subject = $"{parentName} invited you to join TrashMob.eco!";

            var dynamicTemplateData = new
            {
                emailCopy = personalizedBody,
                subject,
            };

            List<EmailAddress> recipients =
            [
                new() { Name = dependent.FirstName, Email = invitation.Email },
            ];

            await emailManager.SendTemplatedEmailAsync(
                subject,
                SendGridEmailTemplateId.GenericEmail,
                SendGridEmailGroupId.General,
                dynamicTemplateData,
                recipients,
                cancellationToken);
        }

        private static string GenerateToken()
        {
            var bytes = new byte[TokenSizeBytes];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        private static string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes);
        }
    }
}
