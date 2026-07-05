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
    /// V2 controller for the sales-report distribution list (Project 63 Phase 4b).
    /// SiteAdmin-only — the salesperson can't add themselves to Cynthia's
    /// weekly email.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/reports/sales/subscribers")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public class SalesReportSubscribersV2Controller(
        ISalesReportSubscriberService subscriberService,
        ILogger<SalesReportSubscribersV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId)
            ? parsedUserId
            : Guid.Empty;

        /// <summary>
        /// Lists every subscription, ordered by user name.
        /// </summary>
        /// <response code="200">Returns the subscriber list.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SalesReportSubscriberDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List(CancellationToken cancellationToken)
        {
            var subscribers = await subscriberService.ListAsync(cancellationToken);
            return Ok(subscribers.Select(s => s.ToV2Dto()).ToList());
        }

        /// <summary>
        /// Adds a new subscription, or updates the cadence flags on an existing
        /// one when the user is already subscribed.
        /// </summary>
        /// <param name="request">Subscription details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the created/updated subscription.</response>
        /// <response code="400">Missing or invalid <c>UserId</c>.</response>
        [HttpPost]
        [ProducesResponseType(typeof(SalesReportSubscriberDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(
            [FromBody] AddSalesReportSubscriberRequest request,
            CancellationToken cancellationToken)
        {
            if (request?.UserId == Guid.Empty)
            {
                return Problem("UserId is required.", statusCode: StatusCodes.Status400BadRequest);
            }

            logger.LogInformation(
                "V2 AddSalesReportSubscriber UserId={UserId} Weekly={Weekly} Monthly={Monthly} By={ActorId}",
                request!.UserId, request.IncludeWeekly, request.IncludeMonthly, UserId);

            var subscriber = await subscriberService.AddOrUpdateAsync(
                request.UserId,
                request.IncludeWeekly,
                request.IncludeMonthly,
                UserId,
                cancellationToken);

            // Reload with User navigation for the response payload.
            var withUser = (await subscriberService.ListAsync(cancellationToken))
                .FirstOrDefault(s => s.Id == subscriber.Id);
            return Ok((withUser ?? subscriber).ToV2Dto());
        }

        /// <summary>
        /// Updates cadence flags on an existing subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="request">Cadence flags to apply.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated subscription.</response>
        /// <response code="404">Subscription not found.</response>
        [HttpPut("{subscriptionId}")]
        [ProducesResponseType(typeof(SalesReportSubscriberDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            Guid subscriptionId,
            [FromBody] UpdateSalesReportSubscriberRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "V2 UpdateSalesReportSubscriber SubscriptionId={SubscriptionId} Weekly={Weekly} Monthly={Monthly}",
                subscriptionId, request.IncludeWeekly, request.IncludeMonthly);

            var updated = await subscriberService.UpdateAsync(
                subscriptionId,
                request.IncludeWeekly,
                request.IncludeMonthly,
                UserId,
                cancellationToken);

            if (updated == null)
            {
                return NotFound();
            }

            var withUser = (await subscriberService.ListAsync(cancellationToken))
                .FirstOrDefault(s => s.Id == subscriptionId);
            return Ok((withUser ?? updated).ToV2Dto());
        }

        /// <summary>
        /// Removes a subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Subscription removed.</response>
        /// <response code="404">Subscription not found.</response>
        [HttpDelete("{subscriptionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid subscriptionId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeleteSalesReportSubscriber SubscriptionId={SubscriptionId}", subscriptionId);

            var removed = await subscriberService.DeleteAsync(subscriptionId, cancellationToken);
            return removed ? NoContent() : NotFound();
        }
    }
}
