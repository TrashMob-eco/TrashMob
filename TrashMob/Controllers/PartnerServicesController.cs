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
    [Route("api/partnerservices")]
    public class PartnerServicesController : BaseController
    {
        private readonly IBaseManager<PartnerService> PartnerServicesManager;

        public PartnerServicesController(TelemetryClient telemetryClient,
                                         IUserRepository userRepository,
                                         IBaseManager<PartnerService> PartnerServicesManager)
            : base(telemetryClient, userRepository)
        {
            this.PartnerServicesManager = PartnerServicesManager;
        }

        [HttpGet("{PartnerId}")]
        public async Task<IActionResult> GetPartnerServices(Guid PartnerId, CancellationToken cancellationToken)
        {
            var PartnerServices = await PartnerServicesManager.GetByParentId(PartnerId, cancellationToken);

            return Ok(PartnerServices);
        }

        [HttpGet("{PartnerId}/{serviceTypeId}")]
        public async Task<IActionResult> GetPartnerService(Guid PartnerId, int serviceTypeId, CancellationToken cancellationToken)
        {
            var PartnerService = await PartnerServicesManager.Get(PartnerId, serviceTypeId, cancellationToken);

            return Ok(PartnerService);
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerService(PartnerService PartnerService)
        {
            await PartnerServicesManager.Add(PartnerService);

            return CreatedAtAction(nameof(GetPartnerService), new { PartnerId = PartnerService.PartnerId, serviceTypeId = PartnerService.ServiceTypeId });
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartnerService(PartnerService PartnerService)
        {
            //// Make sure the person adding the user is either an admin or already a user for the partner
            //var currentUser = await GetUser();

            //if (!currentUser.IsSiteAdmin)
            //{
            //    var currentUserPartner = partnerUserRepository.GetPartnerUsers().FirstOrDefault(pu => pu.PartnerId == Partner.PartnerId && pu.UserId == currentUser.Id);

            //    if (currentUserPartner == null)
            //    {
            //        return Forbid();
            //    }
            //}

            await PartnerServicesManager.Update(PartnerService).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdatePartnerService));

            return Ok(PartnerService);
        }

        [HttpDelete("{PartnerId}/{serviceTypeId}")]
        public async Task<IActionResult> DeletePartnerService(Guid PartnerId, int serviceTypeId, CancellationToken cancellationToken)
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

            await PartnerServicesManager.Delete(PartnerId, serviceTypeId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeletePartnerService));

            return Ok(PartnerId);
        }
    }
}
