using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrashMob.Models;
using TrashMob.Persistence;

namespace TrashMob.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MobEventsController : ControllerBase
    {
        private readonly IMobEventRepository mobEventRepository;

        public MobEventsController(IMobEventRepository mobEventRepository)
        {
            this.mobEventRepository = mobEventRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<MobEvent>> GetMobEvents()
        {
            return Ok(mobEventRepository.GetAllMobEvents());
        }

        [HttpGet("{id}")]
        public ActionResult<MobEvent> GetMobEvent(Guid id)
        {
            var mobEvent = mobEventRepository.GetMobEvent(id);

            if (mobEvent == null)
            {
                return NotFound();
            }

            return Ok(mobEvent);
        }

        // PUT: api/MobEvents/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public ActionResult PutMobEvent(Guid id, MobEvent mobEvent)
        {
            try
            {
                _ = mobEventRepository.UpdateMobEvent(mobEvent);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MobEventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/MobEvents
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Consumes("application/json")]
        public ActionResult<MobEvent> PostMobEvent(MobEvent mobEvent)
        {
            var newId = mobEventRepository.AddMobEvent(mobEvent);

            return CreatedAtAction("GetMobEvent", new { id = newId }, mobEvent);
        }

        // DELETE: api/MobEvents/5
        [HttpDelete("{id}")]
        public ActionResult DeleteMobEvent(Guid id)
        {
            mobEventRepository.DeleteMobEvent(id);
            return NoContent();
        }

        private bool MobEventExists(Guid id)
        {
            return mobEventRepository.GetAllMobEvents().Any(e => e.MobEventId == id);
        }
    }
}
