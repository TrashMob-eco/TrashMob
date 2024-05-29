namespace TrashMob.Controllers.IFTTT
{
    using System.Collections.Generic;

    public class IftttTrigger
    {
        public IftttTrigger()
        {
            any_new_event_created = new Dictionary<string, string>();

            new_event_created_by_country = new Dictionary<string, string>
            {
                { "country", "United States" },
            };

            new_event_created_by_region = new Dictionary<string, string>
            {
                { "region", "Washington" },
                { "country", "United States" },
            };

            new_event_created_by_city = new Dictionary<string, string>
            {
                { "city", "Issaquah" },
                { "region", "Washington" },
                { "country", "United States" },
            };

            new_event_created_by_postal_code = new Dictionary<string, string>
            {
                { "city", "Issaquah" },
                { "region", "Washington" },
                { "country", "United States" },
                { "postal_code", "98027" },
            };
        }

        public Dictionary<string, string> any_new_event_created { get; set; }

        public Dictionary<string, string> new_event_created_by_country { get; set; }

        public Dictionary<string, string> new_event_created_by_region { get; set; }

        public Dictionary<string, string> new_event_created_by_city { get; set; }

        public Dictionary<string, string> new_event_created_by_postal_code { get; set; }
    }
}