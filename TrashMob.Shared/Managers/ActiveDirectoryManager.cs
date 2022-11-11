
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
            // Preventative check
            if ((await userManager.UserExistsAsync(activeDirectoryNewUserRequest.email, cancellationToken).ConfigureAwait(false)) != null)
            {
                var duplicateEmailResponse = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "This Email account is already in use."
                };

                return duplicateEmailResponse;
            }

            var checkUser = await userManager.GetUserByUserNameAsync(activeDirectoryNewUserRequest.userName, CancellationToken.None);

            if (checkUser != null)
            {
                var duplicateUserNameResponse = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "Please choose a different User Name. This name already in use."
                };

                return duplicateUserNameResponse;
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
            if ((await userManager.UserExistsAsync(activeDirectoryValidateNewUserRequest.email, cancellationToken).ConfigureAwait(false)) != null)
            {
                var duplicateEmailResponse = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "This email is already in use."
                };

                return duplicateEmailResponse;
            }

            var checkUser = await userManager.GetUserByUserNameAsync(activeDirectoryValidateNewUserRequest.userName, CancellationToken.None);

            if (checkUser != null)
            {
                var duplicateUserNameResponse = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "Please choose a different User Name. This name already in use."
                };

                return duplicateUserNameResponse;
            }

            var newUserResponse = new ActiveDirectoryContinuationResponse
            {
                action = "Continue",
                version = "1.0.0",
            };

            return newUserResponse;
        }
    }
}
