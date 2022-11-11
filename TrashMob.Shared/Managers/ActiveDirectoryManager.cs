
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
            User originalUser;

            if ((originalUser = await userManager.UserExistsAsync(activeDirectoryNewUserRequest.email, cancellationToken).ConfigureAwait(false)) != null)
            {
                originalUser.SourceSystemUserName = activeDirectoryNewUserRequest.email;

                // User does exist, see if they are trying to change their userName to something already in use
                if (activeDirectoryNewUserRequest.userName != originalUser.UserName)
                {
                    var checkUserName = await userManager.GetUserByUserNameAsync(activeDirectoryNewUserRequest.userName, CancellationToken.None);

                    if (checkUserName != null)
                    {
                        var duplicateUserNameResponse = new ActiveDirectoryValidationFailedResponse
                        {
                            action = "ValidationError",
                            version = "1.0.0",
                            userMessage =  "Please choose a different User Name. This name already in use." 
                        };

                        return duplicateUserNameResponse;
                    }
                }

                await userManager.UpdateAsync(originalUser, cancellationToken).ConfigureAwait(false);

                var userExistsResponse = new ActiveDirectoryContinuationResponse
                {
                    action = "Continue",
                    version = "1.0.0",
                };

                return userExistsResponse;
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

        public async Task<ActiveDirectoryResponseBase> ValidateUserAsync(ActiveDirectoryValidateNewUserRequest activeDirectoryValidateNewUserRequest, CancellationToken cancellationToken = default)
        {
            User originalUser;

            if ((originalUser = await userManager.UserExistsAsync(activeDirectoryValidateNewUserRequest.email, cancellationToken).ConfigureAwait(false)) != null)
            {
                // User does exist, see if they are trying to change their userName to something already in use
                if (activeDirectoryValidateNewUserRequest.userName != originalUser.UserName)
                {
                    var checkUserName = await userManager.GetUserByUserNameAsync(activeDirectoryValidateNewUserRequest.userName, CancellationToken.None);

                    if (checkUserName != null)
                    {
                        var duplicateUserNameResponse = new ActiveDirectoryValidationFailedResponse
                        {
                            action = "ValidationError",
                            version = "1.0.0",
                            userMessage = "Please choose a different User Name. This name already in use."
                        };

                        return duplicateUserNameResponse;
                    }
                }

                var userExistsResponse = new ActiveDirectoryContinuationResponse
                {
                    action = "Continue",
                    version = "1.0.0",
                };

                return userExistsResponse;
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
