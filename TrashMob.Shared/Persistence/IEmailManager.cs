namespace TrashMob.Shared.Persistence
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared;

    public interface IEmailManager
    {
        Task SendGenericSystemEmail(string subject, string message,  string htmlMessage, List<EmailAddress> recipients, CancellationToken cancellationToken);

        Task SendSystemEmail(string subject, string message, string htmlMessage, List<EmailAddress> recipients, CancellationToken cancellationToken);

        string GetEmailTemplate(string notificationType);
        
        string GetHtmlEmailTemplate(string notificationType);
    }
}
