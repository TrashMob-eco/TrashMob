namespace TrashMob.Shared.Persistence
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared;

    public interface IEmailManager
    {
        Task SendTemplatedEmail(string subject, string templateId, int groupId, object dynamicTemplateData, List<EmailAddress> recipients, CancellationToken cancellationToken = default);

        string GetHtmlEmailCopy(string notificationType);
    }
}
