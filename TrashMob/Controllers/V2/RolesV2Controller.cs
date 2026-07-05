namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for administering role assignments (Project 64 Phase 3).
    /// SiteAdmin-only — grants and revocations are audited and trigger email
    /// notifications to the affected user.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/roles")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public class RolesV2Controller(
        IUserRoleService userRoleService,
        IUserManager userManager,
        IRoleGrantNotificationService notificationService,
        ILogger<RolesV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId)
            ? parsedUserId
            : Guid.Empty;

        /// <summary>
        /// Lists every seeded role with the count of users currently holding
        /// an active grant.
        /// </summary>
        /// <response code="200">Returns the role list.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List(CancellationToken cancellationToken)
        {
            var roles = await userRoleService.ListRolesAsync(cancellationToken);

            var dtos = new List<RoleDto>(roles.Count);
            foreach (var role in roles)
            {
                var members = await userRoleService.GetUsersInRoleAsync(role.Name, cancellationToken);
                dtos.Add(role.ToV2Dto(members.Count));
            }

            return Ok(dtos);
        }

        /// <summary>
        /// Lists every user with an active grant of the given role.
        /// </summary>
        /// <param name="roleName">The role name (case-insensitive).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the member list (PII-safe user DTOs).</response>
        /// <response code="404">Role not found.</response>
        [HttpGet("{roleName}/members")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Members(string roleName, CancellationToken cancellationToken)
        {
            var roles = await userRoleService.ListRolesAsync(cancellationToken);
            if (!roles.Any(r => string.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase)))
            {
                return NotFound();
            }

            var members = await userRoleService.GetUsersInRoleAsync(roleName, cancellationToken);
            return Ok(members.Select(u => u.ToV2Dto()).ToList());
        }

        /// <summary>
        /// Lists the active role grants for the given user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the user's active role grants.</response>
        /// <response code="404">User not found.</response>
        [HttpGet("~/api/v{version:apiVersion}/users/{userId}/roles")]
        [ProducesResponseType(typeof(IEnumerable<UserRoleGrantDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserRoles(Guid userId, CancellationToken cancellationToken)
        {
            var user = await userManager.GetAsync(userId, cancellationToken);
            if (user is null)
            {
                return NotFound();
            }

            var grants = await userRoleService.GetActiveGrantsForUserAsync(userId, cancellationToken);
            return Ok(grants.Select(g => g.ToV2Dto()).ToList());
        }

        /// <summary>
        /// Grants a role to a user. Idempotent — if the user already holds an
        /// active grant of the role, the existing grant is returned (with
        /// <c>ExpiryDate</c> updated if supplied).
        /// </summary>
        /// <param name="userId">The user receiving the role.</param>
        /// <param name="request">The role to grant and optional expiry date.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the resulting grant.</response>
        /// <response code="400">Invalid role name.</response>
        /// <response code="404">User not found.</response>
        [HttpPost("~/api/v{version:apiVersion}/users/{userId}/roles")]
        [ProducesResponseType(typeof(UserRoleGrantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Grant(
            Guid userId,
            [FromBody] GrantRoleRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request?.RoleName))
            {
                return Problem("Role name is required.", statusCode: StatusCodes.Status400BadRequest);
            }

            var recipient = await userManager.GetAsync(userId, cancellationToken);
            if (recipient is null)
            {
                return NotFound();
            }

            logger.LogInformation(
                "V2 Grant role UserId={UserId} Role={Role} By={ActorUserId} Expiry={Expiry}",
                userId, request.RoleName, UserId, request.ExpiryDate);

            try
            {
                var grant = await userRoleService.GrantRoleAsync(userId, request.RoleName, UserId, request.ExpiryDate, cancellationToken);
                var actor = await userManager.GetAsync(UserId, cancellationToken);
                await notificationService.SendRoleGrantedAsync(grant, recipient, actor, cancellationToken);

                return Ok(grant.ToV2Dto());
            }
            catch (InvalidOperationException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
        }

        /// <summary>
        /// Revokes a user's active grant of the named role. Soft-delete: the
        /// row is retained for audit with revocation fields populated.
        /// </summary>
        /// <param name="userId">The user whose grant to revoke.</param>
        /// <param name="roleName">The role name (case-insensitive).</param>
        /// <param name="reason">Optional free-text reason recorded on the audit row.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Grant revoked.</response>
        /// <response code="404">User or active grant not found.</response>
        [HttpDelete("~/api/v{version:apiVersion}/users/{userId}/roles/{roleName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Revoke(
            Guid userId,
            string roleName,
            [FromQuery] string reason,
            CancellationToken cancellationToken)
        {
            var recipient = await userManager.GetAsync(userId, cancellationToken);
            if (recipient is null)
            {
                return NotFound();
            }

            logger.LogInformation(
                "V2 Revoke role UserId={UserId} Role={Role} By={ActorUserId}",
                userId, roleName, UserId);

            var revoked = await userRoleService.RevokeRoleAsync(userId, roleName, UserId, reason, cancellationToken);
            if (revoked is null)
            {
                return NotFound();
            }

            var actor = await userManager.GetAsync(UserId, cancellationToken);
            await notificationService.SendRoleRevokedAsync(revoked, recipient, actor, cancellationToken);

            return NoContent();
        }
    }
}
