namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 controller for admin waiver compliance reporting and export.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/admin/waivers/compliance")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class WaiverComplianceV2Controller(
        IUserWaiverManager userWaiverManager,
        ILogger<WaiverComplianceV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets waiver compliance summary statistics.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Compliance summary statistics.</returns>
        /// <response code="200">Returns the compliance summary.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpGet("summary")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(WaiverComplianceSummary), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetComplianceSummary(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetComplianceSummary");

            var summary = await userWaiverManager.GetComplianceSummaryAsync(cancellationToken);

            return Ok(summary);
        }

        /// <summary>
        /// Gets a filtered, paginated list of user waivers.
        /// </summary>
        /// <param name="filter">The filter parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated list of user waiver details.</returns>
        /// <response code="200">Returns the filtered user waivers.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpPost("waivers")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(UserWaiverListResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserWaivers([FromBody] UserWaiverFilter filter, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetUserWaivers Page={Page} PageSize={PageSize}", filter.Page, filter.PageSize);

            if (filter.Page < 1)
            {
                return BadRequest("Page must be at least 1.");
            }

            if (filter.PageSize < 1 || filter.PageSize > 100)
            {
                return BadRequest("PageSize must be between 1 and 100.");
            }

            var result = await userWaiverManager.GetUserWaiversFilteredAsync(filter, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Gets users with waivers expiring within the specified number of days.
        /// </summary>
        /// <param name="days">Number of days until expiry (default 30, range 1-365).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of users with expiring waivers.</returns>
        /// <response code="200">Returns users with expiring waivers.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpGet("expiring")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsersWithExpiringWaivers(
            [FromQuery] int days = 30,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetUsersWithExpiringWaivers Days={Days}", days);

            if (days < 1 || days > 365)
            {
                return BadRequest("Days must be between 1 and 365.");
            }

            var users = await userWaiverManager.GetUsersWithExpiringWaiversAsync(days, cancellationToken);

            return Ok(users.Select(u => u.ToV2Dto()));
        }

        /// <summary>
        /// Exports waiver records as a CSV file.
        /// </summary>
        /// <param name="filter">The filter parameters (pagination ignored for export).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>CSV file download.</returns>
        /// <response code="200">Returns the CSV file.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpPost("export")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ExportWaivers([FromBody] UserWaiverFilter filter, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ExportWaivers");

            var records = await userWaiverManager.GetWaiversForExportAsync(filter, cancellationToken);
            var csv = GenerateCsv(records);
            var bytes = Encoding.UTF8.GetBytes(csv);

            return File(bytes, "text/csv", "waiver-export.csv");
        }

        /// <summary>
        /// Gets detailed information for a specific user waiver.
        /// </summary>
        /// <param name="userWaiverId">The user waiver ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user waiver details.</returns>
        /// <response code="200">Returns the user waiver details.</response>
        /// <response code="403">User is not an admin.</response>
        /// <response code="404">User waiver not found.</response>
        [HttpGet("waivers/{userWaiverId}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(UserWaiverDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserWaiverDetails(Guid userWaiverId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetUserWaiverDetails UserWaiverId={UserWaiverId}", userWaiverId);

            var userWaiver = await userWaiverManager.GetUserWaiverWithDetailsAsync(userWaiverId, cancellationToken);

            if (userWaiver == null)
            {
                return NotFound();
            }

            return Ok(userWaiver.ToV2Dto());
        }

        private static string GenerateCsv(IEnumerable<WaiverExportRecord> records)
        {
            var sb = new StringBuilder();

            sb.AppendLine("UserWaiverId,UserId,UserName,UserEmail,TypedLegalName,WaiverName,WaiverVersion,AcceptedDate,ExpiryDate,SigningMethod,IsMinor,GuardianName,GuardianRelationship,IPAddress,UserAgent,DocumentUrl");

            foreach (var record in records)
            {
                sb.Append(EscapeCsvField(record.UserWaiverId));
                sb.Append(',');
                sb.Append(EscapeCsvField(record.UserId));
                sb.Append(',');
                sb.Append(EscapeCsvField(record.UserName));
                sb.Append(',');
                sb.Append(EscapeCsvField(record.UserEmail));
                sb.Append(',');
                sb.Append(EscapeCsvField(record.TypedLegalName));
                sb.Append(',');
                sb.Append(EscapeCsvField(record.WaiverName));
                sb.Append(',');
                sb.Append(EscapeCsvField(record.WaiverVersion));
                sb.Append(',');
                sb.Append(EscapeCsvField(record.AcceptedDate));
                sb.Append(',');
                sb.Append(EscapeCsvField(record.ExpiryDate));
                sb.Append(',');
                sb.Append(EscapeCsvField(record.SigningMethod));
                sb.Append(',');
                sb.Append(EscapeCsvField(record.IsMinor));
                sb.Append(',');
                sb.Append(EscapeCsvField(record.GuardianName));
                sb.Append(',');
                sb.Append(EscapeCsvField(record.GuardianRelationship));
                sb.Append(',');
                sb.Append(EscapeCsvField(record.IPAddress));
                sb.Append(',');
                sb.Append(EscapeCsvField(record.UserAgent));
                sb.Append(',');
                sb.Append(EscapeCsvField(record.DocumentUrl));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return string.Empty;
            }

            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }

            return field;
        }
    }
}
