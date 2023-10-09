namespace TrashMob.Shared.Poco.IFTTT
{
    using System;

    public class IftttEventResponse : TriggersResponse
    {
        public string Event_Name { get; set; } = string.Empty;
        
        public string EventId { get;set; } = string.Empty;

        public DateTimeOffset EventDate { get;set; }

        public string Street_Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string Postal_Code { get; set; } = string.Empty;

        public string Event_Details_Url { get; set; } = string.Empty;
    }
}
