
namespace TrashMob.Shared.Managers
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityManager : BaseManager<Community>, IBaseManager<Community>
    {
        private readonly IBaseRepository<CommunityUser> communityUserRepository;
        private readonly IEmailManager emailManager;

        public CommunityManager(IKeyedRepository<Community> repository, IBaseRepository<CommunityUser> communityUserRepository, IEmailManager emailManager) : base(repository)
        {
            this.communityUserRepository = communityUserRepository;
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

        public override async Task<IEnumerable<Community>> GetByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var result = communityUserRepository.Get().Where(cu => cu.UserId == userId);

            if (result.Any())
            {
                return await result
                               .Select(c => c.Community)
                               .ToListAsync(cancellationToken);
            }
            else
            {
                return null;
            }
        }
    }
}
