
namespace TrashMob.Shared.Managers
{
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

                // User does exist, see if they are trying to change their userName to something already in use
                if (activeDirectoryNewUserRequest.displayName != originalUser.UserName)
                {
                    var checkUserName = userManager.GetUserByUserNameAsync(activeDirectoryNewUserRequest.displayName, CancellationToken.None);

                    if (checkUserName != null)
                    {
                        var duplicateDisplayNameResponse = new ActiveDirectoryResponse
                        {
                            action = "Failed",
                            version = "1.0.0",
                            userMessage =  "Please choose a different Display Name. This name already in use." 
                        };

                        return duplicateDisplayNameResponse;
                    }
                }

                await userManager.UpdateAsync(originalUser, cancellationToken).ConfigureAwait(false);

                var userExistsResponse = new ActiveDirectoryResponse
                {
                    action = "Continue",
                    version = "1.0.0",
                    userId = originalUser.Id.ToString()
                };

                return userExistsResponse;
            }

            var checkUser = userManager.GetUserByUserNameAsync(activeDirectoryNewUserRequest.displayName, CancellationToken.None);

            if (checkUser != null)
            {
                var duplicateDisplayNameResponse = new ActiveDirectoryResponse
                {
                    action = "Failed",
                    version = "1.0.0",
                    userMessage = "Please choose a different Display Name. This name already in use."
                };

                return duplicateDisplayNameResponse;
            }

            var user = new User
            {
                Email = activeDirectoryNewUserRequest.email,
                GivenName = activeDirectoryNewUserRequest.givenName,
                SurName = activeDirectoryNewUserRequest.surname,
                UserName = activeDirectoryNewUserRequest.displayName
            };

            var newUser = await userManager.AddAsync(user, cancellationToken).ConfigureAwait(false);

            var newUserResponse = new ActiveDirectoryResponse
            {
                action = "Continue",
                version = "1.0.0",
                userId = newUser.Id.ToString()
            };

            return newUserResponse;
        }
    }
}
