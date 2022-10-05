namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Poco;

    public interface IEmailManager
    {
        Task SendTemplatedEmail(string subject, string templateId, int groupId, object dynamicTemplateData, List<EmailAddress> recipients, CancellationToken cancellationToken = default);

        string GetHtmlEmailCopy(string notificationType);
    }
}
