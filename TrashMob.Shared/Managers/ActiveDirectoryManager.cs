
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

        public ActiveDirectoryManager(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        public async Task<ActiveDirectoryResponseBase> CreateUserAsync(ActiveDirectoryNewUserRequest activeDirectoryNewUserRequest, CancellationToken cancellationToken = default)
        {
            var response = await DoesUserExist(activeDirectoryNewUserRequest.userName, activeDirectoryNewUserRequest.email, cancellationToken).ConfigureAwait(false);
            
            if (response != null)
            {
                return response;
            }

            var user = new User
            {
                Email = activeDirectoryNewUserRequest.email,
                GivenName = activeDirectoryNewUserRequest.givenName,
                SurName = activeDirectoryNewUserRequest.surname,
                UserName = activeDirectoryNewUserRequest.userName
            };

            await userManager.AddAsync(user, cancellationToken).ConfigureAwait(false);

            var newUserResponse = new ActiveDirectoryContinuationResponse
            {
                action = "Continue",
                version = "1.0.0",
            };

            return newUserResponse;
        }

        public async Task<ActiveDirectoryResponseBase> ValidateNewUserAsync(ActiveDirectoryValidateNewUserRequest activeDirectoryValidateNewUserRequest, CancellationToken cancellationToken = default)
        {
            var response = await DoesUserExist(activeDirectoryValidateNewUserRequest.userName, activeDirectoryValidateNewUserRequest.email, cancellationToken).ConfigureAwait(false);
            
            if (response != null)
            {
                return response;
            }

            var newUserResponse = new ActiveDirectoryContinuationResponse
            {
                action = "Continue",
                version = "1.0.0",
            };

            return newUserResponse;
        }

        private async Task<ActiveDirectoryResponseBase> DoesUserExist(string userName, string email, CancellationToken cancellationToken = default)
        {
            var originalUser = await userManager.GetUserByEmailAsync(email, cancellationToken).ConfigureAwait(false);

            if (originalUser != null)
            {
                var response = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "This email is already in use."
                };

                return response;
            }

            var checkUser = await userManager.GetUserByUserNameAsync(userName, CancellationToken.None);

            if (checkUser != null)
            {
                var response = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "Please choose a different User Name. This name already in use."
                };

                return response;
            }

            return null;
        }
    }
}
