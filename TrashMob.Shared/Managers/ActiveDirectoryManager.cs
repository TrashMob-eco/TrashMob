namespace TrashMob.Shared.Managers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using User = TrashMob.Models.User;

    /// <summary>
    /// Manages Azure Active Directory B2C user lifecycle operations including creation, validation, and profile updates.
    /// </summary>
    public class ActiveDirectoryManager : IActiveDirectoryManager
    {
        private readonly IUserManager userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveDirectoryManager"/> class.
        /// </summary>
        /// <param name="userManager">The user manager for user data operations.</param>
        public ActiveDirectoryManager(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        /// <inheritdoc />
        public async Task<ActiveDirectoryResponseBase> CreateUserAsync(
            ActiveDirectoryNewUserRequest activeDirectoryNewUserRequest, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(activeDirectoryNewUserRequest.email))
            {
                var failResponse = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "Email cannot be blank.",
                };

                return failResponse;
            }

            if (string.IsNullOrWhiteSpace(activeDirectoryNewUserRequest.userName))
            {
                var failResponse = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "Username cannot be blank.",
                };

                return failResponse;
            }

            var response = await DoesUserExist(activeDirectoryNewUserRequest.userName,
                activeDirectoryNewUserRequest.email, cancellationToken).ConfigureAwait(false);

            if (response != null)
            {
                return response;
            }

            var user = new User
            {
                Email = activeDirectoryNewUserRequest.email,
                ObjectId = activeDirectoryNewUserRequest.objectId,
                UserName = activeDirectoryNewUserRequest.userName,
            };

            await userManager.AddAsync(user, cancellationToken).ConfigureAwait(false);

            var newUserResponse = new ActiveDirectoryContinuationResponse();

            return newUserResponse;
        }

        /// <inheritdoc />
        public async Task<ActiveDirectoryResponseBase> DeleteUserAsync(Guid objectId,
            CancellationToken cancellationToken = default)
        {
            var user = await userManager.GetUserByObjectIdAsync(objectId, cancellationToken);

            if (user == null)
            {
                var response = new ActiveDirectoryValidationFailedResponse
                {
                    action = "UserNotFound",
                    version = "1.0.0",
                    userMessage = "User not found.",
                };

                return response;
            }

            await userManager.DeleteAsync(user.Id, cancellationToken).ConfigureAwait(false);

            var deleteUserResponse = new ActiveDirectoryContinuationResponse
            {
                action = "Continue",
                version = "1.0.0",
            };

            return deleteUserResponse;
        }

        /// <inheritdoc />
        public async Task<ActiveDirectoryResponseBase> ValidateNewUserAsync(
            ActiveDirectoryValidateNewUserRequest activeDirectoryValidateNewUserRequest,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(activeDirectoryValidateNewUserRequest.email))
            {
                var failResponse = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "Email cannot be blank.",
                };

                return failResponse;
            }

            if (string.IsNullOrWhiteSpace(activeDirectoryValidateNewUserRequest.userName))
            {
                var failResponse = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "Username cannot be blank.",
                };

                return failResponse;
            }

            var response = await DoesUserExist(activeDirectoryValidateNewUserRequest.userName,
                activeDirectoryValidateNewUserRequest.email, cancellationToken).ConfigureAwait(false);

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

        /// <inheritdoc />
        public async Task<ActiveDirectoryResponseBase> ValidateUpdateUserProfileAsync(
            ActiveDirectoryUpdateUserProfileRequest activeDirectoryUpdateUserProfileRequest,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(activeDirectoryUpdateUserProfileRequest.userName))
            {
                var failResponse = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "Username cannot be blank.",
                };

                return failResponse;
            }

            var originalUser = await userManager
                .GetUserByObjectIdAsync(activeDirectoryUpdateUserProfileRequest.objectId, cancellationToken)
                .ConfigureAwait(false);

            if (originalUser == null)
            {
                var response = new ActiveDirectoryValidationFailedResponse
                {
                    action = "UserNotFound",
                    version = "1.0.0",
                    userMessage = "User not found.",
                };

                return response;
            }

            var checkUser = await userManager
                .GetUserByUserNameAsync(activeDirectoryUpdateUserProfileRequest.userName, CancellationToken.None)
                .ConfigureAwait(false);

            if (checkUser != null && checkUser.ObjectId != activeDirectoryUpdateUserProfileRequest.objectId)
            {
                var response = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "Please choose a different User Name. This name already in use.",
                };

                return response;
            }

            var newUserResponse = new ActiveDirectoryContinuationResponse
            {
                action = "Continue",
                version = "1.0.0",
            };

            return newUserResponse;
        }

        /// <inheritdoc />
        public async Task<ActiveDirectoryResponseBase> UpdateUserProfileAsync(
            ActiveDirectoryUpdateUserProfileRequest activeDirectoryUpdateUserProfileRequest,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(activeDirectoryUpdateUserProfileRequest.userName))
            {
                var failResponse = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "Username cannot be blank.",
                };

                return failResponse;
            }

            var originalUser = await userManager
                .GetUserByObjectIdAsync(activeDirectoryUpdateUserProfileRequest.objectId, cancellationToken)
                .ConfigureAwait(false);

            if (originalUser == null)
            {
                var response = new ActiveDirectoryValidationFailedResponse
                {
                    action = "UserNotFound",
                    version = "1.0.0",
                    userMessage = "User not found.",
                };

                return response;
            }

            originalUser.UserName = activeDirectoryUpdateUserProfileRequest.userName;

            await userManager.UpdateAsync(originalUser, cancellationToken).ConfigureAwait(false);

            var newUserResponse = new ActiveDirectoryContinuationResponse
            {
                action = "Continue",
                version = "1.0.0",
            };

            return newUserResponse;
        }

        private async Task<ActiveDirectoryResponseBase> DoesUserExist(string userName, string email,
            CancellationToken cancellationToken = default)
        {
            var originalUser = await userManager.GetUserByEmailAsync(email, cancellationToken).ConfigureAwait(false);

            if (originalUser != null)
            {
                var response = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "This email is already in use.",
                };

                return response;
            }

            var checkUser = await userManager.GetUserByUserNameAsync(userName, CancellationToken.None)
                .ConfigureAwait(false);

            if (checkUser != null)
            {
                var response = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "Please choose a different User Name. This name already in use.",
                };

                return response;
            }

            return null;
        }
    }
}