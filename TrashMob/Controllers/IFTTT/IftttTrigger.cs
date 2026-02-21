namespace TrashMob.Controllers.IFTTT
{
    using System.Collections.Generic;

    public class IftttTrigger
    {
        public Dictionary<string, string> any_new_event_created { get; set; } = [];

        public Dictionary<string, string> new_event_created_by_country { get; set; } = new()
        {
            { "country", "United States" },
        };

        public Dictionary<string, string> new_event_created_by_region { get; set; } = new()
        {
            { "region", "Washington" },
            { "country", "United States" },
        };

        public Dictionary<string, string> new_event_created_by_city { get; set; } = new()
        {
            { "city", "Issaquah" },
            { "region", "Washington" },
            { "country", "United States" },
        };

        public Dictionary<string, string> new_event_created_by_postal_code { get; set; } = new()
        {
            { "city", "Issaquah" },
            { "region", "Washington" },
            { "country", "United States" },
            { "postal_code", "98027" },
        };
    }
}
