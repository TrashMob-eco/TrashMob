namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Controller for waiver compliance dashboard and reporting operations.
    /// All endpoints require site admin privileges.
    /// </summary>
    [Route("api/admin/waivers/compliance")]
    public class WaiverComplianceController : SecureController
    {
        private readonly IUserWaiverManager userWaiverManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaiverComplianceController"/> class.
        /// </summary>
        /// <param name="userWaiverManager">The user waiver manager.</param>
        public WaiverComplianceController(IUserWaiverManager userWaiverManager)
        {
            this.userWaiverManager = userWaiverManager;
        }

        /// <summary>
        /// Gets waiver compliance summary statistics for the admin dashboard.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Compliance summary statistics.</returns>
        [HttpGet("summary")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(WaiverComplianceSummary), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComplianceSummary(CancellationToken cancellationToken = default)
        {
            var result = await userWaiverManager.GetComplianceSummaryAsync(cancellationToken);
            TrackEvent(nameof(GetComplianceSummary));

            return Ok(result);
        }

        /// <summary>
        /// Gets paginated list of all signed user waivers with filtering options.
        /// </summary>
        /// <param name="filter">Filter parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated list of user waiver details.</returns>
        [HttpPost("waivers")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(UserWaiverListResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserWaivers(
            [FromBody] UserWaiverFilter filter,
            CancellationToken cancellationToken = default)
        {
            filter ??= new UserWaiverFilter();

            // Validate pagination
            if (filter.Page < 1) filter.Page = 1;
            if (filter.PageSize < 1) filter.PageSize = 50;
            if (filter.PageSize > 100) filter.PageSize = 100;

            var result = await userWaiverManager.GetUserWaiversFilteredAsync(filter, cancellationToken);
            TrackEvent(nameof(GetUserWaivers));

            return Ok(result);
        }

        /// <summary>
        /// Gets users with waivers expiring within the specified number of days.
        /// </summary>
        /// <param name="days">Number of days until expiry (default 30).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of users with expiring waivers.</returns>
        [HttpGet("expiring")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<Models.User>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersWithExpiringWaivers(
            [FromQuery] int days = 30,
            CancellationToken cancellationToken = default)
        {
            if (days < 1) days = 1;
            if (days > 365) days = 365;

            var result = await userWaiverManager.GetUsersWithExpiringWaiversAsync(days, cancellationToken);
            TrackEvent(nameof(GetUsersWithExpiringWaivers));

            return Ok(result);
        }

        /// <summary>
        /// Exports user waivers to CSV format for legal review.
        /// </summary>
        /// <param name="filter">Filter parameters (pagination ignored).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>CSV file download.</returns>
        [HttpPost("export")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportWaivers(
            [FromBody] UserWaiverFilter filter,
            CancellationToken cancellationToken = default)
        {
            filter ??= new UserWaiverFilter();

            var records = await userWaiverManager.GetWaiversForExportAsync(filter, cancellationToken);
            TrackEvent(nameof(ExportWaivers));

            // Generate CSV
            var csv = GenerateCsv(records);
            var bytes = Encoding.UTF8.GetBytes(csv);
            var timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);

            return File(bytes, "text/csv", $"waiver-export-{timestamp}.csv");
        }

        /// <summary>
        /// Gets a specific user waiver record with full details.
        /// </summary>
        /// <param name="userWaiverId">The user waiver ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user waiver with details.</returns>
        [HttpGet("waivers/{userWaiverId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(Models.UserWaiver), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserWaiverDetails(
            Guid userWaiverId,
            CancellationToken cancellationToken = default)
        {
            var result = await userWaiverManager.GetUserWaiverWithDetailsAsync(userWaiverId, cancellationToken);

            if (result == null)
            {
                return NotFound();
            }

            TrackEvent(nameof(GetUserWaiverDetails));

            return Ok(result);
        }

        private static string GenerateCsv(IEnumerable<WaiverExportRecord> records)
        {
            var sb = new StringBuilder();

            // Header row
            sb.AppendLine("UserWaiverId,UserId,UserName,UserEmail,TypedLegalName,WaiverName,WaiverVersion,AcceptedDate,ExpiryDate,SigningMethod,IsMinor,GuardianName,GuardianRelationship,IPAddress,UserAgent,DocumentUrl");

            // Data rows
            foreach (var record in records)
            {
                sb.AppendLine(string.Join(",",
                    EscapeCsvField(record.UserWaiverId),
                    EscapeCsvField(record.UserId),
                    EscapeCsvField(record.UserName),
                    EscapeCsvField(record.UserEmail),
                    EscapeCsvField(record.TypedLegalName),
                    EscapeCsvField(record.WaiverName),
                    EscapeCsvField(record.WaiverVersion),
                    EscapeCsvField(record.AcceptedDate),
                    EscapeCsvField(record.ExpiryDate),
                    EscapeCsvField(record.SigningMethod),
                    EscapeCsvField(record.IsMinor),
                    EscapeCsvField(record.GuardianName),
                    EscapeCsvField(record.GuardianRelationship),
                    EscapeCsvField(record.IPAddress),
                    EscapeCsvField(record.UserAgent),
                    EscapeCsvField(record.DocumentUrl)));
            }

            return sb.ToString();
        }

        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                return "";
            }

            // If field contains comma, quote, or newline, wrap in quotes and escape quotes
            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }
    }
}
