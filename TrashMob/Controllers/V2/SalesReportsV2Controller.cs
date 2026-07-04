namespace TrashMob.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for the municipal sales pipeline reports (Project 63).
    /// Restricted to SalesRep and SiteAdmin roles.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/reports/sales")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsSalesRepOrIsAdmin)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public class SalesReportsV2Controller(
        IWeeklySalesReportService weeklyReportService,
        IMonthlySalesReportService monthlyReportService,
        ILogger<SalesReportsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId)
            ? parsedUserId
            : Guid.Empty;

        /// <summary>
        /// Gets the weekly sales report for the seven-day window ending on
        /// <paramref name="weekEnding"/>. Defaults to today when the parameter
        /// is omitted.
        /// </summary>
        /// <param name="weekEnding">Last day of the week (inclusive) in
        /// <c>yyyy-MM-dd</c> format.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the weekly report payload.</response>
        [HttpGet("weekly")]
        [ProducesResponseType(typeof(WeeklySalesReportDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWeekly(
            [FromQuery] DateOnly? weekEnding,
            CancellationToken cancellationToken)
        {
            var effectiveWeekEnding = weekEnding ?? DateOnly.FromDateTime(DateTime.UtcNow);
            logger.LogInformation("V2 GetWeeklySalesReport WeekEnding={WeekEnding}", effectiveWeekEnding);

            var report = await weeklyReportService.GenerateAsync(effectiveWeekEnding, cancellationToken);
            return Ok(report);
        }

        /// <summary>
        /// Gets the monthly sales report for the calendar month containing
        /// <paramref name="month"/>. Defaults to the current month when the
        /// parameter is omitted.
        /// </summary>
        /// <param name="month">Any day in the desired month in
        /// <c>yyyy-MM-dd</c> format. Parsed as UTC.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the monthly report payload.</response>
        [HttpGet("monthly")]
        [ProducesResponseType(typeof(MonthlySalesReportDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMonthly(
            [FromQuery] DateOnly? month,
            CancellationToken cancellationToken)
        {
            var effectiveMonth = month ?? DateOnly.FromDateTime(DateTime.UtcNow);
            logger.LogInformation("V2 GetMonthlySalesReport Month={Month}", effectiveMonth);

            var report = await monthlyReportService.GenerateAsync(effectiveMonth, cancellationToken);
            return Ok(report);
        }

        /// <summary>
        /// Updates per-metric monthly targets. Metrics omitted from the body
        /// are left untouched. The URL <paramref name="month"/> is normalized
        /// to the first day of its calendar month.
        /// </summary>
        /// <param name="month">Any day in the target month in
        /// <c>yyyy-MM-dd</c> format.</param>
        /// <param name="request">Per-metric targets to upsert.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Targets applied.</response>
        /// <response code="400">Empty request body.</response>
        [HttpPut("monthly/{month}/targets")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMonthlyTargets(
            DateOnly month,
            [FromBody] UpdateMonthlyTargetsRequest request,
            CancellationToken cancellationToken)
        {
            if (request?.Targets == null || request.Targets.Count == 0)
            {
                return Problem("At least one target update is required.", statusCode: StatusCodes.Status400BadRequest);
            }

            logger.LogInformation(
                "V2 UpdateMonthlyTargets Month={Month} UpdateCount={Count} User={UserId}",
                month, request.Targets.Count, UserId);

            await monthlyReportService.UpdateTargetsAsync(month, request.Targets, UserId, cancellationToken);
            return NoContent();
        }
    }
}
