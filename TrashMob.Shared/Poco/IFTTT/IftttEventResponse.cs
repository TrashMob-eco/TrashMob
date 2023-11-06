namespace TrashMob.Shared.Poco.IFTTT
{
    using System;

    public class IftttEventResponse : TriggersResponse
    {
        public string event_name { get; set; } = string.Empty;
        
        public string event_id { get;set; } = string.Empty;

        public DateTimeOffset event_date { get;set; }

        public string street_address { get; set; } = string.Empty;

        public string city { get; set; } = string.Empty;

        public string region { get; set; } = string.Empty;

        public string country { get; set; } = string.Empty;

        public string postal_code { get; set; } = string.Empty;

        public string event_details_url { get; set; } = string.Empty;
    }
}
