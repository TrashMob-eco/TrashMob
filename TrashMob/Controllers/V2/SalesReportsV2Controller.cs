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
        ILogger<SalesReportsV2Controller> logger) : ControllerBase
    {
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
    }
}
