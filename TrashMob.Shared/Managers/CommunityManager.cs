
namespace TrashMob.Shared.Managers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityManager : ExtendedManager<Community>, IExtendedManager<Community>
    {
        private readonly IEmailManager emailManager;

        public CommunityManager(IRepository<Community> repository, IEmailManager emailManager) : base(repository)
        {
            this.emailManager = emailManager;
        }      

        public override async Task<Community> Add(Community community)
        {
            var outputCommunity = await Repository.Add(community);

            var message = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.CommunityCreated.ToString());
            var subject = "A Community has been created on TrashMob.eco";

            // TODO: Add more fields for this
            // TODO: Add email to community

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

            await emailManager.SendTemplatedEmail(subject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, dynamicTemplateData, recipients, CancellationToken.None).ConfigureAwait(false);

            return outputCommunity;
        }
    }
}
