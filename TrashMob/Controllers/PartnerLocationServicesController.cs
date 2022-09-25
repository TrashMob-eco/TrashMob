namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    [Authorize]
    [Route("api/partnerlocationservices")]
    public class PartnerLocationServicesController : BaseController
    {
        private readonly IBaseManager<PartnerLocationService> partnerLocationServicesManager;

        public PartnerLocationServicesController(TelemetryClient telemetryClient,
                                                 IUserRepository userRepository,
                                                 IBaseManager<PartnerLocationService> partnerLocationServicesManager)
            : base(telemetryClient, userRepository)
        {
            this.partnerLocationServicesManager = partnerLocationServicesManager;
        }

        [HttpGet("{partnerLocationId}")]
        public async Task<IActionResult> GetPartnerLocationServices(Guid partnerLocationId, CancellationToken cancellationToken)
        {
            var partnerLocationServices = await partnerLocationServicesManager.GetByParentId(partnerLocationId, cancellationToken);

            return Ok(partnerLocationServices);
        }

        [HttpGet("{partnerLocationId}/{serviceTypeId}")]
        public async Task<IActionResult> GetPartnerLocationService(Guid partnerLocationId, int serviceTypeId, CancellationToken cancellationToken)
        {
            var partnerLocationService = await partnerLocationServicesManager.Get(partnerLocationId, serviceTypeId, cancellationToken);

            return Ok(partnerLocationService);
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerLocationService(PartnerLocationService partnerLocationService)
        {
            await partnerLocationServicesManager.Add(partnerLocationService);

            return CreatedAtAction(nameof(GetPartnerLocationService), new { partnerLocationId = partnerLocationService.PartnerLocationId, serviceTypeId = partnerLocationService.ServiceTypeId });
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartnerLocationService(PartnerLocationService partnerLocationService)
        {
            //// Make sure the person adding the user is either an admin or already a user for the partner
            //var currentUser = await GetUser();

            //if (!currentUser.IsSiteAdmin)
            //{
            //    var currentUserPartner = partnerUserRepository.GetPartnerUsers().FirstOrDefault(pu => pu.PartnerId == partnerLocation.PartnerId && pu.UserId == currentUser.Id);

            //    if (currentUserPartner == null)
            //    {
            //        return Forbid();
            //    }
            //}

            await partnerLocationServicesManager.Update(partnerLocationService).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdatePartnerLocationService));

            return Ok(partnerLocationService);
        }

        [HttpDelete("{partnerLocationId}/{serviceTypeId}")]
        public async Task<IActionResult> DeletePartnerLocationService(Guid partnerLocationId, int serviceTypeId, CancellationToken cancellationToken)
        {
            //// Make sure the person adding the user is either an admin or already a user for the partner
            //var currentUser = await GetUser();

            //if (!currentUser.IsSiteAdmin)
            //{
            //    var currentUserPartner = partnerUserRepository.GetPartnerUsers().FirstOrDefault(pu => pu.PartnerId == partnerId && pu.UserId == currentUser.Id);

            //    if (currentUserPartner == null)
            //    {
            //        return Forbid();
            //    }
            //}

            await partnerLocationServicesManager.Delete(partnerLocationId, serviceTypeId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeletePartnerLocationService));

            return Ok(partnerLocationId);
        }
    }
}
