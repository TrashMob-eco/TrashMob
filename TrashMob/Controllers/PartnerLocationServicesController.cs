namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for managing partner location services, including retrieval and creation.
    /// </summary>
    [Route("api/partnerlocationservices")]
    public class PartnerLocationServicesController : SecureController
    {
        private readonly IKeyedManager<PartnerLocation> partnerLocationManager;
        private readonly IBaseManager<PartnerLocationService> partnerLocationServicesManager;
        private readonly IKeyedManager<Partner> partnerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerLocationServicesController"/> class.
        /// </summary>
        /// <param name="partnerLocationServicesManager">The partner location services manager.</param>
        /// <param name="partnerManager">The partner manager.</param>
        /// <param name="partnerLocationManager">The partner location manager.</param>
        public PartnerLocationServicesController(IBaseManager<PartnerLocationService> partnerLocationServicesManager,
            IKeyedManager<Partner> partnerManager,
            IKeyedManager<PartnerLocation> partnerLocationManager)
        {
            this.partnerLocationServicesManager = partnerLocationServicesManager;
            this.partnerManager = partnerManager;
            this.partnerLocationManager = partnerLocationManager;
        }

        /// <summary>
        /// Gets all partner location services for a given partner location. Requires a valid user.
        /// </summary>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>List of partner location services.</remarks>
        [HttpGet("getbypartnerlocation/{partnerLocationId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> Get(Guid partnerLocationId, CancellationToken cancellationToken)
        {
            var partnerLocationServices =
                await partnerLocationServicesManager.GetByParentIdAsync(partnerLocationId, cancellationToken);

            return Ok(partnerLocationServices);
        }

        /// <summary>
        /// Gets a partner location service by partner location and service type. Requires a valid user.
        /// </summary>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="serviceTypeId">The service type ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The partner location service.</remarks>
        [HttpGet("{partnerLocationId}/{serviceTypeId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> Get(Guid partnerLocationId, int serviceTypeId,
            CancellationToken cancellationToken)
        {
            var partnerLocationService =
                await partnerLocationServicesManager.GetAsync(partnerLocationId, serviceTypeId, cancellationToken);

            return Ok(partnerLocationService);
        }

        /// <summary>
        /// Adds a new partner location service.
        /// </summary>
        /// <param name="partnerLocationService">The partner location service to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The newly created partner location service.</remarks>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> Add(PartnerLocationService partnerLocationService,
            CancellationToken cancellationToken)
        {
            var partnerLocation =
                await partnerLocationManager.GetAsync(partnerLocationService.PartnerLocationId, cancellationToken);
            var partner = await partnerManager.GetAsync(partnerLocation.PartnerId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var result =
                await partnerLocationServicesManager.AddAsync(partnerLocationService, UserId, cancellationToken);

            return CreatedAtAction(nameof(Get),
                new
                {
                    partnerLocationId = partnerLocationService.PartnerLocationId,
                    serviceTypeId = partnerLocationService.ServiceTypeId,
                }, result);
        }

        /// <summary>
        /// Updates an existing partner location service.
        /// </summary>
        /// <param name="partnerLocationService">The partner location service to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The updated partner location service.</remarks>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> UpdatePartnerLocationService(PartnerLocationService partnerLocationService,
            CancellationToken cancellationToken)
        {
            var partnerLocation =
                await partnerLocationManager.GetAsync(partnerLocationService.PartnerLocationId, cancellationToken);
            var partner = await partnerManager.GetAsync(partnerLocation.PartnerId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerLocationServicesManager.UpdateAsync(partnerLocationService, UserId, cancellationToken);
            TrackEvent(nameof(UpdatePartnerLocationService));

            return Ok(partnerLocationService);
        }

        /// <summary>
        /// Deletes a partner location service.
        /// </summary>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="serviceTypeId">The service type ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The ID of the deleted partner location service.</remarks>
        [HttpDelete("{partnerLocationId}/{serviceTypeId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> DeletePartnerLocationService(Guid partnerLocationId, int serviceTypeId,
            CancellationToken cancellationToken)
        {
            var partnerLocation = await partnerLocationManager.GetAsync(partnerLocationId, cancellationToken);
            var partner = await partnerManager.GetAsync(partnerLocation.PartnerId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerLocationServicesManager.Delete(partnerLocationId, serviceTypeId, cancellationToken);
            TrackEvent(nameof(DeletePartnerLocationService));

            return Ok(partnerLocationId);
        }
    }
}