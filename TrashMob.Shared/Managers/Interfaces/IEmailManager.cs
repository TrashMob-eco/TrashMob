namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Defines operations for managing and sending emails using SendGrid.
    /// </summary>
    public interface IEmailManager
    {
        /// <summary>
        /// Sends a templated email using SendGrid dynamic templates.
        /// </summary>
        /// <param name="subject">The email subject.</param>
        /// <param name="templateId">The SendGrid template ID.</param>
        /// <param name="groupId">The SendGrid unsubscribe group ID.</param>
        /// <param name="dynamicTemplateData">The dynamic template data.</param>
        /// <param name="recipients">The list of recipients.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task SendTemplatedEmailAsync(string subject, string templateId, int groupId, object dynamicTemplateData,
            List<EmailAddress> recipients, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the HTML email copy for a specific notification type.
        /// </summary>
        /// <param name="notificationType">The notification type name.</param>
        /// <returns>The HTML email body content.</returns>
        string GetHtmlEmailCopy(string notificationType);

        /// <summary>
        /// Retrieves all email templates from storage.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of email templates.</returns>
        Task<IEnumerable<EmailTemplate>> GetEmailTemplatesAsync(CancellationToken cancellationToken);
    }
}