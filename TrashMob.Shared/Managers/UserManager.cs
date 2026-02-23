namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manages user accounts including creation, updates, deletion, and related cascade operations.
    /// </summary>
    public class UserManager(
        IKeyedRepository<User> repository,
        IUserDeletionService userDeletionService,
        IEmailManager emailManager)
        : KeyedManager<User>(repository), IUserManager
    {
        /// <inheritdoc />
        public async Task<User> GetUserByNameIdentifierAsync(string nameIdentifier,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(u => u.NameIdentifier == nameIdentifier).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<User> GetUserByObjectIdAsync(Guid objectId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(u => u.ObjectId == objectId).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<User> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(u => u.UserName == userName).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(u => u.Email == email).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<User> GetUserByInternalIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(u => u.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            // The IsSiteAdmin flag can only be changed directly in the database, so once set, we need to preserve that, no matter what the user passes in
            var matchedUser = await GetUserByInternalIdAsync(user.Id, cancellationToken);
            user.IsSiteAdmin = matchedUser.IsSiteAdmin;

            return await base.UpdateAsync(user, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await userDeletionService.DeleteUserDataAsync(id, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<User> AddAsync(User user, CancellationToken cancellationToken)
        {
            user.Id = Guid.NewGuid();
            user.MemberSince = DateTimeOffset.UtcNow;
            user.DateAgreedToTrashMobWaiver = DateTimeOffset.MinValue;
            user.TrashMobWaiverVersion = string.Empty;
            user.IsSiteAdmin = false;

            var addedUser = await base.AddAsync(user, cancellationToken);

            // Notify Admins that a new user has joined
            var message = $"A new user: {user.Email} has joined TrashMob.eco!";
            var subject = "New User Alert";

            var dynamicTemplateData = new
            {
                username = Constants.TrashMobEmailName,
                emailCopy = message,
                subject,
            };

            List<EmailAddress> recipients =
            [
                new() { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress },
            ];

            await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.GenericEmail,
                    SendGridEmailGroupId.General, dynamicTemplateData, recipients, CancellationToken.None);

            // Send welcome email to new User
            var welcomeMessage = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.WelcomeToTrashMob.ToString());
            var welcomeSubject = "Welcome to TrashMob.eco!";

            var userDynamicTemplateData = new
            {
                username = user.UserName,
                emailCopy = welcomeMessage,
                subject = welcomeSubject,
            };

            List<EmailAddress> welcomeRecipients =
            [
                new() { Name = user.UserName, Email = user.Email },
            ];

            await emailManager.SendTemplatedEmailAsync(welcomeSubject, SendGridEmailTemplateId.GenericEmail,
                    SendGridEmailGroupId.General, userDynamicTemplateData, welcomeRecipients, CancellationToken.None);

            return addedUser;
        }

        /// <inheritdoc />
        public async Task<bool> UserExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Repository.Get(e => e.Id == id).AnyAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<User> UserExistsAsync(string nameIdentifier, CancellationToken cancellationToken = default)
        {
            return await Repository.Get(u => u.NameIdentifier == nameIdentifier).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
