
namespace TrashMob.Shared.Managers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using User = Models.User;

    public class ActiveDirectoryManager : IActiveDirectoryManager
    {
        private readonly IUserManager userManager;
        private readonly IEmailManager emailManager;

        public ActiveDirectoryManager(IUserManager userManager, IEmailManager emailManager)
        {
            this.userManager = userManager;
            this.emailManager = emailManager;
        }

        public async Task<ActiveDirectoryResponse> CreateUserAsync(ActiveDirectoryNewUserRequest activeDirectoryNewUserRequest, CancellationToken cancellationToken = default)
        {
            User originalUser;

            if ((originalUser = await userManager.UserExistsAsync(activeDirectoryNewUserRequest.identities[0].issuerAssignedId, cancellationToken).ConfigureAwait(false)) != null)
            {
                originalUser.Email = activeDirectoryNewUserRequest.email;
                originalUser.SourceSystemUserName = activeDirectoryNewUserRequest.identities[0].issuerAssignedId;

                await userManager.UpdateAsync(originalUser, cancellationToken).ConfigureAwait(false);

                var userExistsResponse = new ActiveDirectoryResponse
                {
                    Action = "Continue",
                    Version = "1.0.0",
                    UserId = originalUser.Id.ToString()
                };

                return userExistsResponse;
            }

            var user = new User
            {
                Email = activeDirectoryNewUserRequest.email,
                GivenName = activeDirectoryNewUserRequest.givenName,
                SurName = activeDirectoryNewUserRequest.surname,
                UserName = activeDirectoryNewUserRequest.displayName
            };

            if (string.IsNullOrEmpty(activeDirectoryNewUserRequest.displayName))
            {
                // On insert we need a random user name to avoid duplicates, but we don't want to show the full email address ever, so take a subset
                // of their email and then add a random number to the end.
                Random rnd = new();
                var userNum = rnd.Next(100, 999).ToString();
                var first = activeDirectoryNewUserRequest.email.Split("@")[0];
                user.UserName = first.Substring(0, Math.Min(first.Length - 1, 8)) + userNum;
            }

            var newUser = await userManager.AddAsync(user, cancellationToken).ConfigureAwait(false);

            var newUserResponse = new ActiveDirectoryResponse
            {
                Action = "Continue",
                Version = "1.0.0",
                UserId = newUser.Id.ToString()
            };

            return newUserResponse;
        }
    }
}
