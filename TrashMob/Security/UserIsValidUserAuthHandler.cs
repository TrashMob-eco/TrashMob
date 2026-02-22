namespace TrashMob.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using TrashMob.Services;
    using TrashMob.Shared.Managers.Interfaces;

    public class UserIsValidUserAuthHandler(
        IHttpContextAccessor httpContext,
        IUserManager userManager,
        ICiamGraphService ciamGraphService,
        ILogger<UserIsValidUserAuthHandler> logger) : AuthorizationHandler<UserIsValidUserRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            UserIsValidUserRequirement requirement)
        {
            try
            {
                var emailAddressClaim = context.User.FindFirst(ClaimTypes.Email);
                var emailClaim = context.User.FindFirst("email");
                var email = emailAddressClaim is null ? emailClaim?.Value : emailAddressClaim?.Value;

                var oidClaim = context.User.FindFirst("oid")
                            ?? context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")
                            ?? context.User.FindFirst(ClaimTypes.NameIdentifier);
                Guid? objectId = oidClaim is not null && Guid.TryParse(oidClaim.Value, out var parsedOid)
                    ? parsedOid
                    : null;

                // Step 1: Try to find user by email
                Models.User user = null;
                if (!string.IsNullOrWhiteSpace(email))
                {
                    user = await userManager.GetUserByEmailAsync(email, CancellationToken.None);

                    // Auto-link CIAM ObjectId when existing user found by email (B2C → CIAM migration)
                    if (user is not null && objectId.HasValue && user.ObjectId != objectId.Value)
                    {
                        logger.LogInformation(
                            "Updating ObjectId for user {Email} from {OldObjectId} to {NewObjectId} (CIAM migration)",
                            email, user.ObjectId, objectId.Value);
                        user.ObjectId = objectId.Value;
                        await userManager.UpdateAsync(user, CancellationToken.None);
                    }
                }

                // Step 2: If not found by email, try ObjectId lookup
                if (user is null && objectId.HasValue)
                {
                    user = await userManager.GetUserByObjectIdAsync(objectId.Value, CancellationToken.None);

                    // If found by OID and email is available but different, update the stored email
                    if (user is not null && !string.IsNullOrWhiteSpace(email) && user.Email != email)
                    {
                        user.Email = email;
                        await userManager.UpdateAsync(user, CancellationToken.None);
                    }
                }

                // Step 3: If still not found and no email in token, fetch email from CIAM via Graph API
                if (user is null && string.IsNullOrWhiteSpace(email) && objectId.HasValue)
                {
                    email = await ciamGraphService.GetUserEmailAsync(objectId.Value, CancellationToken.None);

                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        logger.LogInformation("Resolved email {Email} from CIAM Graph API for ObjectId {ObjectId}",
                            email, objectId.Value);

                        // Try finding by the Graph-resolved email
                        user = await userManager.GetUserByEmailAsync(email, CancellationToken.None);

                        if (user is not null)
                        {
                            // Link the CIAM ObjectId to the existing user
                            user.ObjectId = objectId.Value;
                            await userManager.UpdateAsync(user, CancellationToken.None);
                        }
                    }
                }

                // Step 4: Auto-create if user still not found
                if (user is null)
                {
                    user = await TryAutoCreateUser(context, email);

                    if (user is null)
                    {
                        var identifier = !string.IsNullOrWhiteSpace(email) ? email : "unknown (no email claim)";
                        AuthorizationFailure.Failed(new List<AuthorizationFailureReason>
                            { new(this, $"User '{identifier}' not found and could not be auto-created.") });
                        return;
                    }
                }

                // Auto-populate profile fields from social provider claims (one-time fill)
                var needsUpdate = false;

                if (string.IsNullOrEmpty(user.ProfilePhotoUrl))
                {
                    var pictureClaim = context.User.FindFirst("picture");
                    if (pictureClaim is not null)
                    {
                        user.ProfilePhotoUrl = pictureClaim.Value;
                        needsUpdate = true;
                    }
                }

                if (string.IsNullOrEmpty(user.GivenName))
                {
                    var givenNameClaim = context.User.FindFirst(ClaimTypes.GivenName)
                                      ?? context.User.FindFirst("given_name");
                    if (givenNameClaim is not null)
                    {
                        user.GivenName = givenNameClaim.Value;
                        needsUpdate = true;
                    }
                }

                if (string.IsNullOrEmpty(user.Surname))
                {
                    var surnameClaim = context.User.FindFirst(ClaimTypes.Surname)
                                    ?? context.User.FindFirst("family_name");
                    if (surnameClaim is not null)
                    {
                        user.Surname = surnameClaim.Value;
                        needsUpdate = true;
                    }
                }

                if (!user.DateOfBirth.HasValue)
                {
                    var dobClaim = context.User.FindFirst("dateOfBirth")
                                 ?? context.User.FindFirst(ClaimTypes.DateOfBirth)
                                 ?? context.User.Claims.FirstOrDefault(c => c.Type.Contains("dateOfBirth", StringComparison.OrdinalIgnoreCase));
                    if (dobClaim is not null && DateTimeOffset.TryParse(dobClaim.Value, out var parsedDob))
                    {
                        user.DateOfBirth = parsedDob;
                        needsUpdate = true;
                    }
                }

                if (needsUpdate)
                {
                    await userManager.UpdateAsync(user, CancellationToken.None);
                }

                if (!httpContext.HttpContext.Items.ContainsKey("UserId"))
                {
                    httpContext.HttpContext.Items.Add("UserId", user.Id);
                }

                context.Succeed(requirement);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while authenticating user. {0}",
                    JsonConvert.SerializeObject(context.User));
            }
        }

        private async Task<Models.User> TryAutoCreateUser(AuthorizationHandlerContext context, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                logger.LogWarning("Cannot auto-create user: no email available from token or Graph API");
                return null;
            }

            // Extract the object ID from token claims (Entra uses "oid" or the sub claim)
            var oidClaim = context.User.FindFirst("oid")
                        ?? context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")
                        ?? context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (oidClaim is null || !Guid.TryParse(oidClaim.Value, out var objectId))
            {
                logger.LogWarning("Cannot auto-create user: no valid object ID claim found for email {Email}", email);
                return null;
            }

            // Extract name and profile claims if available
            var givenNameClaim = context.User.FindFirst(ClaimTypes.GivenName)
                              ?? context.User.FindFirst("given_name");
            var surnameClaim = context.User.FindFirst(ClaimTypes.Surname)
                            ?? context.User.FindFirst("family_name");
            var pictureClaim = context.User.FindFirst("picture");
            var dobClaim = context.User.FindFirst("dateOfBirth")
                         ?? context.User.FindFirst(ClaimTypes.DateOfBirth)
                         ?? context.User.Claims.FirstOrDefault(c => c.Type.Contains("dateOfBirth", StringComparison.OrdinalIgnoreCase));

            // Generate a username from email (part before @) — user can change it later
            var userName = email.Split('@')[0];

            // Ensure username is unique
            var existingUser = await userManager.GetUserByUserNameAsync(userName, CancellationToken.None);
            if (existingUser is not null)
            {
                userName = $"{userName}_{objectId.ToString()[..8]}";
            }

            DateTimeOffset? dateOfBirth = null;
            if (dobClaim is not null && DateTimeOffset.TryParse(dobClaim.Value, out var parsedDob))
            {
                dateOfBirth = parsedDob;
            }

            var newUser = new Models.User
            {
                Email = email,
                ObjectId = objectId,
                UserName = userName,
                GivenName = givenNameClaim?.Value,
                Surname = surnameClaim?.Value,
                ProfilePhotoUrl = pictureClaim?.Value,
                DateOfBirth = dateOfBirth,
                MemberSince = DateTimeOffset.UtcNow,
            };

            try
            {
                var createdUser = await userManager.AddAsync(newUser, CancellationToken.None);
                logger.LogInformation("Auto-created user {Email} (ObjectId: {ObjectId}) on first Entra login", email, objectId);
                return createdUser;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to auto-create user {Email} (ObjectId: {ObjectId})", email, objectId);
                return null;
            }
        }
    }
}
