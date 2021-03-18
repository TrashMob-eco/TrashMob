namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using TrashMob.Models;

    [ApiController]
    [Route("[controller]")]
    public class CleanupEventController : ControllerBase
    {
        private readonly ILogger<CleanupEventController> _logger;

        public CleanupEventController(ILogger<CleanupEventController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<CleanupEvent> Get()
        {
            var events = new List<CleanupEvent>
            {
                new CleanupEvent
                {
                    CleanupEventId = 1,
                    EventDate = DateTime.Now,
                    Name = "Mock Clean",
                    Address = "1 Microsoft Way",
                    Country = "USA",
                    Description = "Mock Cleanup Event",
                    ContactPhone = "Got-Junk",
                    Latitude = 25.1,
                    Longitude = 91.8,
                    MaxNumberOfParticipants = 55,
                    UserName = "Joe"
                }
            };

            return events;
        }
    }
}
