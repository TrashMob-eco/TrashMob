namespace TrashMob.Shared.Managers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Sends notification emails for role grants and revocations (Project 64
    /// Phase 3). Runs after the role admin controller has already persisted the
    /// change, so a delivery failure does not roll back the grant.
    /// </summary>
    public class RoleGrantNotificationService(
        IEmailManager emailManager,
        ILogger<RoleGrantNotificationService> logger) : IRoleGrantNotificationService
    {
        /// <inheritdoc />
        public async Task SendRoleGrantedAsync(UserRole grant, User recipient, User grantedBy, CancellationToken cancellationToken = default)
        {
            if (grant == null || recipient == null || string.IsNullOrWhiteSpace(recipient.Email))
            {
                return;
            }

            var roleName = grant.Role?.Name ?? "role";
            var subject = $"You've been granted the {roleName} role on TrashMob.eco";

            var expirySuffix = grant.ExpiryDate.HasValue
                ? $" (expires {grant.ExpiryDate.Value.ToLocalTime():D})"
                : string.Empty;

            var message = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.RoleGranted.ToString())
                .Replace("{RoleName}", roleName)
                .Replace("{RoleDescription}", grant.Role?.Description ?? string.Empty)
                .Replace("{GrantedByName}", DisplayName(grantedBy))
                .Replace("{GrantedDate}", grant.GrantedDate.ToLocalTime().ToString("D"))
                .Replace("{ExpirySuffix}", expirySuffix);

            var recipients = new List<EmailAddress>
            {
                new() { Name = recipient.DisplayFirstName, Email = recipient.Email },
            };

            var dynamicTemplateData = new
            {
                username = recipient.DisplayFirstName,
                emailCopy = message,
                subject,
            };

            await emailManager.SendTemplatedEmailAsync(
                subject,
                SendGridEmailTemplateId.GenericEmail,
                SendGridEmailGroupId.General,
                dynamicTemplateData,
                recipients,
                cancellationToken);

            logger.LogInformation(
                "Sent RoleGranted email UserId={UserId} Role={Role} GrantedBy={GrantedBy}",
                recipient.Id, roleName, grantedBy?.Id);
        }

        /// <inheritdoc />
        public async Task SendRoleRevokedAsync(UserRole grant, User recipient, User revokedBy, CancellationToken cancellationToken = default)
        {
            if (grant == null || recipient == null || string.IsNullOrWhiteSpace(recipient.Email))
            {
                return;
            }

            var roleName = grant.Role?.Name ?? "role";
            var subject = $"The {roleName} role has been removed from your TrashMob.eco account";

            var reasonSuffix = string.IsNullOrWhiteSpace(grant.RevokedReason)
                ? string.Empty
                : $" Reason: {grant.RevokedReason}";

            var revokedDate = grant.RevokedDate?.ToLocalTime().ToString("D") ?? string.Empty;

            var message = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.RoleRevoked.ToString())
                .Replace("{RoleName}", roleName)
                .Replace("{RevokedByName}", DisplayName(revokedBy))
                .Replace("{RevokedDate}", revokedDate)
                .Replace("{ReasonSuffix}", reasonSuffix);

            var recipients = new List<EmailAddress>
            {
                new() { Name = recipient.DisplayFirstName, Email = recipient.Email },
            };

            var dynamicTemplateData = new
            {
                username = recipient.DisplayFirstName,
                emailCopy = message,
                subject,
            };

            await emailManager.SendTemplatedEmailAsync(
                subject,
                SendGridEmailTemplateId.GenericEmail,
                SendGridEmailGroupId.General,
                dynamicTemplateData,
                recipients,
                cancellationToken);

            logger.LogInformation(
                "Sent RoleRevoked email UserId={UserId} Role={Role} RevokedBy={RevokedBy}",
                recipient.Id, roleName, revokedBy?.Id);
        }

        private static string DisplayName(User user)
        {
            if (user == null)
            {
                return "a TrashMob administrator";
            }

            if (!string.IsNullOrWhiteSpace(user.DisplayFirstName))
            {
                return user.DisplayFirstName;
            }

            return string.IsNullOrWhiteSpace(user.UserName) ? "a TrashMob administrator" : user.UserName;
        }
    }
}
