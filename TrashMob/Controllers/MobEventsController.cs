using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrashMob.Models;
using TrashMob.Persistence;

namespace TrashMob.Controllers
{
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
        public IEnumerable<MobEvent> GetMobEvents()
        {
            return mobEventRepository.GetAllMobEvents();
        }

        [HttpGet("{id}")]
        public ActionResult<MobEvent> GetMobEvent(Guid id)
        {
            var mobEvent = mobEventRepository.GetMobEvent(id);

            if (mobEvent == null)
            {
                return NotFound();
            }

            return mobEvent;
        }

        // PUT: api/MobEvents/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public ActionResult<Guid> PutMobEvent(Guid id, MobEvent mobEvent)
        {
            try
            {
                mobEventRepository.UpdateMobEvent(mobEvent);
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

            return NoContent();
        }

        // POST: api/MobEvents
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Consumes("application/json")]
        public ActionResult<MobEvent> PostMobEvent(MobEvent mobEvent)
        {
            mobEventRepository.AddMobEvent(mobEvent);

            return CreatedAtAction("GetMobEvent", new { id = mobEvent.MobEventId }, mobEvent);
        }

        // DELETE: api/MobEvents/5
        [HttpDelete("{id}")]
        public void DeleteMobEvent(Guid id)
        {
            mobEventRepository.DeleteMobEvent(id);
        }

        private bool MobEventExists(Guid id)
        {
            return mobEventRepository.GetAllMobEvents().Any(e => e.MobEventId == id);
        }
    }
}
