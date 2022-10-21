
namespace TrashMob.Shared.Managers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using TrashMob.Shared.Persistence.Interfaces;

    public class ContactRequestManager : KeyedManager<ContactRequest>, IKeyedManager<ContactRequest>
    {
        private readonly IEmailManager emailManager;

        public ContactRequestManager(IKeyedRepository<ContactRequest> repository, IEmailManager emailManager) : base(repository)
        {
            this.emailManager = emailManager;
        }      

        public override async Task<ContactRequest> AddAsync(ContactRequest contactRequest, CancellationToken cancellationToken = default)
        {
            var outputContactRequest = await Repository.AddAsync(contactRequest);

            var message = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.ContactRequestReceived.ToString());
            var subject = "A Contact Request has been received on TrashMob.eco!";

            message = message.Replace("{UserName}", contactRequest.Name);
            message = message.Replace("{UserEmail}", contactRequest.Email);
            message = message.Replace("{Message}", contactRequest.Message);

            var recipients = new List<EmailAddress>
            {
                new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress }
            };

            var dynamicTemplateData = new
            {
                username = Constants.TrashMobEmailName,
                emailCopy = message,
                subject = subject,
            };

            await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, dynamicTemplateData, recipients, CancellationToken.None).ConfigureAwait(false);

            return outputContactRequest;
        }
    }
}
