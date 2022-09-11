
namespace TrashMob.Shared.Managers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityRequestManager : ICommunityRequestManager
    {
        private readonly ICommunityRequestRepository communityRequestRepository;
        private readonly IEmailManager emailManager;

        public CommunityRequestManager(ICommunityRequestRepository communityRequestRepository, IEmailManager emailManager)
        {
            this.communityRequestRepository = communityRequestRepository;
            this.emailManager = emailManager;
        }      

        public async Task AddCommunityRequest(CommunityRequest communityRequest)
        {
            await communityRequestRepository.AddCommunityRequest(communityRequest);

            var message = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.CommunityRequestReceived.ToString());
            var subject = "A Community Request has been received on TrashMob.eco";

            // TODO: Add more fields for this
            // TODO: Add email to community
            message = message.Replace("{UserName}", communityRequest.Name);
            message = message.Replace("{UserEmail}", communityRequest.Email);

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
        }
    }
}
